using RoboRyanTron.SceneReference;
using TMPro;
using UnityEngine;

namespace GrowGame
{
    public class GameOverScreenController: DialogController
    {
        [SerializeField] private TMP_Text messageField;
        [SerializeField] private GlobalGameState gameState;
        [SerializeField] private SceneReference menuScene;

        public override void ShowDialog()
        {
            base.ShowDialog();

            if (gameState.TimePassed >= gameState.LevelTime)
            {
                messageField.text = $"You ran <b>out of time</b>. Working in the garden earned you £ {gameState.Money:0.00} today.";
            }
            else
            {
                messageField.text = $"You have <b>no seeds</b> left. Working in the garden earned you £ {gameState.Money:0.00} today.";
            }
        }
    }
}