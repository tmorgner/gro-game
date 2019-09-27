using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GrowGame
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button quitButton;
        [SerializeField] private Button playButton;
        [SerializeField] private SceneReference gameScene;

        private void Awake()
        {
            Time.timeScale = 1;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                quitButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            quitButton.onClick.AddListener(OnQuitApplication);
            playButton.onClick.AddListener(OnPlay);
        }

        private void OnDisable()
        {
            quitButton.onClick.RemoveListener(OnQuitApplication);
            playButton.onClick.RemoveListener(OnPlay);
        }

        private void OnPlay()
        {
            SceneManager.LoadScene(gameScene.SceneName);
        }

        private void OnQuitApplication()
        {
            Application.Quit();
        }

    }
}