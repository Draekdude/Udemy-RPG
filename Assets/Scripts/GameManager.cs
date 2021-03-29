 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharStats[] playersStats;
    public bool gameMenuOpen;
    public bool dialogActive;
    public bool fadingBetweenAreas;

    public List<string> itemsHeld;
    public List<int> itemsInventory;
    public List<Item> referenceItems;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas)
        {
            PlayerController.instance.canMove = false;
        } else
        {
            PlayerController.instance.canMove = true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddItem("Iron Armor");
            AddItem("TEST");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            RemoveItem("Iron Armor");
            RemoveItem("TEST");
        }
    }

    public Item GetItemDetails(string itemName)
    {
        return referenceItems.Where(i=> i.itemName == itemName).FirstOrDefault();
    }

    public void SortItems()
    {
        bool itemAfterSpace = true;
        while (itemAfterSpace)
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Count - 1; i++)
            {
                if (itemsHeld[i] == "" )
                {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";
                    itemsInventory[i] = itemsInventory[i + 1];
                    itemsInventory[i + 1] = 0;
                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string newItemName) 
    {
        var foundItemIndex = itemsHeld.FindIndex(i => i == newItemName || i == "");
        if (foundItemIndex > -1)
        {
            if (referenceItems.Find(i => i.itemName == newItemName))
            {
                itemsHeld[foundItemIndex] = newItemName;
                itemsInventory[foundItemIndex]++;
            } else {
                Debug.LogError($"{newItemName} - is invalid item");
            }
        }
        GameMenu.instance.LoadItems();
    }

    public void RemoveItem(string newItemName)
    {
        var foundItemIndex = itemsHeld.FindIndex(i => i == newItemName);
        if (foundItemIndex > -1)
        {
            if (referenceItems.Find(i => i.itemName == newItemName))
            {
                if (itemsInventory[foundItemIndex] == 1)
                {
                    itemsHeld[foundItemIndex] = "";
                }
                itemsInventory[foundItemIndex]++;
            }
            else
            {
                Debug.LogError($"{newItemName} - is invalid item");
            }
        }
        GameMenu.instance.LoadItems();
    }
}
