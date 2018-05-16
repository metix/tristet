using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour {

    public bool feet;
    public bool head;
    public bool forward;
    public bool backward;

    public float force;

    private bool push = false;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private bool pushCalled = false;

	void Update () {
		if((feet && head) || (forward && backward))
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            push = true;
            //Game over
        }
        if(push)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * force);

            if (!pushCalled)
            {
                Debug.Log("play audio");
                if (feet & head)
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
                pushCalled = true;
            }


        }
    }
}
