using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Terrain _terrain = null;

    [SerializeField]
    private int _countTileGoal = 4;//�̰� �ø��� ���밡 �����鼭 ���� ���� �پ����
    [SerializeField]
    private int _generation = 3;//���귯 ���丶Ÿ ��� ��������
    [SerializeField]
    private float _terrainHeightValue = 0.001f;//height���� �ʹ� ���Ƽ� �������

    [SerializeField]
    private int _noiseVelocity = 55;//0 ���� 100 ���� �� Ŭ���� �� ���� �ö�
    private int _mapSize;//Start���� �ͷ��� ������-1
    private const int _maxMapSize = 512;//�̰ͺ��� �۾ƾ���
    private int _heightMul;
    private int _heightResol;//�̰Ŷ� _terrainHeightValue ���ؾ� ���� ���� ����

    int[,] _map;
    int[,] _tempMap;

    //���귯 ���丶Ÿ=================================================
    //�ΰ��� ����=====================================================
    private PlayerController _playerController;
    private ObjectController _objectController;
    //�ΰ��� ����=====================================================

    void MakeNoise()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                int TileIndex = UnityEngine.Random.Range(0,100);
                if (TileIndex > _noiseVelocity)
                    TileIndex = 1;
                else
                    TileIndex = 0;

                _map[i,j] = TileIndex;
            }
        }
    }

    int CheckTile(int i, int j)
    {
        if (i == 0 || i == _mapSize - 1 || j == 0 || j == _mapSize - 1)
        {
            return 0;
        }

        int countTile = 0;
        //======================================
        //������
        if ((i == 0 || j == 0) == false)
            countTile += _map[i - 1,j - 1];
        //��
        if ((i == 0) == false)
            countTile += _map[i - 1,j];
        //������
        if ((i == 0 || j == _mapSize - 1) == false)
            countTile += _map[i - 1,j + 1];
        //======================================
        //����
        if (j != 0)
            countTile += _map[i,j - 1];
        //����
        if (j != _mapSize - 1)
            countTile += _map[i,j + 1];
        //======================================
        //�����Ʒ�
        if ((i == _mapSize - 1 || j == 0) == false)
            countTile += _map[i + 1,j - 1];
        //�Ʒ�
        if ((i == _mapSize - 1) == false)
            countTile += _map[i + 1,j];
        //�����Ʒ�
        if ((i == _mapSize - 1 || j == _mapSize - 1) == false)
            countTile += _map[i + 1,j + 1];

        //��� �ӽ�����
        if (_countTileGoal <= countTile)
            return 1;
        else
            return 0;
    }

    void Transition()
    {
        //���� 
        //->�ֺ�Ÿ�Ͽ� ���� 4�̻��̸� �� �ƴϸ� ��

        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                _tempMap[i,j] = CheckTile(i, j);
            }
        }

        //Map ����
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                _map[i,j] = _tempMap[i,j];
            }
        }
    }

    void SetMapSize()
    {
        _heightMul = _terrain.terrainData.alphamapWidth;
        int posX = _terrain.terrainData.heightmapResolution;
        int posY = _terrain.terrainData.heightmapResolution;
        _heightResol = (int)_terrain.terrainData.heightmapScale.y;
        Debug.Log(posX + " : " + posY);

        //�ͷ����� ���μ����� �����κ��� ū��������
        int smallSize = posX < posY ? posX : posY;
        if (_maxMapSize < smallSize)
            smallSize = _maxMapSize;
        _mapSize = smallSize;
        _heightMul = _heightMul / (_mapSize - 1);

        _map = new int[_mapSize, _mapSize];
        _tempMap = new int[_mapSize, _mapSize];

        //�����ϴ� �κ�
        //float[,] heights = _terrain.terrainData.GetHeights(0, 0, posX, posY);
        //heights[posX - 1, posY - 1] = 0.5f;
        //_terrain.terrainData.SetHeights(0, 0, heights);
    }
    void SetHeightMap()
    {
        float[,] heights = new float[_mapSize, _mapSize];

        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                heights[i, j] = (float)_tempMap[i, j] * _terrainHeightValue;
            }
        }

        _terrain.terrainData.SetHeights(0, 0, heights);
    }

    public bool IsMaxHeight(float posX, float posZ)
    {
        //���° �ε������� Ȯ��
        int indexX = (int)posX / _heightMul;
        int indexZ = (int)posZ / _heightMul;

        float heightCount = 0;
        heightCount += _map[indexX, indexZ];
        heightCount += _map[indexX + 1, indexZ];
        heightCount += _map[indexX, indexZ + 1];
        heightCount += _map[indexX + 1, indexZ + 1];

        if(heightCount < 4)
            return false;
        return true;
    }

    void MakeMap()
    {
        //���� ������ ����
        SetMapSize();

        //���� ����� ����
        MakeNoise();
        
        //���귯 ���丶Ÿ ��� ��������
        for (int index = 0; index < _generation; index++)
            Transition();
        
        //�ͷ��ο� ���귯���丶Ÿ ����
        SetHeightMap();
    }

    //���귯 ���丶Ÿ=================================================

    void BakeMap()
    {
        GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurfaceControl>().CheckNMSBake();
    }

    private void Awake()
    {
        _playerController = GameObject.Find("InGameManager").GetComponentInChildren<PlayerController>();
        _objectController = GameObject.Find("InGameManager").GetComponentInChildren<ObjectController>();
    }

    void Start()
    {
        _terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (_terrain != null)
        {
            //�ʸ����
            MakeMap();

            //������Ʈ��ġ��
            _objectController.SpawnNature(_map, _mapSize, _heightMul, (float)_heightResol * _terrainHeightValue);

            //����������ġ ������ ����
            _playerController.SetPlayerStart(transform);
            _playerController.SpawnPlayer();

            //�ش� ������ bake�ؼ��� ������Ʈ�� �����ϼ��ְ���
            _playerController.SetPlayerPos();
            BakeMap();

            UnityEngine.Vector3 tempPos = new Vector3();
            tempPos.x = 30;
            tempPos.y = 0.8f;
            tempPos.z = 30;
            
            for(int i=0;i<10; i++)
            {
                tempPos.z += i;
                _objectController.CreateItemObject(100,18, tempPos);
            }
        }
        else
            Debug.Log("Terrain Error");

    }

    void Update()
    {
        
    }
}
