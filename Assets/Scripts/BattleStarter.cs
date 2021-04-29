using System.Security;
using System.Timers;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public List<BattleType> potentialBattles;
    public bool activateOnEnter, activateOnStay, activateOnExit;
    public float timeBetweenBattles = 10f;
    public bool deactivateAfterStarting;
    public bool cannotFlee;
    public bool shouldCompleteQuest;
    public string questToComplete;

    private float betweenBattleCounter;
    private bool inArea;
    // Start is called before the first frame update
    void Start()
    {
        betweenBattleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(inArea && PlayerController.instance.canMove){
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
                betweenBattleCounter -= Time.deltaTime;
            }
            if(betweenBattleCounter <= 0){
                betweenBattleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
                StartCoroutine(StartBattleCo());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "Player"){
            if(activateOnEnter){
                StartCoroutine(StartBattleCo()); 
            } else {
                inArea = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if(collider.tag == "Player"){
            if(activateOnExit){
                StartCoroutine(StartBattleCo()); 
            } else {
                inArea = false;
            }
            
        }
    }

    public IEnumerator StartBattleCo(){
        UIFade.instance.FadeToBlack();
        GameManager.instance.battleActive = true;

        int selectedBattle = Random.Range(0, potentialBattles.Count);
        BattleManager.instance.rewardItems = potentialBattles[selectedBattle].rewardItems;
        BattleManager.instance.rewardXP = potentialBattles[selectedBattle].rewardXp;

        yield return new WaitForSeconds(1.5f);

        BattleManager.instance.BattleStart(potentialBattles[selectedBattle].enemies.ToArray(), cannotFlee);
        UIFade.instance.FadeFromBlack();  
        if (deactivateAfterStarting)
        {
            gameObject.SetActive(false);
        }
        BattleReward.instance.markQuestComplete = shouldCompleteQuest;
        BattleReward.instance.questToMark = questToComplete;
    }
}
