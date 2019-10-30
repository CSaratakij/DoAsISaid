using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoverBall
{
    public class UIController : MonoBehaviour
    {
        enum View
        {
            Menu,
            InGame,
            GameOver
        }

        [SerializeField]
        RectTransform[] menu;

        void Awake()
        {
            GameController.Instance.OnGameStart += OnGameStart;
            GameController.Instance.OnGameOver += OnGameOver;
        }

        void OnDestroy()
        {
            GameController.Instance.OnGameStart -= OnGameStart;
            GameController.Instance.OnGameOver -= OnGameOver;
        }

        void Start()
        {
            Show(View.Menu);
        }

        void OnGameStart()
        {
            Show(View.InGame);
        }

        void OnGameOver()
        {
            Show(View.GameOver);
        }

        void Show(View view)
        {
            Show((int)view);
        }

        void Show(int index, bool isShow = true)
        {
            HideAll();
            menu[index].gameObject.SetActive(isShow);
        }

        void HideAll()
        {
            foreach (RectTransform rect in menu)
            {
                rect.gameObject.SetActive(false);
            }
        }

        public void Restart()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
