using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {

    public void UpdatePosition(Vector3 v, float speed)
    {
       iTween.MoveTo(gameObject, v, speed);
       
    }
}
