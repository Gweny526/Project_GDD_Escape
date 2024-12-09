using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum GuardPatrolState
{
    Patrolling,
    Chasing
}
public class GuardPatrol : MonoBehaviour
{
    [SerializeField]private GuardPatrolState state; 
    [SerializeField]private Transform playerTransform;
    
    [SerializeField]private Transform waypointGroup;

    private Transform currentWaypoint;
    private float moveSpeed = 3f; // Vitesse de déplacement du guard
    [SerializeField]private float chaseSpeed = 10f; // Vitesse lorsqu'il poursuit le joueur

    private Vector2 previousPosition;
    
    //variable player visibility
    private bool playerInvisible = false;
    [SerializeField] private PlayerControl player;
    
    private Collider2D playerCollider;
    

    void Start()
    {
        currentWaypoint = SelectDestination();
        previousPosition = transform.position;
        playerCollider = player.GetComponent<Collider2D>();

    }

    void Update()
    {
        switch(state)
        {
            case GuardPatrolState.Patrolling:
                Patrol();
                if (CheckPlayerVisibility() && !player.IsPlayerHiding())
                {

                    Debug.Log("gweny :  Chasing guard: on");
                    state = GuardPatrolState.Chasing;
                    ChasePlayer();
                }
                break;
            case GuardPatrolState.Chasing:
                ChasePlayer();
                if (!CheckPlayerVisibility())
                {
                    Debug.Log("Player not visible, continuing patrol");
                    state = GuardPatrolState.Patrolling;
                    // L'IA retourne au waypoint
                    currentWaypoint = SelectDestination();
                    Patrol();
                }
                break;
        }
    }

    // Gère le patrouillage du garde
    void Patrol()
    {
        MoveToWaypoint();
        if (Vector2.Distance(transform.position, currentWaypoint.position) < 0.2f)
        {
            currentWaypoint = SelectDestination();
        }
        float direction = currentWaypoint.position.x - transform.position.x;
        FlipSprite(direction);
        
    }

    // Déplace le garde vers le waypoint
    void MoveToWaypoint()
    {
        Vector2 direction = (currentWaypoint.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
    }

    // Gère le comportement de poursuite du joueur
    void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);

        float moveDirection = playerTransform.position.x - transform.position.x;
        FlipSprite(moveDirection);
        

    }

    // Détection de collision avec un waypoint
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == currentWaypoint)
        {
            currentWaypoint = SelectDestination();
        }
    }

    // Sélectionne un nouveau waypoint aléatoire
    Transform SelectDestination()
    {
        int index = Random.Range(0, waypointGroup.childCount);
        Transform newWaypoint = waypointGroup.GetChild(index);
        
        // S'assure de ne pas choisir le même waypoint qu'actuellement
        while(newWaypoint == currentWaypoint)
        {
            index = Random.Range(0, waypointGroup.childCount);
            newWaypoint = waypointGroup.GetChild(index);
        }
        return newWaypoint;
    }
    
    // Vérifie si le joueur est visible par le garde
    bool CheckPlayerVisibility()
    {
        //si le player est invisible ne le détecte pas
        if(playerInvisible)return false;
        // Calcul de la direction dans laquelle le garde regarde
        Vector3 directionToCompare;
        if (transform.localScale.x > 0) 
        {
            // Garde regarde à droite
            directionToCompare = transform.right;
        }
        else
        {
            // Garde regarde à gauche
            directionToCompare = -transform.right;
        }
         // Détermine la direction dans laquelle le raycast va être lancé (le garde regarde dans cette direction)
        Vector3 raycastDirection = directionToCompare.normalized;
        

        Vector3 start = transform.position + (raycastDirection * 3f);
        
        RaycastHit2D hit = Physics2D.Raycast(start, raycastDirection, 10f);

        Debug.DrawRay(start, raycastDirection * 10, Color.red);
        
        if(hit.collider != null) Debug.Log($"+++ {hit.collider.name} +++");
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if(Vector2.Angle(directionToCompare, raycastDirection) < 45)
            {
                return true;
            }
        }
        return false;
    
    }
    
    //permet de définir si le player est invisible ou non 
    public void SetPlayerInvisible(bool isInvisible)
    {
        playerInvisible = isInvisible;
        // if(isInvisible)
        // {
        //     playerCollider.enabled = false;
        // }
        // else
        // {
        //     playerCollider.enabled = true;
        // }

    }
    
    void FlipSprite(float direction)
    {
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;  // Inverser l'échelle en X
            transform.localScale = localScale;
            
        }
    }
}
