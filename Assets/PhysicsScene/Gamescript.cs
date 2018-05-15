using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamescript : MonoBehaviour {

    public GameObject[] blocks;
    public GameObject currentblock;
    public float torque;
    public float force;
    public GameObject camera;
    public GameObject player;

    public Text gameOverText;

    float maxCameraHeight;
    Plane[] frustumPlanes;

    // Use this for initialization
    void Start () {
        currentblock.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera.GetComponent<Camera>());
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetKey(KeyCode.W))
        {
            currentblock.GetComponent<Rigidbody>().AddTorque(transform.forward * torque);
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentblock.GetComponent<Rigidbody>().AddTorque(transform.forward * torque * -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            currentblock.GetComponent<Rigidbody>().AddForce(new Vector3(-1, 0, 0) * force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentblock.GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 0) * force);
        }
    }

    void Update()
    {
        if (player.transform.position.y + 4 > maxCameraHeight)
        {
            maxCameraHeight = player.transform.position.y + 4;
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera.GetComponent<Camera>());
        }
        camera.transform.position = new Vector3(0, maxCameraHeight,-10);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, player.GetComponent<Collider>().bounds))
            gameOver();
    }

    public void spawnBlock(GameObject block, Vector3 transform,Quaternion quaternion)
    {
        block = Instantiate(block,transform,quaternion);
        block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    void gameOver()
    {
        gameOverText.text = "YOU LOOSE";
    }
}
