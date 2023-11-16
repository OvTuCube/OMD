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
    private GameObject _quickSlot;
    private List<GameObject> _quickSlotItems = new List<GameObject>();

    //인벤/제작 UI
    private GameObject _menuPanel;

    private GameObject _inventory;
    private List<GameObject> _inventoryItems = new List<GameObject>();

    private GameObject _craft;

    //상태창
    private GameObject _statusSlot;

    //===============================================================
    private void InvenQuickSlotUISetting()
    {
        _quickSlot = GameObject.Find("QuickSlot").gameObject;
        _inventory = GameObject.Find("Inventory").gameObject;

        Button[] quickSlotItems = _quickSlot.GetComponentsInChildren<Button>();
        for (int index = 0; index < quickSlotItems.Count(); index++)
        {
            _quickSlotItems.Add(quickSlotItems[index].gameObject);
        }

        Button[] invenItems = _inventory.GetComponentsInChildren<Button>();
        for(int index = 0; index < invenItems.Count(); index++)
        {
            _inventoryItems.Add(invenItems[index].gameObject);
        }
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

    //===============================================================
    private void Awake()
    {
        _objectController = GameObject.Find("InGameManager").GetComponent<ObjectController>();
        
        InvenQuickSlotUISetting();
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
