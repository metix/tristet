using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour {

    public bool feet;
    public bool head;
    public bool forward;
    public bool backward;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if((feet && head) || (forward && backward))
        {
            Destroy(gameObject);
            //Game over
        }
	}
}
