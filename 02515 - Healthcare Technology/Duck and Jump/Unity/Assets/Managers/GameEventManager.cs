using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Managers {
    public class GameEventManager {
        public delegate void GameEvent();

        public static event GameEvent GameStart , GameOver;

        public static void StartGame() {
            if (GameStart != null) {
                GameStart();
            }
        }

        public static void EndGame() {
            if (GameOver != null) {
                GameOver();
            }
        }
    }
}