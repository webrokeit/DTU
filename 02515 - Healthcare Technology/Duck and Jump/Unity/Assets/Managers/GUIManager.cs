using Assets.Managers;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class GUIManager : MonoBehaviour {

    public GUIText GameOverText, ScoreText;
    public DangerousObjectManagerScript Doms;

	// Use this for initialization
	void Start () {
	    GameEventManager.GameStart += GameStart;
	    GameEventManager.GameOver += GameOver;
	    GameOverText.enabled = false;
	    ScoreText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void GameStart() {
        GameOverText.enabled = false;
        ScoreText.text = "";
        ScoreText.enabled = false;
    }

    private void GameOver() {
        GameOverText.enabled = true;
        ScoreText.text = "You have avoided "+Doms.ObjectsPassed+"objects";
    }
}
