using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("quest test");
            Debug.Log(CheckIfComplete("quest test")); 
            MarkQuest("quest test", true);
            Debug.Log(CheckIfComplete("quest test"));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveQuestData();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadQuestData();
        }
    }

    public int GetQuestNumber(string questName)
    {
        int questNumber = -1;
        questNumber = Array.IndexOf(questMarkerNames, questName);
        
        return questNumber;
    }

    public bool CheckIfComplete(string questName)
    {
        bool isComplete = false;
        int questIndex = GetQuestNumber(questName);
        if (questIndex != -1)
        {
            isComplete = questMarkersComplete[questIndex];
        }
        return isComplete;
    }

    public void MarkQuest(string questName, bool isComplete)
    {
        int questIndex = GetQuestNumber(questName);
        questMarkersComplete[questIndex] = isComplete;
        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects()
    {
        List<QuestObjectActivator> questObjectActivators = FindObjectsOfType<QuestObjectActivator>().ToList();
        questObjectActivators.ForEach(i => i.CheckCompletion());
    }

    public void SaveQuestData()
    {
        for (int i = 0; i < questMarkersComplete.Length; i++)
        {
            UpdatePlayerPrefs(questMarkerNames[i],questMarkersComplete[i]);
        }
    }

    private void UpdatePlayerPrefs(string questName, bool isComplete)
    {
        if (isComplete)
        {
            PlayerPrefs.SetInt("QuestMarker_" + questName, 1);
        } else
        {
            PlayerPrefs.SetInt("QuestMarker_" + questName, 0);
        }
        
    }

    public void LoadQuestData()
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            int valueToSet = -1;
            string name = "QuestMarker_" + questMarkerNames[i];
            if (PlayerPrefs.HasKey(name))
            {
                valueToSet = PlayerPrefs.GetInt(name);
            }
            if (valueToSet != -1)
            {
                if (valueToSet == 0)
                {
                    questMarkersComplete[i] = false;
                }
                else
                {
                    questMarkersComplete[i] = true;
                }
            }
        }
    }
}
