using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public List<BattleMove> movesList;

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
                uiButtonsHolder.SetActive(isPlayer);
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
        int deadPlayers = players.Where(p => p.currentHp <= 0).Count();
        bool isAllPlayersDead = players.Where(p => p.currentHp <= 0).Count() == deadPlayers && deadPlayers != 0;
        var enemies = activeBattlers.Where(a => a.isPlayer == false).ToList();
        int deadEnemies = enemies.Where(p => p.currentHp <= 0).Count();
        bool isAllEnemiesDead = enemies.Where(p => p.currentHp <= 0).Count() == deadEnemies && deadEnemies != 0;


        if (isAllEnemiesDead || isAllPlayersDead)
        {
            if (isAllEnemiesDead)
            {
                //end battle in victory
            } else
            {
                //end battle in defeat
            }
            battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false;
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()
    {
        List<BattleChar> players = activeBattlers.Where(a => a.isPlayer && a.currentHp > 0).ToList();
        BattleChar defendingChar = players[Random.Range(0, players.Count)];
        //character attack
        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        BattleMove battleMove = movesList.Where(m => m.moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]).FirstOrDefault();
        Instantiate(battleMove.theEffect, defendingChar.transform.position, defendingChar.transform.rotation);
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
    }
}
