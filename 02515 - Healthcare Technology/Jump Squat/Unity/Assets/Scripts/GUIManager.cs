using System;
using Assets.Scripts.Kinect;
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GUIManager : MonoBehaviour {
    public BrainScript Brain;

    public GUIText GameOverText, ScoreText, RunningScoreText, RunningRoundText, RoundTimeText, EndGameText, RoundCountdownText;
    private List<Vector3> _movePositions;
    private List<Gestures> _moves;
    private FadeoutText _roundPointsGained;
    private FadeoutText _roundCompleted;
    private bool _playerCheck;

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
        _playerCheck = false;
        _movePositions = new List<Vector3>();
        _moves = new List<Gestures>();
	}
	
	// Update is called once per frame
	void Update () {
        RunningScoreText.text = "Score: "+Brain.Score;
        RunningRoundText.text = "Round "+Brain.Round;
        RoundTimeText.text = "Time left: " + Brain.GetTimeLeft();
        RoundCountdownText.text = Brain.GetCountdownLeft();
        if (Brain.IsPlayerMissing() && _playerCheck)
        {
            _roundCompleted.enabled = false;
            _roundPointsGained.enabled = false;
            DisplayMissingPlayer();
        }
	}

    private void GameStart()
    {
        GameOverText.enabled = false;
        ScoreText.enabled = false;
        RunningScoreText.text = "Score: " + Brain.Score;
        RunningRoundText.text = "Round " + Brain.Round;
        RunningRoundText.enabled = true;
        RunningScoreText.enabled = true;
        _moves = new List<Gestures>();
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
        _moves.Add(move);
        var pos = GenerateVector();
        while (_movePositions.Contains(pos))
        {
            pos = GenerateVector();
        }
        _movePositions.Add(pos);

        RoundTimeText.enabled = false;
        
        if (Brain.Round == 1) {
            DisplayNewRound();
        }
        else {
            DisplayLastRoundResults();
        }
    }

    void DisplayNewRound() {
        _playerCheck = false;
        var roundPrefabObj = Instantiate(Prefabs.Round, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (roundPrefabObj == null || (fadeoutObj = roundPrefabObj.GetComponent<FadeoutText>()) == null) {
            Debug.LogError("Prefabs.Round does not have a FadeoutText object attached");
            return;
        }
        
        roundPrefabObj.guiText.text = "Round " + Brain.Round;
        fadeoutObj.TextFaded += (script, obj) => DisplayMoves();
    }

    void DisplayLastRoundResults()
    {
        var roundCompletedPrefabObj = Instantiate(Prefabs.RoundCompleted, new Vector3(0.5f, 0.60f, 0), Quaternion.identity) as GameObject;
        var roundPointsGainedPrefabObj = Instantiate(Prefabs.RoundPointsGained, new Vector3(0.5f, 0.4f, 0), Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (roundCompletedPrefabObj == null || (fadeoutObj = roundCompletedPrefabObj.GetComponent<FadeoutText>()) == null)
        {
            Debug.LogError("Prefabs.RoundCompleted does not have a FadeoutText object attached");
            return;
        }
        FadeoutText roundPointsfadeoutObj;
        if (roundPointsGainedPrefabObj == null || (roundPointsfadeoutObj = roundPointsGainedPrefabObj.GetComponent<FadeoutText>()) == null)
        {
            Debug.LogError("Prefabs.RoundPointsGained does not have a FadeoutText object attached");
            return;
        }
        roundCompletedPrefabObj.guiText.text = "Round " + (Brain.Round - 1) + " completed";
        roundPointsGainedPrefabObj.guiText.text = "You have gained " + Brain.RoundScore + " points this round";
        fadeoutObj.TextFaded += (script, obj) => DisplayNewRound();
        _roundPointsGained = roundPointsfadeoutObj;
        _roundCompleted = fadeoutObj;
        if (Brain.IsPlayerMissing())
        {
            fadeoutObj.enabled = false;
            roundPointsfadeoutObj.enabled = false;
            DisplayMissingPlayer();
        }
        _playerCheck = true;
    }

    void PlayerIsBack()
    {
        _roundCompleted.enabled = true;
        _roundPointsGained.enabled = true;
    }

    void DisplayMissingPlayer()
    {
        _playerCheck = false;
        var missingPlayerPrefabObj = Instantiate(Prefabs.MissingPlayer, new Vector3(0.5f, 0.75f, 0), Quaternion.identity) as GameObject;

        FadeoutText fadeoutObj;
        if (missingPlayerPrefabObj == null || (fadeoutObj = missingPlayerPrefabObj.GetComponent<FadeoutText>()) == null)
        {
            Debug.LogError("Prefabs.MissingPlayer does not have a FadeoutText object attached");
            return;
        }

        missingPlayerPrefabObj.guiText.text = "MISSING PLAYER";
        if (Brain.IsPlayerMissing())
        {
            fadeoutObj.TextFaded += (script, obj) => DisplayMissingPlayer();
        }
        else {
            Destroy(fadeoutObj.gameObject);
            PlayerIsBack();
        }
    }

    private void DisplayMoves(int index = 0) {
        if (index < 0) index = 0;
        if (index >= _moves.Count) {
            StartRound();
            return;
        }

        var movePrefabObj = Instantiate(Prefabs.MoveToPerform, _movePositions[index], Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (movePrefabObj == null || (fadeoutObj = movePrefabObj.GetComponent<FadeoutText>()) == null) {
            Debug.LogError("Prefabs.MoveToPerform does not have a FadeoutText object attached");
            return;
        }

        movePrefabObj.guiText.text = _moves[index].ToString();
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

        movePrefabObj.guiText.text = _moves[moveIndex].ToString();
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
        public GameObject RoundCompleted;
        public GameObject RoundPointsGained;
        public GameObject MissingPlayer;
    }
}
