using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

public class GUIManager : MonoBehaviour {
    public BrainScript Brain;

    public GUIText GameOverText, ScoreText, RunningScoreText, RunningRoundText, NextMoveText, PointsGainedText, RoundTimeText, EndGameText, RoundCountdownText;
    private Stopwatch _newRoundTimer;
    private Stopwatch _pointsGainedTimer;
    private Stopwatch _nextMoveTimer;
    private List<Vector3> _possiblePointsPositions = new List<Vector3>() {new Vector3(0.75f,0.25f,0), new Vector3(0.25f,0.75f,0), new Vector3(0.50f,0.75f,0), new Vector3(0.50f,0.25f,0), new Vector3(0.75f,0.50f,0)};
    private System.Random rnd;
    private static GUIManager instance;
    private List<Vector3> _movePositions;
    private List<string> moves;
    private int _index;
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
        NextMoveText.enabled = false;
        PointsGainedText.enabled = false;
        RoundTimeText.enabled = false;
        EndGameText.enabled = false;
        RoundCountdownText.enabled = false;
        _newRoundTimer = new Stopwatch();
        _nextMoveTimer = new Stopwatch();
        _pointsGainedTimer = new Stopwatch();
        rnd = new System.Random();
        _movePositions = new List<Vector3>();
        moves = new List<string>();
        _index = 0;
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
        if (_pointsGainedTimer.IsRunning && _pointsGainedTimer.ElapsedMilliseconds > 1000)
        {
            PointsGainedText.enabled = false;
            _pointsGainedTimer.Reset();
        }

        if (_index == (moves.Count-1) && _nextMoveTimer.IsRunning && _nextMoveTimer.ElapsedMilliseconds > _moveDisplayTime)
        {
            NextMoveText.text = moves[_index];
            NextMoveText.transform.position = _movePositions[_index];
            _index++;
            _nextMoveTimer.Reset();
            _nextMoveTimer.Start();
        } else if(_index <= (moves.Count - 1) && _nextMoveTimer.IsRunning && _nextMoveTimer.ElapsedMilliseconds > _moveDisplayTime) {
            NextMoveText.text = moves[_index];
            NextMoveText.transform.position = _movePositions[_index];
            _index++;
            _nextMoveTimer.Reset();
            _nextMoveTimer.Start();
        }
        else if (_index == moves.Count && _nextMoveTimer.IsRunning && _nextMoveTimer.ElapsedMilliseconds > _finalMoveDisplayTime)
        {
            GameEventManager.StartRound();
            _index = 0;
            NextMoveText.enabled = false;
            NextMoveText.text = "";
            _nextMoveTimer.Reset();
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

    public void EndGameReason(string text)
    {
        EndGameText.text = text;
    }

    public void SetNextMove(string move)
    {
        moves.Add(move);
        Vector3 pos = GenerateVector();
        while (_movePositions.Contains(pos))
        {
            pos = GenerateVector();
        }
        _movePositions.Add(pos);
        NextMoveText.text = moves[_index];
        NextMoveText.transform.position = _movePositions[_index];
        NextMoveText.enabled = true;
        RoundTimeText.enabled = false;
        RunningRoundText.transform.position = new Vector3(0.5f, 0.60f, 0);
        RunningRoundText.fontSize = 30;
        RunningRoundText.anchor = TextAnchor.MiddleCenter;
        RunningRoundText.alignment = TextAlignment.Center;
        _newRoundTimer.Start();
        _nextMoveTimer.Start();
    }

    private Vector3 GenerateVector() {
        float randomX = (float) rnd.NextDouble();
        float randomY = (float)rnd.NextDouble();
        while (randomX < 0.20f || randomX > 0.75f)
        {
            randomX = (float)rnd.NextDouble();
        }

        while (randomY < 0.20f || randomY > 0.75f)
        {
            randomY = (float)rnd.NextDouble();
        }
        return new Vector3(randomX, randomY, 0);
    }

    public void RemoveCountdown()
    {
        RoundCountdownText.enabled = false;
        RoundTimeText.enabled = true;
    }

    public void GainedPoints(int points)
    {
        int pickRandomPosition = rnd.Next(0,_possiblePointsPositions.Count);
        PointsGainedText.transform.position = _possiblePointsPositions[pickRandomPosition];
        PointsGainedText.text = "+"+points;
        PointsGainedText.enabled = true;
        if (_pointsGainedTimer.IsRunning)
        {
            _pointsGainedTimer.Reset();
            _pointsGainedTimer.Start();
        }
        else
        {
            _pointsGainedTimer.Start();
        }
    }

    private void RoundStart()
    {
        RoundCountdownText.enabled = true;
    }
}
