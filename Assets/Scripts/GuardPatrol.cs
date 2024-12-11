using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuardPatrolState
{
    Patrolling,
    Chasing
}

public class GuardPatrol : MonoBehaviour
{
    [SerializeField] private GuardPatrolState state;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform waypointGroup;
    private Transform[] waypoints; // Liste des waypoints

    private Transform currentWaypoint;
    private int currentWaypointIndex = 0; // Index actuel

    private float moveSpeed = 5f;
    [SerializeField] private float chaseSpeed = 12f;

    private Vector2 previousPosition;

    //joueur visibilité
    private bool playerInvisible = false;
    [SerializeField] private PlayerControl player;

    private Collider2D playerCollider;

    void Awake()
    {
        // Récupère tous les waypoints dans un tableau
        int count = waypointGroup.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointGroup.GetChild(i);
        }

        // Initialise le premier waypoint
        currentWaypoint = waypoints[currentWaypointIndex];
        previousPosition = transform.position;
        playerCollider = player.GetComponent<Collider2D>();
    }

    void Update()
    {
        switch (state)
        {
            case GuardPatrolState.Patrolling:
                Patrol();
                if (CheckPlayerVisibility() && !player.IsPlayerHiding())
                {
                    Debug.Log("gweny : Chasing guard: on");
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
                    // Retourne à la patrouille en reprenant le dernier waypoint
                    currentWaypoint = waypoints[currentWaypointIndex];
                    Patrol();
                }
                break;
        }
    }

    // Gère le patrouillage du garde
    void Patrol()
    {
        MoveToWaypoint();

        // Vérifie si le garde est proche du waypoint actuel
        if (Vector2.Distance(transform.position, currentWaypoint.position) < 0.2f)
        {
            // Passe au waypoint suivant dans l'ordre
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            currentWaypoint = waypoints[currentWaypointIndex];
        }

        // Met à jour l'orientation du sprite
        float direction = currentWaypoint.position.x - transform.position.x;
        FlipSprite(direction);
    }

    // Déplace le garde vers le waypoint actuel
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
            // Passe au waypoint suivant dans l'ordre
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            currentWaypoint = waypoints[currentWaypointIndex];
        }
    }

    // Vérifie si le joueur est visible par le garde
    bool CheckPlayerVisibility()
    {
        // Si le joueur est invisible, ne le détecte pas
        if (playerInvisible) return false;

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

        // direction dans laquelle le raycast va être lancé (le garde regarde dans cette direction)
        Vector3 raycastDirection = directionToCompare.normalized;
        Vector3 start = transform.position + (raycastDirection * 1.8f);

        RaycastHit2D hit = Physics2D.Raycast(start, raycastDirection, 12f);
        Debug.DrawRay(start, raycastDirection * 12, Color.green);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log($"gweny : {hit.collider.name}");
            if(Vector2.Angle(directionToCompare, raycastDirection) < 45)
            {
                return true;
            }
        }
        return false;
    }

    // Permet de définir si le joueur est invisible ou non
    public void SetPlayerInvisible(bool isInvisible)
    {
        playerInvisible = isInvisible;
        // Debug.Log($"gweny : Player invisibility set to {isInvisible}");
    }

    // Gère l'orientation du sprite
    void FlipSprite(float direction)
    {
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x; // Inverse l'échelle en X
            transform.localScale = localScale;
        }
    }
}