using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamescript : MonoBehaviour {

    public GameObject[] blocks;
    public GameObject currentblock;
    public float torque;
    public float force;
    public GameObject Playercamera;
    public GameObject player;

    public float spawnTime;
    float spawntimer;
    bool spawn = true;

    public Text gameOverText;

    float maxCameraHeight;
    Plane[] frustumPlanes;

    // Use this for initialization
    void Start () {
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Playercamera.GetComponent<Camera>());
        maxCameraHeight = Playercamera.transform.position.y;
        //Physics.gravity = new Vector3(0, -0.6f, 0);
        spawntimer = 0;
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
        spawntimer -= Time.deltaTime;
        if(spawntimer <=0 && spawn)
        {
            currentblock =  spawnBlock(blocks[Random.Range(0,7)], new Vector3(0,maxCameraHeight + 7, 0), Quaternion.identity);
            spawntimer = spawnTime;
        }


        if (player.transform.position.y + 4 > maxCameraHeight)
        {
            maxCameraHeight = player.transform.position.y + 4;
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Playercamera.GetComponent<Camera>());
        }
        Playercamera.transform.position = new Vector3(Playercamera.transform.position.x, maxCameraHeight, Playercamera.transform.position.z);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, player.GetComponent<Collider>().bounds))
            gameOver();
    }

    public GameObject spawnBlock(GameObject block, Vector3 transform,Quaternion quaternion)
    {
        block = Instantiate(block,transform,quaternion);
        block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        return block;
    }

    void gameOver()
    {
        spawn = false;
        gameOverText.text = "YOU LOOSE";
    }
}
