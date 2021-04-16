using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private bool battleActive;
    public GameObject battleScene;
    public Transform[] playerPositions;
    public Transform[] enemyPositions;
    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;

    public GameObject uiButtonsHolder;
    public GameObject statsMenu;

    public List<BattleMove> movesList;

    public GameObject enemyAttackEffect;

    public DamageNumber damageNumber;

    public List<Text> playerName, playerHp, playerMp;

    public GameObject targetMenu;
    public List<BattleTargetButton> targetButtons;

    public GameObject magicMenu;

    public List<BattleMagicSelect> magicButtons;

    public BattleNotification battleNote;

    public int chanceToFlee = 30;

    public string gameOverScene;

    private bool fleeing = false;
    public int rewardXP;
    public List<string> rewardItems;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Eyeball", "Spider", "Skeleton" });
        }
        if (battleActive)
        {
            if (turnWaiting)
            {
                bool isPlayer = activeBattlers[currentTurn].isPlayer;
                if (Shop.instance.itemMenu.activeInHierarchy == false)
                {
                    uiButtonsHolder.SetActive(isPlayer);
                    statsMenu.SetActive(true);
                }
                
                if (!isPlayer)
                {
                    //enemy should attack
                    turnWaiting = false;
                    StartCoroutine(EnemyMoveCo());
                }
                
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
        }
    }

    public void BattleStart(string[] enemiesToSpawn)
    {
        if (!battleActive)
        {
            battleActive = true;
            GameManager.instance.battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);
            AudioManager.instance.PlayMusic(0);
            AddPlayers();
            AddEnemies(enemiesToSpawn);
            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count);
            UpdateUIStats();
        }
    }

    private void AddEnemies(string[] enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (enemiesToSpawn[i] != "")
            {
                for (int j = 0; j < enemyPrefabs.Length; j++)
                {
                    if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                    {
                        BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                        newEnemy.transform.parent = enemyPositions[i];
                        activeBattlers.Add(newEnemy);
                    }
                }
            }
        }
    }

    private void AddPlayers()
    {
        for (int i = 0; i < playerPositions.Length; i++)
        {
            if (GameManager.instance.playersStats[i].gameObject.activeInHierarchy)
            {
                for (int j = 0; j < playerPrefabs.Length; j++)
                {
                    if (playerPrefabs[j].charName == GameManager.instance.playersStats[i].charName)
                    {
                        BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                        newPlayer.transform.parent = playerPositions[i];
                        activeBattlers.Add(newPlayer);
                        UpdateStats(activeBattlers[i], GameManager.instance.playersStats[i]);
                    }
                }
            }

        }
    }

    public void UpdateStats(BattleChar battleChar, CharStats charStats)
    {
        battleChar.currentHp = charStats.currentHp;
        battleChar.maxHp = charStats.maxHp;
        battleChar.currentMp = charStats.currentMp;
        battleChar.maxMp = charStats.maxMp;
        battleChar.strength = charStats.strength;
        battleChar.defense = charStats.defense;
        battleChar.weaponPower = charStats.weaponPower;
        battleChar.armorPower = charStats.armorPower;
    }

    public void UpdateEndBattleStats(CharStats charStats, BattleChar battleChar)
    {
        charStats.currentHp = battleChar.currentHp;
        charStats.maxHp = battleChar.maxHp;
        charStats.currentMp = battleChar.currentMp;
        charStats.maxMp = battleChar.maxMp;
        charStats.strength = battleChar.strength;
        charStats.defense = battleChar.defense;
        charStats.weaponPower = battleChar.weaponPower;
        charStats.armorPower = battleChar.armorPower;
    }

    public void NextTurn()
    {
        currentTurn++;
        if (currentTurn >= activeBattlers.Count)
        {
            currentTurn = 0;
        }
        turnWaiting = true;
        UpdateBattle();
    }

    public void UpdateBattle()
    {
        var players = activeBattlers.Where(a => a.isPlayer).ToList(); 
        var deadPlayers = players.Where(p => p.currentHp <= 0).ToList();
        deadPlayers.ForEach(d => d.sprite.sprite = d.deadSprite);
        players.Where(p => p.currentHp > 0).ToList().ForEach(a => a.sprite.sprite = a.livingSprite);
        bool isAllPlayersDead = players.Count == deadPlayers.Count;


        var enemies = activeBattlers.Where(a => a.isPlayer == false).ToList();
        var deadEnemies = enemies.Where(p => p.currentHp <= 0).ToList();
        deadEnemies.ForEach(d => {
            d.EnemyFade();
            d.isDead = true;
        });
        bool isAllEnemiesDead = enemies.Count == deadEnemies.Count;


        if (isAllEnemiesDead || isAllPlayersDead)
        {
            if (isAllEnemiesDead)
            {
                //end battle in victory
                StartCoroutine(EndBattleCo());
            } else
            {
                //end battle in defeat
                StartCoroutine(GameOverCo());
            }
        } else
        {
            while (activeBattlers[currentTurn].currentHp == 0)
            {
                currentTurn++;
                if(currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
        UpdateUIStats();
    }

    public void EnemyAttack()
    {
        List<BattleChar> players = activeBattlers.Where(a => a.isPlayer && a.currentHp > 0).ToList();
        BattleChar defendingChar = players[Random.Range(0, players.Count)];
        //character attack
        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        BattleMove battleMove = movesList.Where(m => m.moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]).FirstOrDefault();
        Instantiate(battleMove.theEffect, defendingChar.transform.position, defendingChar.transform.rotation);
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
        DealDamage(defendingChar, battleMove.movePower);
    }

    public void DealDamage(BattleChar defendingChar, int movePower)
    {
        float attackPower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPower;
        float defendPower = defendingChar.defense + defendingChar.armorPower;
        if(defendPower == 0)
        {
            defendPower = 1;
        }
        int damage = Mathf.RoundToInt(attackPower / defendPower * movePower * Random.Range(.9f, 1.1f));
        Debug.Log($"{activeBattlers[currentTurn].charName} is dealing {damage} damage to {defendingChar.charName}");
        defendingChar.currentHp -= damage;
        if (defendingChar.currentHp < 0)
        {
            defendingChar.currentHp = 0;
        }
        Instantiate(damageNumber, defendingChar.transform.position, defendingChar.transform.rotation).SetDamage(damage);
        UpdateUIStats();
    }

     public void UpdateUIStats()
    {
        int index = 0;
        playerName.ForEach(p => UpdateLabels(index++));
    }

    private void UpdateLabels(int index)
    {
        if (activeBattlers.Count > index && activeBattlers[index].isPlayer)
        {
            BattleChar playerChar = activeBattlers[index];
            playerName[index].gameObject.SetActive(true);
            playerName[index].text = playerChar.charName;
            playerHp[index].text = $"{playerChar.currentHp.ToString()}/{playerChar.maxHp}";
            playerMp[index].text = $"{playerChar.currentMp.ToString()}/{playerChar.maxMp}";

        } else
        {
            playerName[index].gameObject.SetActive(false);
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        List<BattleChar> enemies = activeBattlers.Where(a => a.isPlayer == false).ToList();
        BattleChar defendingEnemy = enemies[selectedTarget];
        BattleMove battleMove = movesList.Where(m => m.moveName == moveName).FirstOrDefault();
        Instantiate(battleMove.theEffect, defendingEnemy.transform.position, defendingEnemy.transform.rotation);

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
        DealDamage(defendingEnemy, battleMove.movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);
        var enemies = activeBattlers.Where(a => a.isPlayer == false).ToList();
        targetButtons.ForEach(t => t.gameObject.SetActive(false));
        int index = 0;
        enemies.ForEach(e => {
            if(e.isDead == false)
            {
                targetButtons[index].gameObject.SetActive(true);
                targetButtons[index].moveName = moveName;
                targetButtons[index].selectedTarget = index;
                targetButtons[index].targetName.text = e.charName;
            }
            index++;
        });
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);
        for (int i = 0; i < magicButtons.Count; i++)
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i)
            {
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;
                var move = movesList.Where(m => m.moveName == magicButtons[i].spellName).FirstOrDefault();
                magicButtons[i].costText.text = move.moveCost.ToString();
                magicButtons[i].spellCost = move.moveCost;
            } else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenItemMenu()
    {
        uiButtonsHolder.SetActive(false);
        statsMenu.SetActive(false);
        Shop.instance.OpenItemMenu();
    }

    public void Flee()
    {
        fleeing = true;
        int fleeSuccess = Random.Range(0, 100);
        if (fleeSuccess < chanceToFlee)
        {
            StartCoroutine(EndBattleCo());
        } else
        {
            NextTurn();
            battleNote.theText.text = "Couldn't Escape!";
            battleNote.Activate();
        }
    }

    public IEnumerator EndBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        Shop.instance.itemMenu.SetActive(false);
        magicMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);

        EndBattleUpdateChars();

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;

        if (fleeing)
        {
            GameManager.instance.battleActive = false;
        } else
        {
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardItems);
        }
        AudioManager.instance.PlayMusic(FindObjectOfType<CameraController>().musicToPlay);
    }

    public void EndBattleUpdateChars()
    {
        activeBattlers.Where(a=> a.isPlayer).ToList().ForEach(b => {
            var player = GameManager.instance.playersStats.Where(p => p.charName == b.charName).FirstOrDefault();
            UpdateEndBattleStats(player, b);
        });
        activeBattlers.ForEach(a => Destroy(a.gameObject));
    }

    public IEnumerator GameOverCo()
    {
        battleActive = false;
        battleScene.SetActive(false);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(gameOverScene);
    }
}
