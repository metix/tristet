using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    public Text ScoreTitle;
    public Text Score;
    public Text BestTitle;
    public Text Best;
    public GameObject gameloop;
    public Button Reset;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void afterEnable() {
        EventSystem.current.SetSelectedGameObject(Reset.gameObject);
    }
    public void CallReset()
    {
        
        gameloop.GetComponent<GameFieldScript>().RestartGame();
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
