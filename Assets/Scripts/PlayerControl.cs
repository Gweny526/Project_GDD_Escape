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
    private InputAction useKey;
    public Rigidbody2D rb;

    [SerializeField]private bool rightIsBlocked = false;
    [SerializeField]private bool leftIsBlocked = false;

    //ref du "guard"
    public GameObject guard;
    [SerializeField] private GameObject player;
    private Collider2D playerCollider;


    //position initiale
    private Vector2 initialPlayerPosition;
    private Vector2 initialGuardPosition;

    //est-ce que le player est caché
    private bool isHiding = false;

    //zone de cachette 
    private bool isInHidingSpot = false;


    void Awake()
    {
        xAxis = actions.FindActionMap("PlayerMove").FindAction("XAxis");
        useKey = actions.FindActionMap("PlayerMove").FindAction("Hide");
        rb = GetComponent<Rigidbody2D>();

        //save initial position
        initialPlayerPosition = transform.position;
        if(guard != null)
        {
            initialGuardPosition = guard.transform.position;
        }
        playerCollider = player.GetComponent<Collider2D>();
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
        PlayerIsHiding();
    }
    void MoveX()
    {
        float xMove = xAxis.ReadValue<float>();
        if(xMove < 0 && leftIsBlocked) xMove = 0f;
        if(xMove > 0 && rightIsBlocked) xMove = 0f;

        transform.position += speed * Time.deltaTime * xMove * transform.right;
        
    }

    void PlayerIsHiding()
    {
        //si la touche E est pressée et si le player est caché
        if(useKey.triggered)Debug.Log("Key E is pressed");
        if (useKey.triggered && !isInHidingSpot)
        {
            isHiding = !isHiding; //alterné d'un état a l'autre
            if(isHiding)
            {
                Debug.Log("Key E pressed And Player is now hiding");
                Debug.Log("Player is now hiding!");
                guard.GetComponent<GuardPatrol>().SetPlayerInvisible(true);
                playerCollider.isTrigger = false;
                // playerCollider.enabled = true;
            } // Informer le garde
        }
        else
        {
            Debug.Log("Player is no longer hiding!");
            guard.GetComponent<GuardPatrol>().SetPlayerInvisible(false); // Informer le garde
            playerCollider.isTrigger = true;
                 
                         
        }
    }

    void OnCollisionEnter2D(Collision2D collision){

        if(collision.transform.CompareTag("InvisibleWall")){
        
            if(collision.transform.position.x < transform.position.x){
                leftIsBlocked = true;
            }
            else{
                rightIsBlocked = true;
            }
        }
        if(collision.transform.CompareTag("Guard") && !isHiding){
            transform.position = initialPlayerPosition;
            if (guard != null){
                guard.transform.position = initialGuardPosition;
            }
        }
        if(collision.transform.CompareTag("HidingSpot"))
        {
            Debug.Log("player is in hiding spot");
            isInHidingSpot = true;
            

        }
    }

    
    void OnCollisionExit2D(Collision2D collision){
        if(collision.transform.CompareTag("InvisibleWall"))
        {
            leftIsBlocked = false;

            rightIsBlocked = false;

        }
        if(collision.transform.CompareTag("HidingSpot"))
        {
            isInHidingSpot = false;

        }
    }
    
}
