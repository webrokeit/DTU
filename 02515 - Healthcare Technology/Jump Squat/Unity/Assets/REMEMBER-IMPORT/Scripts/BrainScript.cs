﻿using System;
using System.Globalization;
using Assets.Scripts.Kinect;
using Assets.Scripts.Kinect.Gesture;
using Assets.Scripts.Lib;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class BrainScript : MonoBehaviour {
    public int Round { get; private set; }
    public int Score { get; private set; }
    private readonly Gestures[] _possibleMoves = {Gestures.Jump, Gestures.Squat};
    private List<Gestures> _correctCombi = new List<Gestures>();
    private List<Gestures> _userCombi = new List<Gestures>();
    private bool _timeout;
    private bool _wrongCombi;
    private const int TimePerExercise = 5000;
    private Stopwatch _exerciseTime;

    public GUIManager GuiManager;
    private Stopwatch _roundCountdown;
    public GestureTracker GestureTracker;
    public GestureInputter GestureInputter;

	// Use this for initialization
	void Start () {
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        GameEventManager.RoundStart += RoundStart;
        _exerciseTime = new Stopwatch();
        _timeout = false;
        _wrongCombi = false;
        _roundCountdown = new Stopwatch();

	    if (GestureTracker) {
	        GestureTracker.GestureDetected += (id, gesture) => AddUserCombination(gesture.Gesture);
	    }
	    if (GestureInputter) {
	        GestureInputter.GestureInput += AddUserCombination;
	    }
	}

    public string GetCountdownLeft() {
        return _roundCountdown.ElapsedMilliseconds >= 3000 ? "GO!" : (3 - Math.Floor(_roundCountdown.ElapsedMilliseconds / 1000.0)).ToString(CultureInfo.InvariantCulture);
    }

    public long GetTimeLeft() {
        return _exerciseTime != null ? Math.Max(5000 - _exerciseTime.ElapsedMilliseconds, 0) : 0;
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GameEventManager.StartGame();
        }

        if (_roundCountdown.IsRunning && _roundCountdown.ElapsedMilliseconds > 3750) {
            if (_roundCountdown.ElapsedMilliseconds < 3000) return;
            if(!_exerciseTime.IsRunning) _exerciseTime.Start();
            if (_roundCountdown.ElapsedMilliseconds > 3750) {
                _roundCountdown.Reset();
                GuiManager.RemoveCountdown();
            }
        }

        if (_exerciseTime.IsRunning && _exerciseTime.ElapsedMilliseconds > TimePerExercise) {
            _timeout = true;
            GameEventManager.EndGame();
        }
	}

    private void UpdateScore(long time) {
        var secondsFactor = ((TimePerExercise * Round) - time) / 1000f;
        var totalScore = 10 * (1.0 + secondsFactor);
        Score += (int)totalScore;
        GuiManager.GainedPoints((int) totalScore, _userCombi.Count - 1);
    }

    private void NewRound() {
        var move = _possibleMoves[UnityEngine.Random.Range(0, _possibleMoves.Length)];
        _correctCombi.Add(move);
        Round++;
        GuiManager.SetNextMove(move);
        _userCombi = new List<Gestures>();
    }

    private void AddUserCombination(Gestures gesture) {
        if (!_possibleMoves.Contains(gesture)) return;

        var index = _userCombi.Count;
        if (_correctCombi[index] != gesture) {
            _wrongCombi = true;
            GameEventManager.EndGame();
            return;
        }

        var timeSpent = _exerciseTime.ElapsedMilliseconds;
        _exerciseTime.Reset();
        _userCombi.Add(gesture);
        UpdateScore(timeSpent);
        
        if (_userCombi.Count == _correctCombi.Count) {
            NewRound();
            return;
        }

        _exerciseTime.Start();
    }

    private void RoundStart() {
        _roundCountdown.Start();
    }

    private void GameStart() {
        _correctCombi = new List<Gestures>();
        Score = 0;
        Round = 0;
        NewRound();
    }

    private void GameOver() {
        if (_timeout) {
            GuiManager.EndGameReason("Time ran out!");
            _timeout = false;
        }
        if (_wrongCombi) {
            GuiManager.EndGameReason("You made the wrong gesture!");
            _wrongCombi = false;
        }
        _exerciseTime.Reset();
        _roundCountdown.Reset();
        Round = 0;
    }
}
