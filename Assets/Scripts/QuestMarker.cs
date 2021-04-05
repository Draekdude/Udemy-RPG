using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarker : MonoBehaviour
{

    public string questToMark;
    public bool markComplete;
    public bool markOnEnter;
    public bool deactivateOnMarking;

    private bool canMark;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canMark)
        {
            canMark = false;
            MarkQuest();
        }
    }

    public void MarkQuest()
    {
        QuestManager.instance.MarkQuest(questToMark, markComplete);
        gameObject.SetActive(!deactivateOnMarking);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (markOnEnter)
            {
                MarkQuest();
            } else
            {
                canMark = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canMark = false;
        }
    }
}
