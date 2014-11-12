using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GUIManager : MonoBehaviour {
    public BrainScript Brain;

    public GUIText GameOverText, ScoreText, RunningScoreText, RunningRoundText, NextMoveText;

    private static GUIManager instance;

	// Use this for initialization
	void Start () {
        instance = this;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        RunningRoundText.enabled = false;
        RunningScoreText.enabled = false;
        ScoreText.enabled = false;
        GameOverText.enabled = false;
        NextMoveText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        RunningScoreText.text = "Score: "+Brain.Score;
        RunningRoundText.text = "Round: "+Brain.Round;
	}

    private void GameStart()
    {
        GameOverText.enabled = false;
        ScoreText.enabled = false;
        RunningScoreText.text = "Score: " + Brain.Score;
        RunningRoundText.text = "Round: " + Brain.Round;
        RunningRoundText.enabled = true;
        RunningScoreText.enabled = true;
    }

    private void GameOver()
    {
        RunningRoundText.enabled = false;
        RunningScoreText.enabled = false;
        GameOverText.enabled = true;
        ScoreText.enabled = true;
        ScoreText.text = "You achieved a score of "+Brain.Score+" points";
    }

    public static void SetNextMove(List<string> moves)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < moves.Count; i++)
        {
            if (i == 0)
            {
                 sb.Append(moves[i]);
            }
            else
            {
                if ((i % 8) == 0)
                {
                    sb.Append("\n" + moves[i]);
                }
                else
                {
                    sb.Append(", " + moves[i]);
                }
            }
        }
        instance.NextMoveText.text = sb.ToString();
        instance.NextMoveText.enabled = true;
    }

    public static void RemoveNextMove()
    {
        instance.NextMoveText.text = "";
        instance.NextMoveText.enabled = false;
    }
}
