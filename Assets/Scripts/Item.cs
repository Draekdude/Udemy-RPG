using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon;
    public bool isArmor;

    [Header("Item Details")]
    public string itemName;
    public string description;
    public int value;
    public Sprite itemSprite;

    [Header("Item Effects")]
    public int amountToChange;
    public bool affectHp, affectMp, affectStrength;
    public int weaponStrength;
    public int armorStrength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use(int character, bool isInBattle)
    {
        CharStats selectedChar = GameManager.instance.playersStats[character];
        if (isInBattle)
        {
            selectedChar.currentHp = BattleManager.instance.activeBattlers[character].currentHp;
            selectedChar.currentMp = BattleManager.instance.activeBattlers[character].currentMp;
        }
        if (isItem)
        {
            if (affectHp)
            {
                selectedChar.currentHp += amountToChange;
                if (selectedChar.currentHp > selectedChar.maxHp)
                {
                    selectedChar.currentHp = selectedChar.maxHp;
                }
            }
            if (affectMp)
            {
                selectedChar.currentMp += amountToChange;
                if (selectedChar.currentMp > selectedChar.maxMp)
                {
                    selectedChar.currentMp = selectedChar.maxMp;
                }
            }
            if (affectStrength)
            {
                selectedChar.strength += amountToChange;
            }
        }
        if (isWeapon)
        {
            if (selectedChar.equippedWeapon != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedWeapon);
            }
            selectedChar.equippedWeapon = itemName;
            selectedChar.weaponPower = weaponStrength;
        }
        if (isArmor)
        {
            if (selectedChar.equippedArmor != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedArmor);
            }
            selectedChar.equippedArmor = itemName;
            selectedChar.armorPower = armorStrength;
        }
        GameManager.instance.RemoveItem(itemName);
        if (isInBattle)
        {
            BattleManager.instance.UpdateStats(BattleManager.instance.activeBattlers[character], selectedChar);
            BattleManager.instance.UpdateUIStats();
        }
    }
}
