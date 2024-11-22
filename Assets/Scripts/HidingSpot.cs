// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(BoxCollider2D))]
// public class HidingSpot : MonoBehaviour
// {
//     public bool isInHidingSpot = true;

//     private BoxCollider2D bed;
//     // Start is called before the first frame update
//     void Start()
//     {
//         bed = GetComponent<BoxCollider2D>();
//     }

//     void OnCollisionEnter2D(Collider2D other)
//     {
//         if(other.gameObject.CompareTag("Player"))
//         {
//             Vector3 player = other.transform.position;
//             if(isInHidingSpot)
//             {

//             }
//         }
//     }
// }
