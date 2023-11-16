using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int _invenSize = 24;
    private int _quickSlotSize = 6;

    [SerializeField]
    private List<ItemInven> _inventoryList = new List<ItemInven>();//<-- 인벤토리
    public List<ItemInven> InventoryList { get { return _inventoryList; } }

    [SerializeField]
    private List<ItemInven> _quickSlotList = new List<ItemInven>();//<-- 퀵슬롯
    public List<ItemInven> QuickSlotList { get { return _quickSlotList; } }

    private UIManager _uiManager;
    private ObjectController _objectController;
    //===============================================================

    private void AddItemToList(ref ItemInven item,List<ItemInven> itemList)
    {
        int curItemIndex = item._itemIndex;

        for(int index=0;index < itemList.Count;index++)
        {
            if (item._count == 0) return;

            int slotIndex = itemList[index]._itemIndex;

            if (slotIndex == -1)//만약 비어있을 경우
            {
                ItemInven tempInven = itemList[index];
                tempInven.Change(item);

                itemList[index] = tempInven;

                item._count = 0;
            }
            else if (slotIndex == curItemIndex)//아이템 종류 가 같을때
            {
                ItemInven tempInven = itemList[index];
                tempInven.AddCount(ref item);

                itemList[index] = tempInven;
            }
        }
    }

    public void AddItem_IndexCount(int index,int count)
    {
        AddItem(_objectController.MakeItem(index, count));
    }

    //그냥 추가가될때
    public bool AddItem(ItemInven item)
    {
        AddItemToList(ref item,_quickSlotList);
        AddItemToList(ref item,_inventoryList);

        //하고 남는거 있으면 뱉어냄
        //CreateRestItem();

        UpdateInventoryUI();
        return false;
    }

    void UpdateInventoryUI()
    {

        //참조하여 inven + quickSlot 최신화
        _uiManager.UpdateInven(this);
    }
    //===============================================================
    private void InvenQuickSlotSetting()
    {
        for(int index=0;index < _invenSize;index++)
        {
            _inventoryList.Add(_objectController.CreateItemSlot());
        }

        for (int index = 0; index < _quickSlotSize; index++)
        {
            _quickSlotList.Add(_objectController.CreateItemSlot());
        }
    }

    //===============================================================
    private void Awake()
    {
        _uiManager = GameObject.Find("InGameManager").GetComponent<UIManager>();
        _objectController = GameObject.Find("InGameManager").GetComponent<ObjectController>();
         
        InvenQuickSlotSetting();
    }

    private void LateUpdate()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _uiManager.UpdateInven(this);
    }

    // Update is called once per frame
    void Update()
    {
        //임시로 아이템 추가
        //{
        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        AddItem(_objectController.MakeItem(0,30));
        //    }
        //}

    }
}
