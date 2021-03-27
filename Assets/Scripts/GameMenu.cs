using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{

    public GameObject menu;

    private CharStats[] playerStats;

    public Text[] nameText, hpText, mpText, levelText, xpText;
    public Slider[] xpSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

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
                menu.SetActive(false);
                GameManager.instance.gameMenuOpen = false;
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
}
