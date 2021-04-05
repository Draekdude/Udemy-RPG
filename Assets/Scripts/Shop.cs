using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public GameObject shopMenu;
    public GameObject buyMenu;
    public GameObject sellMenu;

    public Text goldText;

    public string[] itemsForSale;

    public List<ItemButton> buyItemButtons;
    public List<ItemButton> sellItemButtons;

    public Item selectedItem;
    public Text buyItemName, buyItemDesc, buyItemValue;
    public Text sellItemName, sellItemDesc, sellItemValue;

    // Start is called before the first frame update
    void Start()
    {  
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
        GameManager.instance.shopActive = true;
        OpenBuyMenu();
        goldText.text = GameManager.instance.currentGold.ToString() + "g";
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
        GameManager.instance.shopActive = false;
    }

    public void OpenBuyMenu()
    {
        buyMenu.SetActive(true);
        sellMenu.SetActive(false);
        //GameManager.instance.SortItems();
        int index = 0;
        buyItemButtons.ForEach(i => SetUpButton(index++, i, itemsForSale, false));
        buyItemButtons.First().Press();
    }

    public void OpenSellMenu()
    {
        
        buyMenu.SetActive(false);
        sellMenu.SetActive(true);
        //GameManager.instance.SortItems();
        int index = 0;
        sellItemButtons.ForEach(i => SetUpButton(index++, i, GameManager.instance.itemsHeld.ToArray(), true));
        sellItemButtons.First().Press();
    }

    public ItemButton SetUpButton(int index, ItemButton itemButton, string[] items, bool isSell)
    {
        if (items[index] != "")
        {
            itemButton.buttonImage.gameObject.SetActive(true);
            Item foundItem = GameManager.instance.GetItemDetails(items[index]);
            itemButton.buttonImage.sprite = foundItem.itemSprite;
            itemButton.amountText.text = "";
            if (isSell)
            {
                itemButton.amountText.text = GameManager.instance.itemsInventory[index].ToString();
            }
            itemButton.buttonValue = index;
        }
        else
        {
            itemButton.buttonImage.gameObject.SetActive(false);
            itemButton.amountText.text = "";
        }
        return itemButton;
    }

    public void SelectBuyItem(Item item)
    {
        selectedItem = item;
        buyItemName.text = selectedItem.itemName;
        buyItemDesc.text = selectedItem.description;
        buyItemValue.text = $"Value: {selectedItem.value}g";
    }

    public void SelectSellItem(Item item)
    {
        selectedItem = item;
        sellItemName.text = selectedItem.itemName;
        sellItemDesc.text = selectedItem.description;
        sellItemValue.text = $"Value: {Mathf.FloorToInt(selectedItem.value * 0.5f).ToString()}g";
    }

    public void BuyItem()
    {
        if (selectedItem != null && GameManager.instance.currentGold >= selectedItem.value && GameManager.instance.AddItem(selectedItem.itemName))
        {
            GameManager.instance.currentGold -= selectedItem.value;
            goldText.text = GameManager.instance.currentGold.ToString() + "g";
        }
    }

    public void SellItem()
    {
        if (selectedItem != null && GameManager.instance.RemoveItem(selectedItem.itemName))
        {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.value * 0.5f);
            goldText.text = GameManager.instance.currentGold.ToString() + "g";
            int index = 0;
            sellItemButtons.ForEach(i => SetUpButton(index++, i, GameManager.instance.itemsHeld.ToArray(), true));
        }
    }
}
