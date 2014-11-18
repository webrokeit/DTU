using System;
using System.Globalization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class BrainScript : MonoBehaviour {

    public int Round;
    public int Score;
    private readonly string[] _possibleMoves = {"Squat", "Jump"};
    private List<string> _correctCombi;
    private List<string> _userCombi;
    private bool timeout;
    private bool wrongCombi;
    private const int TimePerExercise = 5000;
    private Stopwatch exerciseTime;
    private bool inputReady;
    public GUIManager GUIManager;
    private Stopwatch roundCountdown;

	// Use this for initialization
	void Start () {
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        GameEventManager.RoundStart += RoundStart;
        Score = 0;
        Round = 0;
        _correctCombi = new List<string>();
        exerciseTime = new Stopwatch();
        timeout = false;
        wrongCombi = false;
        roundCountdown = new Stopwatch();
	}

    public string GetCountdownLeft() {
        return roundCountdown.ElapsedMilliseconds >= 3000 ? "GO!" : (3 - Math.Floor(roundCountdown.ElapsedMilliseconds / 1000.0)).ToString(CultureInfo.InvariantCulture);
    }

    public long GetTimeLeft() {
        return exerciseTime != null ? Math.Max(5000*Round - exerciseTime.ElapsedMilliseconds, 0) : 0;
    }

	// Update is called once per frame
	void Update () {
        var up = Input.GetKeyDown(KeyCode.UpArrow);
        var down = Input.GetKeyDown(KeyCode.DownArrow);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEventManager.StartGame();
        }

        if (roundCountdown.IsRunning && roundCountdown.ElapsedMilliseconds <= 3000)
        {
            return;
        }

        if (roundCountdown.IsRunning && roundCountdown.ElapsedMilliseconds > 3750) {
            if (roundCountdown.ElapsedMilliseconds < 3000) return;
            if(!exerciseTime.IsRunning) exerciseTime.Start();
            if (roundCountdown.ElapsedMilliseconds > 3750) {
                roundCountdown.Reset();
                GUIManager.RemoveCountdown();
            }
        }

        if (exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds > TimePerExercise)
        {
            timeout = true;
            GameEventManager.EndGame();
        }

        if (up && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= TimePerExercise)
        {
            var time = exerciseTime.ElapsedMilliseconds;
            AddUserCombination(_possibleMoves[1]);
            exerciseTime.Reset();
            if (WrongCombination())
            {
                wrongCombi = true;
                GameEventManager.EndGame();
                return;
            }
            UpdateScore(time);
            if (_correctCombi.Count == _userCombi.Count)
            {
                NewRound();
                return;
            }
            exerciseTime.Start();
        }
        if (down && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= TimePerExercise)
        {
            var time = exerciseTime.ElapsedMilliseconds;
            AddUserCombination(_possibleMoves[0]);
            exerciseTime.Reset();
            if (WrongCombination())
            {
                wrongCombi = true;
                GameEventManager.EndGame();
                return;
            }
            UpdateScore(time);
            if (_correctCombi.Count == _userCombi.Count)
            {
                NewRound();
                return;
            }
            exerciseTime.Start();
        }
	}

    private bool WrongCombination()
    {
        for (var i = 0; i < _userCombi.Count; i++)
        {
            if(!_correctCombi[i].Equals(_userCombi[i])) {
                return true;
            }
        }
        return false;
    }

    private void UpdateScore(long time) {
        var secondsFactor = ((TimePerExercise * Round) - time) / 1000f;
        var totalScore = 10 * (1.0 + secondsFactor);
        Score += (int)totalScore;
        GUIManager.GainedPoints((int) totalScore, _userCombi.Count - 1);
    }

    private void NewRound() {
        var move = _possibleMoves[UnityEngine.Random.Range(0, _possibleMoves.Length)];
        _correctCombi.Add(move);
        Round++;
        GUIManager.SetNextMove(move);
        _userCombi = new List<string>();
    }

    private void AddUserCombination(string move)
    {
        if (_possibleMoves.Contains(move))
        {
            _userCombi.Add(move);
        }
    }

    private void RoundStart()
    {
        roundCountdown.Start();
    }

    private void GameStart()
    {
        _correctCombi = new List<string>();
        Score = 0;
        Round = 0;
        NewRound();
    }

    private void GameOver()
    {
        if (timeout)
        {
            GUIManager.EndGameReason("Time ran out!");
            timeout = false;
        }
        if (wrongCombi)
        {
            GUIManager.EndGameReason("You made the wrong move!");
            wrongCombi = false;
        }
        exerciseTime.Reset();
        roundCountdown.Reset();
        Round = 0;
    }
}
