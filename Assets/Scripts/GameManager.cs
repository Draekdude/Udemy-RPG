using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharStats[] playersStats;
    public bool gameMenuOpen;
    public bool dialogActive;
    public bool fadingBetweenAreas;
    public bool shopActive;
    public bool battleActive;

    public List<string> itemsHeld;
    public List<int> itemsInventory;
    public List<Item> referenceItems;

    public int currentGold;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
        SortItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive)
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadData();
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

    public bool AddItem(string newItemName) 
    {
        bool isBought = false;
        var foundItemIndex = itemsHeld.FindIndex(i => i == newItemName || i == "");
        if (foundItemIndex > -1)
        {
            if (referenceItems.Find(i => i.itemName == newItemName))
            {
                itemsHeld[foundItemIndex] = newItemName;
                itemsInventory[foundItemIndex]++;
                isBought = true;
            } else {
                Debug.LogError($"{newItemName} - is invalid item");
            }
            GameMenu.instance.LoadItems();
        }
        return isBought;
    }

    public bool RemoveItem(string newItemName)
    {
        bool isSold = false;
        var foundItemIndex = itemsHeld.FindIndex(i => i == newItemName);
        if (foundItemIndex > -1)
        {
            if (referenceItems.Find(i => i.itemName == newItemName))
            {
                if (itemsInventory[foundItemIndex] == 1)
                {
                    itemsHeld[foundItemIndex] = "";
                }
                itemsInventory[foundItemIndex]--;
                isSold = true;
            }
            else
            {
                Debug.LogError($"{newItemName} - is invalid item");
            }
            GameMenu.instance.LoadItems();
        }
        return isSold;
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

        //save char info
        playersStats.ToList().ForEach(i => SavePlayerStats(i));

        //store inventory data
        int index = 0;
        itemsHeld.ForEach(i=> PlayerPrefs.SetString($"ItemInInventory_{index++}", i));
        index = 0;
        itemsInventory.ForEach(i => PlayerPrefs.SetInt($"ItemAmount_{index++}", i));
    }

    private void SavePlayerStats(CharStats charStats)
    {
        string playerName = "Player_" + charStats.charName;
        if (charStats.gameObject.activeInHierarchy)
        {
            PlayerPrefs.SetInt($"{playerName}_active", 1);
        } else
        {
            PlayerPrefs.SetInt($"{playerName}_active", 0);
        }
        PlayerPrefs.SetInt($"{playerName}_Level", charStats.playerLevel);
        PlayerPrefs.SetInt($"{playerName}_CurrentExp", charStats.currentExp);
        PlayerPrefs.SetInt($"{playerName}_CurrentHp", charStats.currentHp);
        PlayerPrefs.SetInt($"{playerName}_CurrentMp", charStats.currentMp);
        PlayerPrefs.SetInt($"{playerName}_MaxLevel", charStats.maxLevel);
        PlayerPrefs.SetInt($"{playerName}_MaxHp", charStats.maxHp);
        PlayerPrefs.SetInt($"{playerName}_MaxMp", charStats.maxMp);
        PlayerPrefs.SetInt($"{playerName}_Strength", charStats.strength);
        PlayerPrefs.SetInt($"{playerName}_Defense", charStats.defense);
        PlayerPrefs.SetInt($"{playerName}_WeaponPower", charStats.weaponPower);
        PlayerPrefs.SetInt($"{playerName}_ArmorPower", charStats.armorPower);
        PlayerPrefs.SetString($"{playerName}_EquippedWeapon", charStats.equippedWeapon);
        PlayerPrefs.SetString($"{playerName}_EquippedArmor", charStats.equippedArmor);
    }

    public void LoadData()
    {
        float x = PlayerPrefs.GetFloat("Player_Position_x");
        float y = PlayerPrefs.GetFloat("Player_Position_y");
        float z = PlayerPrefs.GetFloat("Player_Position_z");
        PlayerController.instance.transform.position = new Vector3(x, y, z);

        //load char info
        playersStats.ToList().ForEach(i => LoadPlayerStats(i));


        //store inventory data
        int index = 0;
        var newItemsHeld = new List<string>();
        itemsHeld.ForEach(i => newItemsHeld.Add(LoadItemHeld(index++)));
        itemsHeld = newItemsHeld;
        index = 0;
        var newItemsInventory = new List<int>();
        itemsInventory.ForEach(i => newItemsInventory.Add(LoadInventory(index++)));
        itemsInventory = newItemsInventory;
    }

    public string LoadItemHeld(int index)
    {
        string value = PlayerPrefs.GetString($"ItemInInventory_{index}");
        return value;
    }

    public int LoadInventory(int index)
    {
        int value = PlayerPrefs.GetInt($"ItemAmount_{index}");
        return value;
    }

    private void LoadPlayerStats(CharStats charStats)
    {
        string playerName = "Player_" + charStats.charName;
        if (PlayerPrefs.GetInt($"{playerName}_active") == 0)
        {
            charStats.gameObject.SetActive(false);
        } else
        {
            charStats.gameObject.SetActive(true);
        }

        charStats.playerLevel = PlayerPrefs.GetInt($"{playerName}_Level");
        charStats.currentExp = PlayerPrefs.GetInt($"{playerName}_CurrentExp");
        charStats.currentHp = PlayerPrefs.GetInt($"{playerName}_CurrentHp");
        charStats.currentMp = PlayerPrefs.GetInt($"{playerName}_CurrentMp");
        charStats.maxLevel = PlayerPrefs.GetInt($"{playerName}_MaxLevel");
        charStats.maxHp = PlayerPrefs.GetInt($"{playerName}_MaxHp");
        charStats.maxMp = PlayerPrefs.GetInt($"{playerName}_MaxMp");
        charStats.strength = PlayerPrefs.GetInt($"{playerName}_Strength");
        charStats.defense = PlayerPrefs.GetInt($"{playerName}_Defense");
        charStats.equippedArmor = PlayerPrefs.GetString($"{playerName}_EquippedArmor");
        charStats.equippedWeapon = PlayerPrefs.GetString($"{playerName}_EquippedWeapon");
    }
}
