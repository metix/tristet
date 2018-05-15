using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
     
	}

    public void UpdatePosition(Vector3 v, float speed)
    {
        iTween.MoveTo(gameObject, v, speed);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
