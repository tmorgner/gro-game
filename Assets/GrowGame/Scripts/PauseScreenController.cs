using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GrowGame
{
    public class PauseScreenController : DialogController
    {
        [SerializeField] private SceneReference mainScene;

        public void ReturnToMain()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(mainScene.SceneName);
        }
    }
}