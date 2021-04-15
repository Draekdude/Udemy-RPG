﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public GameObject uiScreen;
    public GameObject player;
    public GameObject gameManager;
    public GameObject audioManager;
    public GameObject battleManager;

    // Start is called before the first frame update
    void Start()
    {
        if (UIFade.instance == null)
        {
            UIFade.instance = Instantiate(uiScreen).GetComponent<UIFade>();
        }
        if (PlayerController.instance == null)
        {
            PlayerController.instance = Instantiate(player).GetComponent<PlayerController>();
        }
        if (GameManager.instance == null)
        {
            GameManager.instance = Instantiate(gameManager).GetComponent<GameManager>();
        }
        if (AudioManager.instance == null)
        {
            AudioManager.instance = Instantiate(audioManager).GetComponent<AudioManager>();
        }
        if (BattleManager.instance == null)
        {
            BattleManager.instance = Instantiate(battleManager).GetComponent<BattleManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
