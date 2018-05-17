using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour {

    public GameObject blockPrefab;

	// Use this for initialization
	void Start () {
        var number = DigitType.InstantiateNumber(1234567890, new Vector3(0, 0, 0), blockPrefab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
