using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class BrainScript : MonoBehaviour {

    public int Round;
    public int Score;
    private readonly List<string> _possibleMoves = new List<string>() {"Duck", "Jump"};
    private List<string> _correctCombi;
    private List<string> _userCombi;
    private readonly int timePerExercise = 5000;
    private Stopwatch displayTime;
    private Stopwatch exerciseTime;
    private bool inputReady;
    private System.Random rand;

	// Use this for initialization
	void Start () {
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        Score = 0;
        Round = 0;
        _correctCombi = new List<string>();
        displayTime = new Stopwatch();
        exerciseTime = new Stopwatch();
        rand = new System.Random();
	}
	
	// Update is called once per frame
	void Update () {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEventManager.StartGame();
        }

        if (displayTime.IsRunning && displayTime.ElapsedMilliseconds <= 3000)
        {
            return;
        }

        if (displayTime.IsRunning && displayTime.ElapsedMilliseconds > 3000)
        {
            displayTime.Reset();
            GUIManager.RemoveNextMove();
            exerciseTime.Start();
        }

        if (exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds > timePerExercise)
        {
            GameEventManager.EndGame();
        }

        if (up && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= timePerExercise)
        {
            long time = exerciseTime.ElapsedMilliseconds;
            AddUserCombination(_possibleMoves[1]);
            exerciseTime.Reset();
            if (WrongCombination())
            {
                GameEventManager.EndGame();
                return;
            }
            
            if (_correctCombi.Count == _userCombi.Count)
            {
                NewRound();
            }
            UpdateScore(time);
            exerciseTime.Start();
        }
        if (down && exerciseTime.IsRunning && exerciseTime.ElapsedMilliseconds <= timePerExercise)
        {
            long time = exerciseTime.ElapsedMilliseconds;
            AddUserCombination(_possibleMoves[0]);
            exerciseTime.Reset();
            if (WrongCombination())
            {
                GameEventManager.EndGame();
                return;
            }
            
            if (_correctCombi.Count == _userCombi.Count)
            {
                NewRound();
            }
            UpdateScore(time);
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
    }

    private void NewRound()
    {
        //int pickRandom = Random.Range(1,2);
        int pickRandom = rand.Next(0, 2);
        string move = _possibleMoves[pickRandom];
        _correctCombi.Add(move);
        //UnityEngine.Debug.Log(_correctCombi.Count+": " + move);
        Round++;
        displayTime.Start();
        GUIManager.SetNextMove(_correctCombi);
        _userCombi = new List<string>();
        //UnityEngine.Debug.Log("UserCombi: " + _userCombi.Count);
    }

    private void AddUserCombination(string move)
    {
        if (_possibleMoves.Contains(move))
        {
            //UnityEngine.Debug.Log("UserCombi: "+_userCombi.Count+" - "+move);
            _userCombi.Add(move);
        }
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
        Round = 0;
        //this.enabled = false;
    }
}
