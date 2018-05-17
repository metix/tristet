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
        GetComponent<Canvas>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(float score, float best)
    {
        Score.text = score.ToString().Replace(".", ",");
        Best.text = best.ToString().Replace(".", ",");

        GetComponent<Canvas>().enabled = true;
        afterEnable();
    }

    public void afterEnable() {
        EventSystem.current.SetSelectedGameObject(Reset.gameObject);
    }

    public void CallReset()
    {
        Debug.Log("call-reset");
        gameloop.GetComponent<GameFieldScript>().RestartGame();
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
