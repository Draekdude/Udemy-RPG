 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    public string charName;
    public int playerLevel = 1;
    public int currentExp;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseExp = 1000;

    public int currentHp;
    public int maxHp = 100;
    public int currentMp;
    public int maxMp = 30;
    public int[] mpLevelBonus;
    public int strength;
    public int defense;
    public int weaponPower;
    public int armorPower;
    public string equippedWeapon;
    public string equippedArmor;
    public Sprite characterImage;

    // Start is called before the first frame update
    void Start()
    {
        expToNextLevel = new int[maxLevel];
        int lastValue = 1000;
        expToNextLevel[1] = lastValue;
        for (int i = 2; i < expToNextLevel.Length; i++)
        {
            expToNextLevel[i] = Mathf.FloorToInt(lastValue * 1.05f);
            lastValue = expToNextLevel[i];
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddExp(500);
        }
    }

    public void AddExp(int expToAdd)
    {
        currentExp += expToAdd;
        if (playerLevel < maxLevel && currentExp > expToNextLevel[playerLevel])
        {
            currentExp -= expToNextLevel[playerLevel];
            playerLevel++;

            //add to strength or defense
            if (playerLevel % 2 == 0)
            {
                strength++;
            }
            else
            {
                defense++;
            }

            maxHp = Mathf.FloorToInt(maxHp * 1.05f);
            currentHp = maxHp;

            maxMp += mpLevelBonus[playerLevel];
            currentMp = maxMp;
        } else if (playerLevel >= maxLevel)
        {
            currentExp = 0;
        }
    }
}
