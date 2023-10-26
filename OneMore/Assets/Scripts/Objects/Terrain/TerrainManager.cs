using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Terrain _terrain = null;

    public NavMeshSurface _navMeshSurface;

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

    int[,] _map;
    int[,] _tempMap;

    //���귯 ���丶Ÿ=================================================
    //�ΰ��� ����=====================================================
    private PlayerController _playerController;

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
        int posX = _terrain.terrainData.heightmapResolution;
        int posY = _terrain.terrainData.heightmapResolution;
        Debug.Log(posX + " : " + posY);

        //�ͷ����� ���μ����� �����κ��� ū��������
        int smallSize = posX < posY ? posX : posY;
        if (_maxMapSize < smallSize)
            smallSize = _maxMapSize;
        _mapSize = smallSize;

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
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
        if(_navMeshSurface)
            _navMeshSurface.BuildNavMesh();
    }

    private void Awake()
    {
        _playerController = GameObject.Find("InGameManager").GetComponentInChildren<PlayerController>();
    }

    void Start()
    {
        _terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (_terrain != null)
        {
            //�ʸ����
            MakeMap();
            //������Ʈ��ġ��
            //SceneController.FirstSpawn(�ͷ���,2�����迭);
            //�ش� ������ bake�ؼ��� ������Ʈ�� �����ϼ��ְ���
            BakeMap();
            //GameStart();
            //����������ġ ������ ����
            _playerController.SetPlayerStart(transform);
            _playerController.SpawnPlayer();
        }
        else
            Debug.Log("Terrain Error");

    }

    void Update()
    {
        
    }
}
