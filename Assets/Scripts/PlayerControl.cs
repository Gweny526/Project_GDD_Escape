using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    public InputActionAsset actions;
    public float speed = 1f;
    private InputAction xAxis;
    private InputAction duck;
    public Rigidbody2D rb;

    [SerializeField]private bool rightIsBlocked = false;
    [SerializeField]private bool leftIsBlocked = false;


    void Awake()
    {
        xAxis = actions.FindActionMap("PlayerMove").FindAction("XAxis");
        duck = actions.FindActionMap("PlayerMove").FindAction("Duck");
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        actions.FindActionMap("PlayerMove").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("PlayerMove").Disable();
        
    }
    

    // Update is called once per frame
    void Update()
    {
        MoveX();
    }
    void MoveX()
    {
        float xMove = xAxis.ReadValue<float>();
        if(xMove < 0 && leftIsBlocked) xMove = 0f;
        if(xMove > 0 && rightIsBlocked) xMove = 0f;

        transform.position += speed * Time.deltaTime * xMove * transform.right;
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        collision.transform.CompareTag("InvisibleWall");
        if(collision.transform.position.x < transform.position.x){
            leftIsBlocked = true;
        }
        else{
            rightIsBlocked = true;
        }
    }

    
    void OnCollisionExit2D(Collision2D collision){
        collision.transform.CompareTag("InvisibleWall");

        leftIsBlocked = false;

        rightIsBlocked = false;

    }
}
