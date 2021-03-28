﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameMenu : MonoBehaviour
{

    public GameObject menu;

    private CharStats[] playerStats;

    public Text[] nameText, hpText, mpText, levelText, xpText;
    public Slider[] xpSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;
    public List<GameObject> windows;

    public List<GameObject> statusButtons;

    public GameObject playerStatLabels;
    public Image statusImage;

    public List<ItemButton> itemButtons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) 
        {
            if (menu.activeInHierarchy)
            {
                CloseMenu();
            } else
            {
                menu.SetActive(true);
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }
        }
    }

    public void UpdateMainStats()
    {
        playerStats = GameManager.instance.playersStats;
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatHolder[i].SetActive(true);
                nameText[i].text = playerStats[i].charName;
                hpText[i].text = $"HP: {playerStats[i].currentHp}/{playerStats[i].maxHp} ";
                mpText[i].text = $"MP: {playerStats[i].currentMp}/{playerStats[i].maxMp} ";
                levelText[i].text = $"Level: {playerStats[i].playerLevel}";
                xpText[i].text = $"{playerStats[i].currentExp}/{playerStats[i].expToNextLevel[playerStats[i].playerLevel]}";
                xpSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                xpSlider[i].value = playerStats[i].currentExp;
                charImage[i].sprite = playerStats[i].characterImage;
            } else
            {
                charStatHolder[i].SetActive(false);
            }
        }
    }

    public void UpdatePlayerStats(int playerIndex)
    {
        playerStats = GameManager.instance.playersStats;
        var textValueFields = playerStatLabels.GetComponentsInChildren<Text>();
        textValueFields.SingleOrDefault(item => item.name == "Name").text = playerStats[playerIndex].charName;
        textValueFields.SingleOrDefault(item => item.name == "Name").text = playerStats[playerIndex].charName;
        textValueFields.SingleOrDefault(item => item.name == "HP").text = $"{playerStats[playerIndex].currentHp}/{playerStats[playerIndex].maxHp}";
        textValueFields.SingleOrDefault(item => item.name == "MP").text = $"{playerStats[playerIndex].currentMp}/{playerStats[playerIndex].maxMp}";
        textValueFields.SingleOrDefault(item => item.name == "Strength").text = playerStats[playerIndex].strength.ToString();
        textValueFields.SingleOrDefault(item => item.name == "Defence").text = playerStats[playerIndex].defense.ToString();
        textValueFields.SingleOrDefault(item => item.name == "EquippedWeapon").text = playerStats[playerIndex].equippedWeapon == null ? playerStats[playerIndex].equippedWeapon : "None";
        textValueFields.SingleOrDefault(item => item.name == "EquippedArmor").text = playerStats[playerIndex].equippedArmor == null ? playerStats[playerIndex].equippedArmor : "None";
        textValueFields.SingleOrDefault(item => item.name == "ArmorPower").text = playerStats[playerIndex].armorPower.ToString();
        textValueFields.SingleOrDefault(item => item.name == "XpToNextLevel").text = $"{playerStats[playerIndex].expToNextLevel[playerStats[playerIndex].playerLevel] - playerStats[playerIndex].currentExp}";
        statusImage.sprite = playerStats[playerIndex].characterImage;
    }

    public void ToggleWindow(int windowIndex)
    {
        UpdateMainStats();
        bool isActive = windows[windowIndex].activeInHierarchy;
        windows.ForEach(w => w.SetActive(false));
        windows[windowIndex].SetActive(!isActive);
    }

    public void CloseMenu()
    {
        windows.ForEach(w => w.SetActive(false));
        menu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;
    }

    public void OpenStatus()
    {
        UpdateMainStats();
        for (int i = 0; i < statusButtons.Count; i++)
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName;
            
        }
        UpdatePlayerStats(0);
    }

    public void LoadItems()
    {
        GameManager.instance.SortItems();
        int index = 0;
        itemButtons.ForEach(i => SetUpButton(index++, i));
    }

    public ItemButton SetUpButton(int index, ItemButton itemButton)
    {
        if (GameManager.instance.itemsHeld[index] != "")
        {
            itemButton.buttonImage.gameObject.SetActive(true);
            Item foundItem = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[index]);
            itemButton.buttonImage.sprite = foundItem.itemSprite;
            itemButton.amountText.text = GameManager.instance.itemsInventory[index].ToString();
        } else
        {
            itemButton.buttonImage.gameObject.SetActive(false);
            itemButton.amountText.text = "";
        }
        return itemButton;
    }
}
