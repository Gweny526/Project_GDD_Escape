using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Wall : MonoBehaviour
{
    public bool leftWall = true;
    
    private BoxCollider2D box;
    void Start(){
        box = GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D other){
        
        if(other.gameObject.CompareTag("Player")){
            Vector3 pos = other.transform.position;
            if(leftWall){
                pos.x = box.bounds.max.x;
            }
            else{
                pos.x = box.bounds.min.x;
            }
            other.transform.position = pos;
        }
    }

}
