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

    public Item seletedItem;
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
        buyItemButtons.ForEach(i => SetUpButton(index++, i, itemsForSale));
        buyItemButtons.First().Press();
    }

    public void OpenSellMenu()
    {
        buyMenu.SetActive(false);
        sellMenu.SetActive(true);
        int index = 0;
        sellItemButtons.ForEach(i => SetUpButton(index++, i, GameManager.instance.itemsHeld.ToArray()));
        sellItemButtons.First().Press();
    }

    public ItemButton SetUpButton(int index, ItemButton itemButton, string[] items)
    {
        if (items[index] != "")
        {
            itemButton.buttonImage.gameObject.SetActive(true);
            Item foundItem = GameManager.instance.GetItemDetails(items[index]);
            itemButton.buttonImage.sprite = foundItem.itemSprite;
            itemButton.amountText.text = "";
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
        seletedItem = item;
        buyItemName.text = seletedItem.itemName;
        buyItemDesc.text = seletedItem.description;
        buyItemValue.text = $"Value: {seletedItem.value}g";
    }

    public void SelectSellItem(Item item)
    {
        seletedItem = item;
        sellItemName.text = seletedItem.itemName;
        sellItemDesc.text = seletedItem.description;
        sellItemValue.text = $"Value: {Mathf.FloorToInt(seletedItem.value * 0.5f).ToString()}g";
    }
}
