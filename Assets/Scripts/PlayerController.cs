using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rigidbodyLink;
    [SerializeField] public float moveSpeed;
    [SerializeField] public Animator animator;
    [SerializeField] public float jumpHeight;

    public static PlayerController instance;

    public string areaTransitionName;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            Movement();
        }
        else
        {
            StopMovement();
        }
        
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight)
    {
        bottomLeftLimit = bottomLeft + new Vector3(0.5f, 0.65f, 0);
        topRightLimit = topRight + new Vector3(-0.5f, -0.65f, 0);
    }

    private void Movement()
    {
        rigidbodyLink.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed;
        WalkingAnimation(rigidbodyLink.velocity.x, rigidbodyLink.velocity.y);
        DirectionalAnimation(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);

        //if (Input.GetKeyDown("space"))
        //{
        //    //transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.MoveTowards(transform.position.z, jumpHeight, Time.deltaTime));
        //    //transform.position = new Vector3(transform.position.x, transform.position.y, jumpHeight);
        //}
    }

    private void WalkingAnimation(float xVelocity, float hVelocity)
    {
        animator.SetFloat("moveX", xVelocity);
        animator.SetFloat("moveY", hVelocity);
    }

    private void DirectionalAnimation(float horizonal, float vertical)
    {
        if (horizonal == 1 || horizonal == -1 || vertical == 1 || vertical == -1)
        {
            animator.SetFloat("lastMoveX", horizonal);
            animator.SetFloat("lastMoveY", vertical);
        }
    }

    private void StopMovement()
    {
        rigidbodyLink.velocity = Vector3.zero;
        WalkingAnimation(0, 0);
        DirectionalAnimation(0, -1);
    }
}
