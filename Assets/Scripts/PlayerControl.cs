using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    public InputActionAsset actions;
    public float speed = 8f;
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
    public GuardPatrol[] guards;
    //ref du player
    [SerializeField] private GameObject player;
    
    private Collider2D playerCollider;


    //position initiale
    private Vector2 initialPlayerPosition;

    
    public bool isHiding = false;

    //zone de cachette 
    private bool isInHidingSpot = false;

    //virtual camera
    private CinemachineVirtualCamera activeCamera;
    
    public TextManager textManager;
    private Animator animator;
    private HidingSpot.HidingSpotType currentHidingSpotType;

    private bool isCrawling = false;
    private bool isCrawlingMoving = false;  




    void Awake()
    {
        xAxis = actions.FindActionMap("PlayerMove").FindAction("XAxis");
        useKey = actions.FindActionMap("PlayerMove").FindAction("Hide");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        

        //save initial position
        initialPlayerPosition = transform.position;
        foreach(GuardPatrol guard in guards)
        {
            if(guard != null)
            {
                initialPlayerPosition = player.transform.position; 
            }
        }
        playerCollider = player.GetComponent<Collider2D>();
        

        isHiding = false;
        isInHidingSpot = false; 

    //     if(textPressE != null)
    //     {
    //         textPressE.enabled = false;
    //     }
    }
    void Start()
    {
        guards = FindObjectsOfType<GuardPatrol>();
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
        animator.SetBool("isMoving", xMove != 0f);
        FlipSprite(xMove);
        
    }
    void FlipSprite(float direction)
    {
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x; // Flip the sprite horizontally
            transform.localScale = localScale;
        }
    }
    public void ViewChangeCamera(CinemachineVirtualCamera cameraToActivate)
    {
        float xMove = xAxis.ReadValue<float>();
        if(xMove < 0 && Input.GetButtonDown("Look"))
        {
            
        }
    }

    public void PlayerIsHiding(InputAction.CallbackContext context)
    {
        
        //verifie si useKey a été appuyer et si je suis dans mon hiding spot
        if (isInHidingSpot)
        {
            isHiding = !isHiding;

            if (isHiding)
            {
                if(currentHidingSpotType == HidingSpot.HidingSpotType.Bed)
                {
                    playerCollider.isTrigger = true;
                    // Active l'animation de rampage
                    animator.SetBool("IsHidingUnderBed", true);
                    isCrawling = true;
                    isCrawlingMoving = false;

                }
                else if(currentHidingSpotType == HidingSpot.HidingSpotType.Door)
                {
                    playerCollider.isTrigger = true;

                    player.GetComponent<SpriteRenderer>().enabled = false;      
                    // Aucune animation spécifique
                    animator.SetBool("IsHidingUnderBed", false);
                    isCrawling = false;
                    isCrawlingMoving = false;               
                }
            }
            else
            {

                playerCollider.isTrigger = false;
                player.GetComponent<SpriteRenderer> ().enabled = true;
                animator.SetBool("IsHidingUnderBed", false);
                isCrawling = false;
                isCrawlingMoving = false;                
                

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
            foreach(GuardPatrol guard in guards)
            {
                if (guard != null)
                {
                    initialPlayerPosition = player.transform.position; 

                }

            }
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("HidingSpot"))
        {
            isInHidingSpot = true;
            HidingSpot hidingSpot = other.GetComponent<HidingSpot>();
            if(hidingSpot != null)
            {
                currentHidingSpotType= hidingSpot.hidingType;
                HidingSpotText hidingSpotText = other.GetComponent<HidingSpotText>();
                if (textManager != null)
                {
                    // Active les textes selon les spécifications du HidingSpot
                    textManager.ShowText(hidingSpotText.activateFirstText, hidingSpotText.activateSecondText, hidingSpotText.activateThirdText, hidingSpotText.activateFourthText);
                }
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
            currentHidingSpotType = default; // Réinitialise le type de cachette

            //Désactive l'animation 
            animator.SetBool("IsHidingUnderBed", false);               
            isCrawling = false;
            isCrawlingMoving = false;

            player.GetComponent<SpriteRenderer> ().enabled = true;

            if (textManager != null)
            {
                textManager.ShowText(false, false, false, false); // Désactiver tous les textes
            }
            

            playerCollider.isTrigger = false;
            Debug.Log($"gweny : Triggered player as now exited the hiding spot {other.name}, isInHidingSpot = {isInHidingSpot}");

        }

    }
    
}
