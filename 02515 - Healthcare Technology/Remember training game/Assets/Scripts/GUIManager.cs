using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GUIManager : MonoBehaviour {
    public BrainScript Brain;

    public GUIText GameOverText, ScoreText, RunningScoreText, RunningRoundText, RoundTimeText, EndGameText, RoundCountdownText;
    public GameObject PointsScoredPrefab;
    public GameObject MoveToPerformPrefab;
    private Stopwatch _newRoundTimer;
    private static GUIManager instance;
    private List<Vector3> _movePositions;
    private List<string> moves;
    private readonly long _moveDisplayTime = 1500;
    private readonly long _finalMoveDisplayTime = 3000;

	// Use this for initialization
	void Start () {
        instance = this;
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
        _newRoundTimer = new Stopwatch();
        _movePositions = new List<Vector3>();
        moves = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
        RunningScoreText.text = "Score: "+Brain.Score;
        RunningRoundText.text = "Round: "+Brain.Round;
        RoundTimeText.text = "Time left: " + Brain.GetTimeLeft();
        RoundCountdownText.text = Brain.GetCountdownLeft();
        if (_newRoundTimer.IsRunning && _newRoundTimer.ElapsedMilliseconds > 1000)
        {
            RunningRoundText.transform.position = new Vector3(0, 1, 0);
            RunningRoundText.fontSize = 20;
            RunningRoundText.anchor = TextAnchor.UpperLeft;
            RunningRoundText.alignment = TextAlignment.Left;
            _newRoundTimer.Reset();
        }
	}

    private void GameStart()
    {
        GameOverText.enabled = false;
        ScoreText.enabled = false;
        RunningScoreText.text = "Score: " + Brain.Score;
        RunningRoundText.text = "Round: " + Brain.Round;
        RunningRoundText.enabled = true;
        RunningScoreText.enabled = true;
        moves = new List<string>();
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

    public void SetNextMove(string move)
    {
        moves.Add(move);
        var pos = GenerateVector();
        while (_movePositions.Contains(pos))
        {
            pos = GenerateVector();
        }
        _movePositions.Add(pos);
        RoundTimeText.enabled = false;
        RunningRoundText.transform.position = new Vector3(0.5f, 0.60f, 0);
        RunningRoundText.fontSize = 30;
        RunningRoundText.anchor = TextAnchor.MiddleCenter;
        RunningRoundText.alignment = TextAlignment.Center;
        _newRoundTimer.Start();
        DisplayMoves();
    }

    private void DisplayMoves(int index = 0) {
        if (index < 0) index = 0;
        if (index >= moves.Count) {
            StartRound();
            return;
        }

        var movePrefabObj = Instantiate(MoveToPerformPrefab, _movePositions[index], Quaternion.identity) as GameObject;
        FadeoutText fadeoutObj;
        if (movePrefabObj == null || (fadeoutObj = movePrefabObj.GetComponent<FadeoutText>()) == null) {
            Debug.LogError("MoveToPerformPrefab does not have a FadeoutText object attached");
            return;
        }

        movePrefabObj.guiText.text = moves[index];
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
        var pointsGainedObj = Instantiate(PointsScoredPrefab, startPosition, Quaternion.identity) as GameObject;
        if (pointsGainedObj == null || pointsGainedObj.GetComponent<GUIText>() == null) {
            Debug.LogError("PointsScoredPrefab does not have a GUIText object attached");
            return;
        }

        pointsGainedObj.guiText.text = "+" + points;
        pointsGainedObj.guiText.pixelOffset = new Vector2(0, 15);
    }

    private void RoundStart()
    {
        RoundCountdownText.enabled = true;
    }
}
