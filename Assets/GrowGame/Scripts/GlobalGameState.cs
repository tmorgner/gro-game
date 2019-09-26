using System;
using System.Collections.Generic;
using System.Linq;
using GrowGame.Data;
using UnityEditor;
using UnityEngine;

namespace GrowGame
{
    public class GlobalGameState : MonoBehaviour
    {
        [SerializeField] private List<FlowerBedBehaviour> flowerBeds;
        [SerializeField] private List<CritterDefinition> availableCritters;
        [SerializeField] private List<PlantDefinition> availablePlants;

        [SerializeField] private float lightRecovery;
        [SerializeField] private float bugChance;

        public float Money { get; private set; }

        [SerializeField] private float levelTime;
        [SerializeField] private int starterSeedCount;

        private readonly Dictionary<long, int> seedCounts;

        public List<PlantDefinition> AvailablePlants => availablePlants;

        private readonly List<FlowerBedBehaviour> flowerBedsSortedByPos;

        public float LevelTime => levelTime;

        private readonly ActivationTimer timer;

        public GlobalGameState()
        {
            seedCounts = new Dictionary<long, int>();
            flowerBedsSortedByPos = new List<FlowerBedBehaviour>();
            timer = new ActivationTimer();
        }

        private void Awake()
        {
            flowerBedsSortedByPos.AddRange(flowerBeds.OrderBy(e => e.transform.position.x));
            timer.Start();

            foreach (var p in availablePlants)
            {
                seedCounts[p.GetInstanceID()] = 1;
            }
        }

        private void OnEnable()
        {
            foreach (var p in flowerBeds)
            {
                p.OnPlantSold += HandlePlantSold;
                p.OnPlantSeedsHarvested += HandlePlantSeedHarvested;
                p.Harvested.AddListener(HandlePlantFullyHarvested);
            }
        }

        private void OnDisable()
        {
            foreach (var p in flowerBeds)
            {
                p.OnPlantSold -= HandlePlantSold;
                p.OnPlantSeedsHarvested -= HandlePlantSeedHarvested;
                p.Harvested.RemoveListener(HandlePlantFullyHarvested);
            }
        }

        private void HandlePlantFullyHarvested()
        {
            foreach (var p in flowerBeds)
            {
                if (p.State == FlowerBedBehaviour.FlowerBedState.Inactive)
                {
                    p.ActivateFlowerBed();
                }
            }
        }

        private void HandlePlantSeedHarvested(object sender, PlantDefinition e)
        {
            AddSeeds(e);
        }

        private void HandlePlantSold(object sender, PlantBehaviour e)
        {
            var value = e.Health * e.Definition.SalePrice;
            Money += value;
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

        public event EventHandler SeedCountChanged;

        public bool HaveTimeForPlanting
        {
            get
            {
                var timeLeft = LevelTime - TimePassed;
                foreach (var kp in availablePlants)
                {
                    if (kp.GrowTimeInSeconds < timeLeft)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool HaveSeeds
        {
            get
            {
                foreach (var kp in seedCounts)
                {
                    if (kp.Value > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool OutOfSeeds
        {
            get
            {
                if (HaveSeeds)
                {
                    return false;
                }

                foreach (var b in flowerBeds)
                {
                    if (b.State == FlowerBedBehaviour.FlowerBedState.Inactive)
                    {
                        continue;
                    }

                    if (b.State != FlowerBedBehaviour.FlowerBedState.Empty)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void AddSeeds(PlantDefinition plant)
        {
            var sc = plant.SeedCount;
            if (sc > 0)
            {
                seedCounts[plant.GetInstanceID()] += sc;
                SeedCountChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int GetAllSeedCount()
        {
            int c = 0;
            foreach (var p in seedCounts)
            {
                c += p.Value;
            }

            return c;
        }

        public int GetSeedCount(PlantDefinition plant)
        {
            return seedCounts[plant.GetInstanceID()];
        }

        public bool TakeSeed(PlantDefinition plantDefinition)
        {
            var seedCount = GetSeedCount(plantDefinition);
            if (seedCount > 0)
            {
                seedCounts[plantDefinition.GetInstanceID()] -= 1;
                SeedCountChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }
    }
}