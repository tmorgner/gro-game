using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GrowGame
{
    public class GlobalGameStateController : MonoBehaviour
    {
        [FormerlySerializedAs("slider")] [SerializeField] private Slider timeSlider;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GlobalGameState gameState;
        [SerializeField] private GameOverScreenController gameOverScreen;
        [SerializeField] private PauseScreenController pauseScreen;

        private void OnEnable()
        {
            pauseButton.onClick.AddListener(OnPausePressed);
            timeSlider.minValue = 0;
            timeSlider.maxValue = gameState.LevelTime;
        }

        private void OnDisable()
        {
            pauseButton.onClick.RemoveListener(OnPausePressed);
        }

        private void OnPausePressed()
        {
            pauseScreen.ShowDialog();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPausePressed();
            }

            timeSlider.value = gameState.TimePassed;

            if (gameState.TimePassed >= gameState.LevelTime ||
                gameState.OutOfSeeds)
            {
                gameOverScreen.ShowDialog();
            }
        }
    }
}