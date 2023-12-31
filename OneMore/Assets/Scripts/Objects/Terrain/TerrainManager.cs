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
    private int _countTileGoal = 4;//이거 늘리면 세대가 지나면서 땅이 점점 줄어들음
    [SerializeField]
    private int _generation = 3;//셀룰러 오토마타 몇번 돌릴껀지
    [SerializeField]
    private float _terrainHeightValue = 0.001f;//height값이 너무 높아서 낮춰야함

    [SerializeField]
    private int _noiseVelocity = 55;//0 에서 100 사이 값 클수록 벽 비율 올라감
    private int _mapSize;//Start에서 터레인 사이즈-1
    private const int _maxMapSize = 512;//이것보단 작아야함
    private int _heightMul;
    private int _heightResol;//이거랑 _terrainHeightValue 곱해야 실제 높이 나옴

    int[,] _map;
    int[,] _tempMap;

    //셀룰러 오토마타=================================================
    //인게임 시작=====================================================
    private PlayerController _playerController;
    private ObjectController _objectController;
    //인게임 시작=====================================================

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
        //좌측위
        if ((i == 0 || j == 0) == false)
            countTile += _map[i - 1,j - 1];
        //위
        if ((i == 0) == false)
            countTile += _map[i - 1,j];
        //우측위
        if ((i == 0 || j == _mapSize - 1) == false)
            countTile += _map[i - 1,j + 1];
        //======================================
        //좌측
        if (j != 0)
            countTile += _map[i,j - 1];
        //우측
        if (j != _mapSize - 1)
            countTile += _map[i,j + 1];
        //======================================
        //좌측아래
        if ((i == _mapSize - 1 || j == 0) == false)
            countTile += _map[i + 1,j - 1];
        //아래
        if ((i == _mapSize - 1) == false)
            countTile += _map[i + 1,j];
        //우측아래
        if ((i == _mapSize - 1 || j == _mapSize - 1) == false)
            countTile += _map[i + 1,j + 1];

        //결과 임시저장
        if (_countTileGoal <= countTile)
            return 1;
        else
            return 0;
    }

    void Transition()
    {
        //조건 
        //->주변타일에 벽이 4이상이면 벽 아니면 땅

        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                _tempMap[i,j] = CheckTile(i, j);
            }
        }

        //Map 적용
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

        //터레인의 가로세로중 작은부분을 큰값으로함
        int smallSize = posX < posY ? posX : posY;
        if (_maxMapSize < smallSize)
            smallSize = _maxMapSize;
        _mapSize = smallSize;
        _heightMul = _heightMul / (_mapSize - 1);

        _map = new int[_mapSize, _mapSize];
        _tempMap = new int[_mapSize, _mapSize];

        //적용하는 부분
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
        //몇번째 인덱스인지 확인
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
        //맵의 사이즈 세팅
        SetMapSize();

        //먼저 노이즈를 만듬
        MakeNoise();
        
        //셀룰러 오토마타 몇번 돌릴건지
        for (int index = 0; index < _generation; index++)
            Transition();
        
        //터레인에 셀룰러오토마타 적용
        SetHeightMap();
    }

    //셀룰러 오토마타=================================================

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
            //맵만들고
            MakeMap();

            //오브젝트배치함
            _objectController.SpawnNature(_map, _mapSize, _heightMul, (float)_heightResol * _terrainHeightValue);

            //스폰가능위치 지정후 스폰
            _playerController.SetPlayerStart(transform);
            _playerController.SpawnPlayer();

            //해당 맵으로 bake해서서 오브젝트들 움직일수있게함
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
