using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
     
	}

    public void UpdatePosition(Vector3 v)
    {
        iTween.MoveTo(gameObject, v, 4f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
