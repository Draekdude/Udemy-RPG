using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleReward : MonoBehaviour
{
    public static BattleReward instance;
    public Text xpText, itemText;
    public GameObject rewardScreen;
    public List<string> rewardItems;
    public int xpEarned;
    public bool markQuestComplete;
    public string questToMark;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            OpenRewardScreen(54, new List<string> { "Iron Sword", "Iron Armor" });
        }
    }

    public void OpenRewardScreen(int xp, List<string> rewards)
    {
        xpEarned = xp;
        rewardItems = rewards;
        xpText.text = $"Everyone earned {xpEarned} xp!";
        itemText.text = "";
        rewardItems.ForEach(r => itemText.text += $"{r}\n");
        rewardScreen.SetActive(true);
    }

    public void CloseRewardScreen()
    {
        GameManager.instance.playersStats.ToList().ForEach(p =>
        {
            if (p.gameObject.activeInHierarchy)
            {
                p.AddExp(xpEarned);
            }
        });

        rewardItems.ForEach(r => GameManager.instance.AddItem(r));

        GameManager.instance.battleActive = false;
        rewardScreen.SetActive(false);
        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuest(questToMark, true);
        }
    }
}
