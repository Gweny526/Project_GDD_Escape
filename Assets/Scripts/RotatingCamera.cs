using UnityEngine;

public class RotatingCamera : MonoBehaviour
{
    // Variables pour le mouvement de rotation
    public float rotationAngle = 45f; // Angle maximal de rotation de gauche à droite
    public float rotationSpeed = 5f; // Vitesse de la rotation

    // LayerMask pour vérifier uniquement certains objets
    public LayerMask layerMask;
    public Transform player; // Référence au joueur

    private Vector3 playerInitialPosition;
    private float currentAngle = 0f; // Angle actuel
    private bool rotatingUp = false; // Direction de rotation

    void Awake()
    {
        //save Player initial position
        if (player != null)
        {
            playerInitialPosition = player.position;
        }
        currentAngle = -Mathf.Abs(rotationAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    void Update()
    {
        // Gérer la rotation
        RotateCamera();

        // Lancer un Raycast depuis la position de l'objet dans la direction du regard
        CheckPlayerVisibility();
    }

    void RotateCamera()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;

        // Appliquer la rotation selon la direction actuelle
        if (rotatingUp)
        {
            currentAngle += rotationStep;
        }
        else
        {
            currentAngle -= rotationStep;
        }

        // Vérifier si les limites sont atteintes
        if (currentAngle >= 0)
        {
            rotatingUp = false;

        } // Limite supérieure
        else if (currentAngle <= -Mathf.Abs(rotationAngle))
        {
            rotatingUp = true;

        } // Limite inférieure

        // Appliquer la rotation (Z)
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    void CheckPlayerVisibility()
    {
       
        Vector3 directionToCompare;
        if (transform.localScale.x > 0)
        {
            directionToCompare = transform.right;
        }
        else
        {

            directionToCompare = -transform.right;
        }
        Vector3 raycastDirection = directionToCompare.normalized;

        Vector3 start = transform.position + (raycastDirection * 0.4f);

        RaycastHit2D hit = Physics2D.Raycast(start, raycastDirection, 8f);

        Debug.DrawRay(start, raycastDirection * 8, Color.green);

        if (hit.collider != null)
        {
            if (hit.collider.transform == player)
            {
                Debug.Log("Player détecté par la fausse caméra !");
                Debug.Log(hit.collider.name);
                ResetPlayerPosition();

            }
            else
            {
                Debug.Log($"Un autre objet est détecté. {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("Aucun objet détecté.");
        }
    }
    void ResetPlayerPosition()
    {
        if (player != null)
        {
            player.position = playerInitialPosition;
            Debug.Log("Gweny :Le Joueur a été renvoyer à sa position initial");
        }
    }
}
