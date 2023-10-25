using System;
using System.Collections;
using System.Collections.Generic;
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

    int[,] _map;
    int[,] _tempMap;

    //셀룰러 오토마타=================================================
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
        int posX = _terrain.terrainData.heightmapResolution;
        int posY = _terrain.terrainData.heightmapResolution;
        Debug.Log(posX + " : " + posY);

        //터레인의 가로세로중 작은부분을 큰값으로함
        int smallSize = posX < posY ? posX : posY;
        if (_maxMapSize < smallSize)
            smallSize = _maxMapSize;
        _mapSize = smallSize;

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

    void Start()
    {
        _terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (_terrain != null)
        {
            MakeMap();
        }
        else
            Debug.Log("Terrain Error");

    }

    void Update()
    {
        
    }
}
