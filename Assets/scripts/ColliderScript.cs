using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour {

    public bool head;
    public bool feet;
    public bool forward;
    public bool backward;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(head)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().head = true;
        if(feet)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().feet = true;
        if (forward)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().forward = true;
        if (backward)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().backward = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (head)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().head = false;
        if (feet)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().feet = false;
        if (forward)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().forward = false;
        if (backward)
            gameObject.transform.parent.GetComponent<PlayerCollisionScript>().backward = false;
    }
}
