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

    bool wasSlaped;
    bool wasSquashed;

	void Update () {
   

        if ((feet && head) || (forward && backward))
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            push = true;
            //Game over
        }
        if(push)
        {
            if (wasSlaped)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * force * 3);

            }

            if (wasSquashed)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * force * 3);
            }

            if (!pushCalled)
            {
                wasSlaped = feet && head;
                wasSquashed = forward && backward;

                Debug.Log("player died");
                if (feet && head)
                {
                    Debug.Log("you are slaped by a long block");
                    gameObject.GetComponent<AudioSource>().Play();
                }

                if (forward && backward)
                {
                    Debug.Log("you are squashed");
                }
                pushCalled = true;
            }


        }
    }
}
