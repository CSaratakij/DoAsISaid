using System;
using UnityEngine;

namespace HoverBall
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance = null;

        public Action OnGameStart;
        public Action OnGameOver;

        public bool IsGameStart { get; private set; }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void GameStart()
        {
            if (IsGameStart)
                return;

            IsGameStart = true;
            OnGameStart?.Invoke();

            Debug.Log("Game Start..");
        }

        public void GameOver()
        {
            if (!IsGameStart)
                return;

            IsGameStart = false;
            OnGameOver?.Invoke();

            Debug.Log("Game Over..");
        }
    }
}
