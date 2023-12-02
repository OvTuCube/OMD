using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //아이템 정보를 불러오기위해
    private ObjectController _objectController;

    //아래쪽 UI
    private GameObject _quickSlot;//<==
    private List<GameObject> _quickSlotItems = new List<GameObject>();

    //인벤/제작 UI
    private GameObject _itemSlot;//<==
    private bool _isItemSlot = true;
    private GameObject _inventory;//<==
    private List<GameObject> _inventoryItems = new List<GameObject>();

    private GameObject _craft;//<==
    private GameObject _craftContent;

    //상태창
    private GameObject _statusSlot;

    //===============================================================
    private void CraftSetting()
    {

    }

    private void InvenQuickSlotUISetting()
    {
        _itemSlot = GameObject.Find("ItemSlot").gameObject;

        _quickSlot = GameObject.Find("QuickSlot").gameObject;
        _inventory = GameObject.Find("Inventory").gameObject;
        _craft = GameObject.Find("Craft").gameObject;
        _craftContent = GameObject.Find("CraftContent").gameObject;

        //퀵슬롯 초기세팅
        Button[] quickSlotItems = _quickSlot.GetComponentsInChildren<Button>();
        for (int index = 0; index < quickSlotItems.Count(); index++)
        {
            _quickSlotItems.Add(quickSlotItems[index].gameObject);
        }

        //인벤 초기세팅
        Button[] invenItems = _inventory.GetComponentsInChildren<Button>();
        for(int index = 0; index < invenItems.Count(); index++)
        {
            _inventoryItems.Add(invenItems[index].gameObject);
        }

        //크레프트 초기세팅
        CraftSetting();
    }

    //===============================================================
    public void UpdateInven(PlayerInventory inven)
    {
        List<ItemInven> quickSlots = inven.QuickSlotList;

        for(int index = 0;index < quickSlots.Count();index++)
        {
            Image curImage = _quickSlotItems[index].gameObject.GetComponent<Image>();

            SetItemSlotIcon(ref curImage, quickSlots[index]._itemIndex);

            TextMeshProUGUI countText = _quickSlotItems[index].GetComponentInChildren<TextMeshProUGUI>();

            if (quickSlots[index]._itemIndex == -1)
            {
                countText.enabled = false;
            }
            else
            {
                countText.text = (quickSlots[index]._count).ToString();
                countText.enabled = true;
            }
        }

        List<ItemInven> itemInvens = inven.InventoryList;

        for (int index = 0; index < itemInvens.Count(); index++)
        {
            Image curImage = _inventoryItems[index].gameObject.GetComponent<Image>();

            SetItemSlotIcon(ref curImage, itemInvens[index]._itemIndex);

            TextMeshProUGUI countText = _inventoryItems[index].GetComponentInChildren<TextMeshProUGUI>();

            if (itemInvens[index]._itemIndex == -1)
            {
                countText.enabled = false;
            }
            else
            {
                countText.text = (itemInvens[index]._count).ToString();
                countText.enabled = true;
            }

        }
    }

    private void SetItemSlotIcon(ref Image curImage,int index)
    {
        Color curColor = curImage.color;

        if (index == -1)
        {
            curImage.sprite = null;

            curColor.a = 0.125f;
            curImage.color = curColor;

            return;
        }

        curColor.a = 1.0f;
        curImage.color = curColor;

        ItemData tempData = _objectController.ItemDatas[index];
        curImage.sprite = tempData._icon;
    }

    void CraftItemSetting()
    {
        GameObject craftItemButtonPrefab = Resources.Load<GameObject>("Prefabs/UI/CraftItemButton"); ;

        Dictionary<int, ItemData> craftItemDatas = _objectController.ItemDatas;

        foreach(KeyValuePair<int, ItemData> item in craftItemDatas)
        {
            if(item.Value._type >= 400)
            {
                GameObject curCraftItem = Instantiate(craftItemButtonPrefab);

                curCraftItem.transform.parent = _craftContent.transform;

                Vector3 tempCraftItemPos;
                tempCraftItemPos.x = 45;
                tempCraftItemPos.y = -45;
                tempCraftItemPos.z = 0;
                curCraftItem.GetComponent<RectTransform>().transform.localPosition = tempCraftItemPos;
            }
        }



    }

    //===============================================================
    //UI컨트롤(On.Off)
    public void OnClickedInventoryUI()
    {
        _inventory.SetActive(true);
        _craft.SetActive(false);
    }

    public void OnClickedCraftUI()
    {
        _craft.SetActive(true);
        _inventory.SetActive(false);
    }

    public void ItemSlotUI()
    {
        _isItemSlot = !_isItemSlot;

        if (_isItemSlot) 
        {
            _itemSlot.SetActive(true);
            OnClickedInventoryUI();
        }
        else
        {
            _itemSlot.SetActive(false);
        }
    }

    //===============================================================
    private void Awake()
    {
        _objectController = GameObject.Find("InGameManager").GetComponent<ObjectController>();

        //UI 저장
        //_itemSlot

        InvenQuickSlotUISetting();
    }

    // Start is called before the first frame update
    void Start()
    {
        //인벤토리창 내리기
        ItemSlotUI();

        //제작창의 아이템들 배치
        CraftItemSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
