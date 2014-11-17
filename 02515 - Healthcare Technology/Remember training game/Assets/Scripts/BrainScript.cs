using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class BrainScript : MonoBehaviour {

    public int Round;
    public int Score;
    private readonly List<string> _possibleMoves = new List<string>() {"Squat", "Jump"};
    private List<string> _correctCombi;
    private List<string> _userCombi;
    private bool timeout;
    private bool wrongCombi;
    private readonly int timePerExercise = 5000;
    private Stopwatch exerciseTime;
    private bool inputReady;
    private System.Random rand;
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
        rand = new System.Random();
        timeout = false;
        wrongCombi = false;
        roundCountdown = new Stopwatch();
	}

    public string GetCountdownLeft()
    {
        if (roundCountdown.ElapsedMilliseconds < 1000)
        {
            return "3";
        }
        else if (roundCountdown.ElapsedMilliseconds < 2000)
        {
            return "2";
        }
        else if (roundCountdown.ElapsedMilliseconds < 3000)
        {
            return "1";
        }
        else
        {
            return "GO!";
        }
    }

    public long GetTimeLeft()
    {
        return (5000 - exerciseTime.ElapsedMilliseconds < 0) ? 0 : (5000 - exerciseTime.ElapsedMilliseconds);
    }

	// Update is called once per frame
	void Update () {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEventManager.StartGame();
        }

        if (roundCountdown.IsRunning && roundCountdown.ElapsedMilliseconds <= 3000)
        {
            return;
        }

        if (roundCountdown.IsRunning && roundCountdown.ElapsedMilliseconds > 3000)
        {
            roundCountdown.Reset();
            GUIManager.RemoveCountdown();
            exerciseTime.Start();
        }

        if (exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds > timePerExercise)
        {
            timeout = true;
            GameEventManager.EndGame();
        }

        if (up && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= timePerExercise)
        {
            long time = exerciseTime.ElapsedMilliseconds;
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
        if (down && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= timePerExercise)
        {
            long time = exerciseTime.ElapsedMilliseconds;
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
        for (int i = 0; i < _userCombi.Count; i++)
        {
            if(!_correctCombi[i].Equals(_userCombi[i])) {
                return true;
            }
        }
        return false;
    }

    private void UpdateScore(long time) {
        var secondsFactor = ((timePerExercise * Round) - time) / 1000f;
        var totalScore = 10 * (1.0 + secondsFactor);
        Score += (int)totalScore;
        GUIManager.GainedPoints((int)totalScore);
    }

    private void NewRound()
    {
        int pickRandom = rand.Next(0, 2);
        string move = _possibleMoves[pickRandom];
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
        //this.enabled = true;
        _correctCombi = new List<string>();
        Score = 0;
        Round = 0;
        NewRound();
    }

    private void GameOver()
    {
        if (timeout == true)
        {
            GUIManager.EndGameReason("Time ran out!");
            timeout = false;
        }
        if (wrongCombi == true)
        {
            GUIManager.EndGameReason("You made the wrong move!");
            wrongCombi = false;
        }
        exerciseTime.Reset();
        roundCountdown.Reset();
        Round = 0;
        //this.enabled = false;
    }
}
