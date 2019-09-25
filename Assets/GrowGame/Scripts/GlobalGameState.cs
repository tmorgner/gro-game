using System.Collections.Generic;
using System.Linq;
using GrowGame.Data;
using UnityEditor;
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

        [SerializeField] private float levelTime;

        public List<PlantDefinition> AvailablePlants => availablePlants;

        private List<FlowerBedBehaviour> flowerBedsSortedByPos;

        public float LevelTime => levelTime;

        private ActivationTimer timer;

        private void Awake()
        {
            flowerBedsSortedByPos = new List<FlowerBedBehaviour>();
            flowerBedsSortedByPos.AddRange(flowerBeds.OrderBy(e => e.transform.position.x));
            timer = new ActivationTimer();
            timer.Start();
        }

        public float TimePassed => timer.TimePassed;

        private void Update()
        {
            timer.Update();

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