  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour
{
    public bool isPlayer;
    public string[] movesAvailable;

    public string charName;
    public int currentHp, maxHp, currentMp, maxMp, strength, defense, weaponPower, armorPower;
    public bool isDead;

    public SpriteRenderer sprite;
    public Sprite deadSprite;
    public Sprite livingSprite;

    
    public float fadeSpeed = 1f;


    private bool shouldFade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFade)
        {
            float r = Mathf.MoveTowards(sprite.color.r, 1, fadeSpeed * Time.deltaTime);
            float g = Mathf.MoveTowards(sprite.color.g, 0, fadeSpeed * Time.deltaTime);
            float b = Mathf.MoveTowards(sprite.color.b, 0, fadeSpeed * Time.deltaTime);
            float a = Mathf.MoveTowards(sprite.color.a, 0, fadeSpeed * Time.deltaTime);
            sprite.color = new Color(r, g, b, a);
            if (sprite.color.a == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void EnemyFade()
    {
        shouldFade = true;
    }
}
