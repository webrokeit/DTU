using System;
using Assets.Scripts.Kinect;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GUIManager : MonoBehaviour {
    public BrainScript Brain;

    public GUIText GameOverText, ScoreText, RunningScoreText, RunningRoundText, RoundTimeText, EndGameText, RoundCountdownText;
    private List<Vector3> _movePositions;
    private List<Gestures> moves;

    public PrefabCol Prefabs;

	// Use this for initialization
	void Start () {
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        GameEventManager.RoundStart += RoundStart;
        RunningRoundText.enabled = false;
        RunningScoreText.enabled = false;
        ScoreText.enabled = false;
        GameOverText.enabled = false;
        RoundTimeText.enabled = false;
        EndGameText.enabled = false;
        RoundCountdownText.enabled = false;
        _movePositions = new List<Vector3>();
        moves = new List<Gestures>();
	}
	
	// Update is called once per frame
	void Update () {
        RunningScoreText.text = "Score: "+Brain.Score;
        RunningRoundText.text = "Round "+Brain.Round;
        RoundTimeText.text = "Time left: " + Brain.GetTimeLeft();
        RoundCountdownText.text = Brain.GetCountdownLeft();
	}

    private void GameStart()
    {
        GameOverText.enabled = false;
        ScoreText.enabled = false;
        RunningScoreText.text = "Score: " + Brain.Score;
        RunningRoundText.text = "Round: " + Brain.Round;
        RunningRoundText.enabled = true;
        RunningScoreText.enabled = true;
        moves = new List<Gestures>();
        _movePositions = new List<Vector3>();
        EndGameText.text = "";
    }

    private void GameOver()
    {
        RunningRoundText.enabled = false;
        RunningScoreText.enabled = false;
        RoundTimeText.enabled = false;
        GameOverText.enabled = true;
        ScoreText.enabled = true;
        EndGameText.enabled = true;
        ScoreText.text = "You achieved a score of "+Brain.Score+" points";
    }

    void StartRound() {
        GameEventManager.StartRound();
    }

    public void EndGameReason(string text)
    {
        EndGameText.text = text;
    }

    public void SetNextMove(Gestures move)
    {
        moves.Add(move);
        var pos = GenerateVector();
        while (_movePositions.Contains(pos))
        {
            pos = GenerateVector();
        }
        _movePositions.Add(pos);

        RoundTimeText.enabled = false;
        DisplayNewRound();
    }

    void DisplayNewRound() {
        var roundPrefabObj = Instantiate(Prefabs.Round, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (roundPrefabObj == null || (fadeoutObj = roundPrefabObj.GetComponent<FadeoutText>()) == null) {
            Debug.LogError("Prefabs.Round does not have a FadeoutText object attached");
            return;
        }
        
        roundPrefabObj.guiText.text = "Round " + Brain.Round;
        fadeoutObj.TextFaded += (script, obj) => DisplayMoves();
    }

    private void DisplayMoves(int index = 0) {
        if (index < 0) index = 0;
        if (index >= moves.Count) {
            StartRound();
            return;
        }

        var movePrefabObj = Instantiate(Prefabs.MoveToPerform, _movePositions[index], Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (movePrefabObj == null || (fadeoutObj = movePrefabObj.GetComponent<FadeoutText>()) == null) {
            Debug.LogError("Prefabs.MoveToPerform does not have a FadeoutText object attached");
            return;
        }

        movePrefabObj.guiText.text = moves[index].ToString();
        fadeoutObj.TextFaded += (script, obj) => DisplayMoves(index + 1);
    }

    private static Vector3 GenerateVector() {
        return new Vector3(Random.Range(0.2f, 0.75f), Random.Range(0.2f, 0.75f), 0);
    }

    public void RemoveCountdown()
    {
        RoundCountdownText.enabled = false;
        RoundTimeText.enabled = true;
    }

    public void GainedPoints(int points, int moveIndex) {
        var startPosition = _movePositions[moveIndex];
        var pointsGainedObj = Instantiate(Prefabs.PointsScored, startPosition, Quaternion.identity) as GameObject;
        if (pointsGainedObj == null || pointsGainedObj.GetComponent<GUIText>() == null) {
            Debug.LogError("Prefabs.PointsScored does not have a GUIText object attached");
            return;
        }

        var movePrefabObj = Instantiate(Prefabs.MoveCorrect, _movePositions[moveIndex], Quaternion.identity) as GameObject;
        if (movePrefabObj == null || movePrefabObj.GetComponent<GUIText>() == null) {
            Debug.LogError("Prefabs.MoveCorrect does not have a GUIText object attached");
            return;
        }

        pointsGainedObj.guiText.text = "+" + points;
        pointsGainedObj.guiText.pixelOffset = new Vector2(0, 15);

        movePrefabObj.guiText.text = moves[moveIndex].ToString();
    }

    private void RoundStart()
    {
        RoundCountdownText.enabled = true;
    }

    [Serializable]
    public struct PrefabCol {
        public GameObject Round;
        public GameObject MoveToPerform;
        public GameObject MoveCorrect;
        public GameObject MoveWrong;
        public GameObject PointsScored;
    }
}
