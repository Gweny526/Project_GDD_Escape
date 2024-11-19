using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public const string Layer_Name = "GameLayer1";
    public int sortingOrder = 0;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
      sprite = GetComponent<SpriteRenderer>();
      if(sprite)
      {
        sprite.sortingOrder = sortingOrder;
        sprite.sortingLayerName = Layer_Name;
      }   
    }
}
