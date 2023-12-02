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

[Serializable]
public struct ItemData
{
    public int _key;
    public string _name;
    public int _type;
    public int _hp;
    public GameObject _prefab;
    public Sprite _icon;
    public Dictionary<int, int> _partItems;//�������,� �ʿ����� (count == 0 -> ����)
    public string _stand;//��ġ���϶� ��ġ�� stand�� �ε���(-1 == ����)
}

[Serializable]
public struct ItemInven
{
    public int _itemIndex;
    public int ItemIndex { get { return _itemIndex; }set { _itemIndex = value; } }

    public int _count;
    public int Count { get { return _count; } set { _count = value; } }

    public int _maxCount;
    public int MaxCount { get { return _maxCount; } set { _maxCount = value; } }

    public void Change(ItemInven newItem)
    {
        _itemIndex = newItem._itemIndex;
        _count = newItem._count;
        _maxCount = newItem._maxCount;
    }

    public void ClearInvenSlot()
    {
        _itemIndex = -1;
        _count = 0;
        _maxCount = 0;
    }

    public void SetInvenSlot(int index,int count)
    {
        _itemIndex = index;
        _count = count;
    }

    public void AddCount(ref ItemInven newItem)
    {

        if (_itemIndex == newItem._itemIndex)
        {
            if(_maxCount != _count)
            {
                int addedCount = _count + newItem._count;
                if(addedCount > _maxCount)
                {
                    newItem._count = addedCount - _maxCount;
                    addedCount = _maxCount;
                }
                else
                {
                    newItem._count = 0;
                }

                _count = addedCount;
            }
        }
    }
}

public class ObjectController : MonoBehaviour
{
    private TerrainManager _terrainManager;

    //===============================================================
    //���ĵ������������� ����
    private Transform _standingTransform;
    
    //��ġ�� map�� ����
    private Dictionary<int, StandingObjectData> _standingItemDatas = new Dictionary<int, StandingObjectData>();

    //terrain���� �ڿ��� ������ ����
    private List<StandingObjectData> _natureStanding = new List<StandingObjectData>();
    //===============================================================
    //������ ���� ����
    private Dictionary<int, ItemData> _itemDatas = new Dictionary<int, ItemData>(); 
    public Dictionary<int, ItemData> ItemDatas { get { return _itemDatas; } }

    //===============================================================
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
            if(colData[5] != "-")
            {
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
            }

            _standingItemDatas.Add(standingData._key, standingData);

            if((standingData._key / 1000) == 1)
            {
                _natureStanding.Add(standingData);
            }
        }
    }
    //===============================================================
    private void LoadItemData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TextData/ItemObject");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            ItemData itemData = new ItemData();
            itemData._key = int.Parse(colData[0]);
            itemData._name = colData[1];
            itemData._type = int.Parse(colData[2]);
            itemData._hp = int.Parse(colData[3]);
            itemData._prefab = Resources.Load<GameObject>("Prefabs/Item/" + colData[4]);
            
            itemData._icon = Resources.Load<Sprite>("Prefabs/Icon/" + colData[5]);

            itemData._partItems = new Dictionary<int, int>();
            if (colData[6] != "-")
            {
                string[] items = colData[6].Split("/");
                for (int index = 0; index < items.Length; index++)
                {
                    string[] words;
                    words = items[index].Split("~");

                    itemData._partItems.Add(int.Parse(words[0]), int.Parse(words[1]));
                }
            }

            if (colData[7] != "-")
                itemData._stand = colData[7];
            else
                itemData._stand = "-";

            _itemDatas.Add(itemData._key,itemData);
        }
    }



    //===============================================================
    //�ͷ��� ���� �ɶ�
    private void SetDropItem(ref Status status,Dictionary<int,DropItem> dropItems)
    {
        status.DropItems = new Dictionary<int, int>();

        //��� ������ ����
        foreach(KeyValuePair<int,DropItem> pair in dropItems)
        {
            int curCount = UnityEngine.Random.Range(pair.Value._minCount, pair.Value._maxCount + 1);

            status.DropItems.Add(pair.Key, curCount);
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
                    //Status�� ȹ�� �� ��� ������ ����
                    SetDropItem(ref curSpawnStatus,curStandingObject._dropItems);

                    curSpawnStatus.HP = curStandingObject._hp;
                    curSpawnStatus.StandingType = (StandingType)curStandingObject._type;
                }
            }
        }
    }
    //===============================================================
    //ItemInven + QuickSLot
    public ItemInven CreateItemSlot()
    {
        ItemInven itemInven = new ItemInven();
        itemInven._itemIndex = -1;
        itemInven._count = 0;
        itemInven._maxCount = 0;

        return itemInven;
    }

    //���� �ִ밳�� ��Ģ
    private int SetMaxItemSlot(int index)
    {
        int curItemMaxCount = 1;

        //������ Ÿ�� ����
        //0�� : �⺻���������� 100����
        //100�� : �������� 20�� ���� + �ż��� ����
        int itemType = _itemDatas[index]._type;

        //���⼭ Ÿ�Ժ��� ���������� �Ǵ��ϴ� csv���� �̾Ƶ���
        switch (itemType)
        {
            case 0:
                {
                    curItemMaxCount = 100;
                }
                break;
            case 100:
                {
                    curItemMaxCount = 20;
                }
                break;
        }

        return curItemMaxCount;
    }

    //===============================================================
    //�ʿ� ������ �Ѹ��� ������ �ʱ�ȭ 
    public ItemInven MakeItem(int itemIndex,int count)
    {
        ItemInven newItem = new ItemInven();
        if(_itemDatas.ContainsKey(itemIndex) == false)
        {
            //�ε��� ���� -1�� �ٲ�
            newItem.ClearInvenSlot();
            return newItem;
        }

        newItem.ItemIndex = itemIndex;
        newItem.Count = count;
        newItem.MaxCount = SetMaxItemSlot(itemIndex);//������ ��������

        return newItem;
    }

    //===============================================================
    //�ʿ� ������ ������ų��
    public void CreateItemObject(int itemIndex,int count , Vector3 pos)
    {
        Quaternion rot = new Quaternion();
        GameObject spawnObject = Instantiate(_itemDatas[itemIndex]._prefab, pos,rot);
        ItemObjectIndexCount itemIndexCount = spawnObject.GetComponent<ItemObjectIndexCount>();
        itemIndexCount.ItemIndex = itemIndex;
        itemIndexCount.ItemCount = count;

        itemIndexCount.gameObject.transform.position = pos + itemIndexCount.Position;
        itemIndexCount.gameObject.transform.rotation = itemIndexCount.Rotation;
        itemIndexCount.gameObject.transform.localScale = itemIndexCount.Scale;
    }


    //===============================================================

    private void Awake()
    {
        //������������
        LoadStandingObjectData();
        LoadItemData();

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
