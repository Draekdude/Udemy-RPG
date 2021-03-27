using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    public string currentTransitionName;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.instance != null)
        {
            if (currentTransitionName == PlayerController.instance.areaTransitionName)
            {
                PlayerController.instance.transform.position = transform.position;
            }
        }
        UIFade.instance.FadeFromBlack();
        GameManager.instance.fadingBetweenAreas = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
