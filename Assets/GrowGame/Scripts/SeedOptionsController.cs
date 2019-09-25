using System;
using System.Collections.Generic;
using GrowGame.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GrowGame
{
    public class SeedOptionsController: MonoBehaviour
    {
        [SerializeField]
        private GlobalGameState globalState;

        [SerializeField]
        private List<SeedButtonController> buttons;

        public event EventHandler<PlantDefinition> OnSeedSelected;

        private void Awake()
        {
            var maxIndex = Math.Min(globalState.AvailablePlants.Count, buttons.Count);
            for (var index = 0; index < maxIndex; index++)
            {
                var plant = globalState.AvailablePlants[index];
                var button = buttons[index];
                button.gameObject.SetActive(true);
                button.Configure(plant);
            }

            for (var index = maxIndex; index < buttons.Count; index += 1)
            {
                var button = buttons[index];
                button.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            foreach (var b in buttons)
            {
                b.OnClicked -= SeedSelectedHandler;
            }
        }

        private void SeedSelectedHandler(object sender, PlantDefinition e)
        {
            OnSeedSelected?.Invoke(this, e);
        }

        private void OnEnable()
        {
            foreach (var b in buttons)
            {
                b.OnClicked += SeedSelectedHandler;
            }
        }
    }
}