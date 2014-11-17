using System;
using System.Collections.Generic;
using System.Text;


public class GameEventManager
{
    public delegate void GameEvent();

    public static event GameEvent GameStart, GameOver, RoundStart;

    public static void StartGame()
    {
        if (GameStart != null)
        {
            GameStart();
        }
    }

    public static void EndGame()
    {
        if (GameOver != null)
        {
            GameOver();
        }
    }

    public static void StartRound()
    {
        if (RoundStart != null)
        {
            RoundStart();
        }
    }
}