using System;
using System.Collections.Generic;
using UnityEngine;

namespace GrowGame.Data
{
    public class SeedListController : MonoBehaviour
    {
        [SerializeField]
        private GlobalGameState globalState;

        [SerializeField]
        private List<SeedListEntryController> buttons;

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


        private void Update()
        {
            foreach (var button in buttons)
            {
                if (button.Plant)
                {
                    button.UpdateSeedCount(globalState.GetSeedCount(button.Plant));
                }
            }
        }
    }
}