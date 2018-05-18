using System.Collections;
using System.Collections.Generic;
using Invector.CharacterController;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartPanelScript : MonoBehaviour {

    public GameObject global;

	// Use this for initialization
	void Start () {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void close()
    {
        GetComponent<Canvas>().enabled = false;
        global.GetComponent<GameFieldScript>().StartGame();
    }
}
