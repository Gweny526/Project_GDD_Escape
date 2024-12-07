using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    public InputActionAsset actions;
    public float speed = 1f;
    //move keys
    private InputAction xAxis;
    //hide key
    private InputAction useKey;
    //view key
    private InputAction viewKey;
    public Rigidbody2D rb;

    [SerializeField]private bool rightIsBlocked = false;
    [SerializeField]private bool leftIsBlocked = false;

    //ref du "guard"
    public GameObject guard;
    //ref du player
    [SerializeField] private GameObject player;
    private Collider2D playerCollider;


    //position initiale
    private Vector2 initialPlayerPosition;
    private Vector2 initialGuardPosition;

    //est-ce que le player est caché
    private bool isHiding = false;

    //zone de cachette 
    private bool isInHidingSpot = false;

    //virtual camera
    private CinemachineVirtualCamera activeCamera;
    
    public TMP_Text textPressE;



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

        if(textPressE != null)
        {
            textPressE.enabled = false;
        }
    }

    void OnEnable()
    {
        actions.FindActionMap("PlayerMove").Enable();
        useKey.performed += PlayerIsHiding; //lance le listener
    }
    void OnDisable()
    {
        actions.FindActionMap("PlayerMove").Disable();
        useKey.performed -= PlayerIsHiding; //desactive le listener
        
    }
    

    
    void Update()
    {
        MoveX();
        
        Debug.Log($"gweny : Update: isHiding = {isHiding}, isInHidingSpot = {isInHidingSpot}");

        
    }
    void MoveX()
    {
        float xMove = xAxis.ReadValue<float>();
        if(xMove < 0 && leftIsBlocked) xMove = 0f;
        if(xMove > 0 && rightIsBlocked) xMove = 0f;

        transform.position += speed * Time.deltaTime * xMove * transform.right;
        
    }
    public void ViewChangeCamera(CinemachineVirtualCamera cameraToActivate)
    {
        float xMove = xAxis.ReadValue<float>();
        if(xMove < 0 && Input.GetButtonDown("Look"))
        {
            
        }
    }
    public bool IsPlayerHiding()
    {
        return isHiding;
    }

    public void PlayerIsHiding(InputAction.CallbackContext context)
    {
        
        //verifie si useKey a été appuyer et si je suis dans mon hiding spot
        if (isInHidingSpot)
        {
            isHiding = !isHiding;

            if (isHiding)
            {
                
                guard.GetComponent<GuardPatrol>().SetPlayerInvisible(true);
                playerCollider.isTrigger = true;
            }
            else
            {
                Debug.Log(" gweny : Player is no longer hiding");
                guard.GetComponent<GuardPatrol>().SetPlayerInvisible(false);
                playerCollider.isTrigger = false;
            }
        }
        else
        {
            Debug.Log("gweny : Cannot hide, player is not in a hiding spot!");
        }    
    }

    void OnCollisionEnter2D(Collision2D collision){
        Debug.Log($"gweny : Collision Entered with: {collision.transform.name}, Tag: {collision.transform.tag}");

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
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("HidingSpot"))
        {
            isInHidingSpot = true;
            if(textPressE != null)
            {
                textPressE.enabled = true;
            }
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
            if (!isHiding)
            {
                Debug.Log("Player is still hiding, ignoring exit trigger");
                return; // Ne pas forcer l'état si le joueur est encore caché
            }

            Debug.Log("Player has exited the hiding spot");
            isInHidingSpot = false;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("HidingSpot"))
        {
            
            isHiding = false;
            isInHidingSpot = false;
            if(textPressE != null)
            {
                textPressE.enabled = false;
            }
            
            guard.GetComponent<GuardPatrol>().SetPlayerInvisible(false);
            playerCollider.isTrigger = false;
            Debug.Log($"gweny : Triggered player as now exited the hiding spot {other.name}, isInHidingSpot = {isInHidingSpot}");

        }

    }
    
}
