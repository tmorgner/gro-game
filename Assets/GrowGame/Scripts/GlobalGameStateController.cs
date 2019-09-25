using UnityEngine;
using UnityEngine.UI;

namespace GrowGame
{
    public class GlobalGameStateController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GlobalGameState gameState;

        [SerializeField] private PauseScreenController pauseScreen;

        private void OnEnable()
        {
            pauseButton.onClick.AddListener(OnPausePressed);
            slider.minValue = 0;
            slider.maxValue = gameState.LevelTime;
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

            slider.value = gameState.TimePassed;
        }
    }
}