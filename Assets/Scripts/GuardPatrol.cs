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
    [SerializeField] private GuardPatrolState state; 
    public Transform playerTransform;
    
    public Transform waypointGroup;

    private Transform currentWaypoint;
    private float moveSpeed = 3f; // Vitesse de déplacement du guard
    private float chaseSpeed = 10f; // Vitesse lorsqu'il poursuit le joueur

    

    void Start()
    {
        currentWaypoint = SelectDestination();
    }

    void Update()
    {
        switch(state)
        {
            case GuardPatrolState.Patrolling:
                Patrol();
                if (CheckPlayerVisibility())
                {
                    state = GuardPatrolState.Chasing;
                }
                break;
            case GuardPatrolState.Chasing:
                ChasePlayer();
                if (!CheckPlayerVisibility())
                {
                    state = GuardPatrolState.Patrolling;
                    // L'IA retourne au waypoint
                    currentWaypoint = SelectDestination();
                }
                break;
        }
    }

    // Gère le patrouillage de l'IA
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

    // Déplace l'IA vers le waypoint
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
    // Cette fonction inverse l'échelle sur l'axe X pour retourner le personnage


    // Vérifie si le joueur est visible par l'IA
    bool CheckPlayerVisibility()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction = direction.normalized;

        Vector3 start = transform.position + (direction * 0.8f);

        RaycastHit2D hit = Physics2D.Raycast(start, direction, 10f);

        Debug.DrawRay(start, direction * 10, Color.red);
        if(hit.collider != null) Debug.Log($"+++ {hit.collider.name} +++");
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Debug.Log($"[GuardPatrolState] Raycast hit: {hit.collider.name}");
            Debug.Log("is it working!!");
            if (Vector3.Angle(transform.forward, playerTransform.position - transform.position) < 45)
            {
                return true;
            }
        }
        return false;
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
