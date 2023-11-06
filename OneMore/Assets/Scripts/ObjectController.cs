using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//StandingObject.csv ������ ���� ����ü
[Serializable]
public struct DropItem
{
    public int _itemIndex;
    public int _minCount;
    public int _maxCount;
}

[Serializable]
public struct StandingObjectData
{
    public int _key;
    public string _name;
    public int _type;
    public int _hp;
    public GameObject _prefab;
    public Dictionary<int, DropItem> _dropItems;
}

public class ObjectController : MonoBehaviour
{
    private TerrainManager _terrainManager;
    //���ĵ������������� ����
    private Transform _standingTransform;

    private Dictionary<int, StandingObjectData> _standingItemDatas = new Dictionary<int, StandingObjectData>();
    private List<StandingObjectData> _natureStanding = new List<StandingObjectData>();

    private void LoadStandingObjectData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TextData/StandingObject");
        
        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1;i<rowData.Length;i++)
        {
            string[] colData = rowData[i].Split(",");

            StandingObjectData standingData = new StandingObjectData();
            standingData._key = int.Parse(colData[0]);
            standingData._name = colData[1];
            standingData._type = int.Parse(colData[2]);
            standingData._hp = int.Parse(colData[3]);
            standingData._prefab = Resources.Load<GameObject>("Prefabs/StandingObject/" + colData[4]);

            standingData._dropItems = new Dictionary<int, DropItem>();
            string[] items = colData[5].Split("/");
            for(int index = 0;index < items.Length;index++)
            {
                string[] words;
                words = items[index].Split("~");

                DropItem dropItem = new DropItem();
                dropItem._itemIndex = int.Parse(words[0]);
                dropItem._minCount = int.Parse(words[1]);
                dropItem._maxCount = int.Parse(words[2]);
                standingData._dropItems.Add(dropItem._itemIndex, dropItem);
            }

            _standingItemDatas.Add(standingData._key, standingData);

            if((standingData._key / 1000) == 1)
            {
                _natureStanding.Add(standingData);
            }
        }
    }

    public void SpawnNature(int[,] map,int mapSize,int heightMul,float tileHeight)
    {
        int maxNatureCount = _natureStanding.Count;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if(map[i,j] >= 1)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0) continue;

                    Vector3 pos;
                    //�ش�Ÿ�Ͽ��� heightMul������(+2/-2)�� �������� �߰��� Ÿ���� ������ġ�� ����
                    float ranRange = (float)(heightMul / 2.0f) - 0.1f;
                    float ranX = UnityEngine.Random.Range(-ranRange, ranRange);
                    float ranY = UnityEngine.Random.Range(-ranRange, ranRange);
                    pos.x = j * heightMul + ranX; pos.z = i * heightMul + ranY;
                    if (_terrainManager.IsMaxHeight(pos.x, pos.z) == false) continue;
                    pos.y = tileHeight;
                    //����
                    Quaternion quat = new Quaternion();
                    quat.y = UnityEngine.Random.Range(0, 360);

                    //���� �ڿ��� �ε��� ����
                    int ranPrefab = UnityEngine.Random.Range(0, maxNatureCount);

                    //���� �ڿ���������
                    StandingObjectData curStandingObject = _natureStanding[ranPrefab];

                    //���� ����
                    GameObject spawnObject = Instantiate(curStandingObject._prefab, pos, quat);

                    spawnObject.transform.parent = _standingTransform;

                    //������ �۹̿�����Ʈ�� ��,���� ����
                    Status curSpawnStatus = spawnObject.GetComponent<Status>();
                    curSpawnStatus.HP = curStandingObject._hp;
                    curSpawnStatus.StandingType = (StandingType)curStandingObject._type;
                }
            }
        }
    }

    private void Awake()
    {
        LoadStandingObjectData();
        _terrainManager = GameObject.Find("Terrain").GetComponent<TerrainManager>();

        _standingTransform = GameObject.Find("StandingObjects").GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
