using System;
using System.Collections.Generic;
using System.Linq;
using GrowGame.Data;
using UnityEngine;

namespace GrowGame
{
    public class GlobalGameState: MonoBehaviour
    {
        [SerializeField]
        private List<FlowerBedBehaviour> flowerBeds;
        [SerializeField]
        private List<CritterDefinition> availableCritters;
        [SerializeField]
        private List<PlantDefinition> availablePlants;

        [SerializeField]
        private float lightRecovery;
        [SerializeField]
        private float bugChance;

        private List<FlowerBedBehaviour> flowerBedsSortedByPos;

        private void Awake()
        {
            flowerBedsSortedByPos = new List<FlowerBedBehaviour>();
            flowerBedsSortedByPos.AddRange(flowerBeds.OrderBy(e => e.transform.position.x));
        }

        private void Update()
        {
            // all plants start with full sunlight
            var effectiveShade = 0f;
            var previousShade = 0f;
            foreach (var b in flowerBedsSortedByPos)
            {
                // Apply sun 
                b.UpdateSunLight(1 - effectiveShade);
                // recover shade from the previous plants shade.
                effectiveShade = Mathf.Clamp01(effectiveShade + lightRecovery * previousShade);
                // compute next plant's shade level
                effectiveShade = Mathf.Clamp01(effectiveShade - b.ProvidedShade);
                // remember how much total shade is active for faster recovery.
                previousShade = Mathf.Clamp01(previousShade + b.ProvidedShade);
            }

            foreach (var b in flowerBedsSortedByPos)
            {
                if (b.State != FlowerBedBehaviour.FlowerBedState.Planted)
                {
                    continue;
                }

                if (UnityEngine.Random.value <= (bugChance * Time.deltaTime))
                {
                    if (b.SelectBug(this.availableCritters, out var critter, out var target))
                    {
                        var critterInstance = Instantiate(critter);
                            // todo: Position and send on your way...
                    }
                }
            }
        }

    }
}