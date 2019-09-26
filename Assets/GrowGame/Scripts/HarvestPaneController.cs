using System;
using UnityEngine;
using UnityEngine.UI;

namespace GrowGame
{
    public class HarvestPaneController : MonoBehaviour
    {
        [SerializeField] private FlowerBedUIController bedController;
        [SerializeField] private Button sellPlantButton;
        [SerializeField] private Button replantButton;

        public void OnPaneVisible()
        {
            sellPlantButton.interactable = bedController.GameState.HaveSeeds || !bedController.GameState.HaveTimeForPlanting;
            replantButton.interactable = bedController.GameState.HaveTimeForPlanting;
        }
    }
}