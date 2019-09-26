using System;
using System.Collections.Generic;
using System.Linq;
using GrowGame.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GrowGame
{
    public class FlowerBedBehaviour: MonoBehaviour
    {
        public enum FlowerBedState
        {
            Inactive, Empty, Planted, Ripe
        }

        [SerializeField]
        [BoxGroup("Internal Components")]
        [Required]
        private GameObject plantAnchor;

        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent seeded;
        
        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent ripened;

        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent harvested;
        
        [SerializeField]
        [BoxGroup("Events")]
        [Tooltip("This event is thrown when the plant started dying.")]
        private UnityEvent dying;

        [SerializeField]
        [BoxGroup("Events")]
        [Tooltip("This event is thrown when the plant finished dying.")]
        private UnityEvent died;

        [SerializeField]
        [BoxGroup("Events")]
        [Tooltip("This event is thrown when the plant awaits seeds.")]
        private UnityEvent activated;

        [Tooltip("How much water is added when watering the plant.")]
        [SerializeField] private float waterSupplyPerSecond;
  
        [Tooltip("How much nutrition is added when fertilizing the plant.")]
        [SerializeField] private float nutritionSupplyPerSecond;
        [Tooltip("How much water goes away in direct sunlight?")]
        [SerializeField] private float waterEvaporatedPerSecond;

        public event EventHandler<PlantDefinition> OnPlantSeedsHarvested;
        public event EventHandler<PlantBehaviour> OnPlantSold;

        private float waterLevel;
        private float nutritionLevel;
        private float sunlightLevel;

        public UnityEvent Activated => activated;

        public UnityEvent Seeded => seeded;

        public UnityEvent Ripened => ripened;

        public UnityEvent Harvested => harvested;

        public UnityEvent Dying => dying;

        public UnityEvent Died => died;

        public FlowerBedState State
        {
            get => state;
            private set => state = value;
        }

        [ShowNonSerializedField]
        private PlantBehaviour plant;

        [SerializeField]
        private FlowerBedState state;

        public void PlantSeed(PlantDefinition seed)
        {
            if (State != FlowerBedState.Empty)
            {
                throw new InvalidOperationException();
            }

            plant = Instantiate(seed.PlantPrefab, plantAnchor.transform);
            plant.transform.localPosition = Vector3.zero;
            plant.transform.localScale = Vector3.one;
            plant.transform.localRotation = Quaternion.identity;

            plant.OnPlantRipened.AddListener(PlantRipened);
            plant.OnPlantDying.AddListener(PlantDying);
            plant.OnPlantDied.AddListener(PlantDied);
            plant.OnPlantHarvested.AddListener(PlantHarvested);

            State = FlowerBedState.Planted;
            Seeded?.Invoke();
        }

        public void PlantDying()
        {
            State = FlowerBedState.Inactive;
            Dying?.Invoke();
        }

        public void PlantDied()
        {
            State = FlowerBedState.Empty;
            Debug.Log("Plant died");
            if (plant)
            {
                plant.OnPlantRipened.RemoveListener(PlantRipened);
                plant.OnPlantDying.RemoveListener(PlantDying);
                plant.OnPlantDied.RemoveListener(PlantDied);
                plant.OnPlantHarvested.RemoveListener(PlantHarvested);

                Destroy(plant.gameObject);
                plant = null;
            }
            Died?.Invoke();
        }

        void PlantRipened()
        {
            State = FlowerBedState.Ripe;
            Ripened?.Invoke();
        }

        void PlantHarvested()
        {
            State = FlowerBedState.Empty;
            Debug.Log("Plant Harvested " + plant);
            if (plant)
            {
                plant.OnPlantRipened.RemoveListener(PlantRipened);
                plant.OnPlantDying.RemoveListener(PlantDying);
                plant.OnPlantDied.RemoveListener(PlantDied);
                plant.OnPlantHarvested.RemoveListener(PlantHarvested);

                Destroy(plant.gameObject);
                plant = null;
            }

            Harvested?.Invoke();
        }

        public void HarvestPlant()
        {
            if (!plant)
            {
                return;
            }

            if (State != FlowerBedState.Ripe)
            {
                return;
            }

            OnPlantSeedsHarvested?.Invoke(this, Plant.Definition);
            plant.Harvested();
        }

        public void SellPlant()
        {
            if (!plant)
            {
                return;
            }

            if (State != FlowerBedState.Ripe)
            {
                return;
            }

            OnPlantSold?.Invoke(this, Plant);
            plant.Harvested();
        }

        public void AddWaterOnce()
        {
            Debug.Log("Adding Water");
            waterLevel = Mathf.Clamp01(waterLevel + waterSupplyPerSecond);
        }

        public void AddNutritionOnce()
        {
            Debug.Log("Adding Nutrition");
            nutritionLevel = Mathf.Clamp01(nutritionLevel + nutritionSupplyPerSecond);
        }

        public void AddWaterContinuously()
        {
            waterLevel = Mathf.Clamp01(waterLevel + waterSupplyPerSecond * Time.deltaTime);
        }

        public void AddNutritionContinuously()
        {
            waterLevel = Mathf.Clamp01(waterLevel + nutritionSupplyPerSecond * Time.deltaTime);
        }

        public void UpdateSunLight(float sunlight)
        {
            this.sunlightLevel = sunlight;
        }

        public float WaterLevel => waterLevel;

        public float NutritionLevel => nutritionLevel;

        public PlantBehaviour Plant => plant;

        public float ProvidedShade
        {
            get
            {
                if (plant && plant.Definition)
                {
                    return plant.Definition.ProducedShade(plant.Growth);
                }

                return 0;
            }
        }

        private void Update()
        {
            if (plant && plant.Definition)
            {
                var plantDefinition = plant.Definition;
                var plantAge = plant.Growth;

                // plants consume water just for photosynthesis
                // plants also provide shade, which reduces water evaporation
                var waterConsumption = plantDefinition.RequiredWater(plantAge, sunlightLevel) + 
                    waterEvaporatedPerSecond * sunlightLevel * plantDefinition.ProducedShade(plantAge);
                waterLevel = Mathf.Clamp01(waterLevel - waterConsumption * Time.deltaTime); 
                
                // plants consume nutrition
                var nutrientConsumption = plantDefinition.RequiredNutrition(plantAge);
                nutritionLevel = Mathf.Clamp01(nutritionLevel - nutrientConsumption * Time.deltaTime);
                
                plant.TickGrowth(waterLevel, nutritionLevel, sunlightLevel);
            }
            else
            {
                waterLevel = Mathf.Clamp01(waterLevel - waterEvaporatedPerSecond * Time.deltaTime);
            }
        }

        public bool SelectBug(IReadOnlyList<CritterDefinition> availableCritters, 
            out CritterDefinition selectedCritter,
            out PlantPartBehaviour target)
        {
            if (plant && plant.ChoosePlantPart(out var part))
            {
                var list = availableCritters.Where(cr => cr.TargetPart == part.Part).ToList();
                if (list.Count != 0)
                {
                    selectedCritter = list[UnityEngine.Random.Range(0, list.Count)];
                    target = part;
                    return true;
                }
            }

            selectedCritter = default;
            target = default;
            return false;
        }

        public void ActivateFlowerBed()
        {
            if (State == FlowerBedState.Inactive)
            {
                State = FlowerBedState.Empty;
            }
        }
    }
}