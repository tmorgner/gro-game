using System;
using System.Linq;
using GrowGame.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GrowGame
{
    public class FlowerBedUIController : MonoBehaviour
    {
        [SerializeField] private Slider nutritionSlider;
        [SerializeField] private Slider waterSlider;
        [SerializeField] private Button waterButton;
        [SerializeField] private Button nutritionButton;
        [SerializeField] private Slider healthLevel;

        [SerializeField] private CanvasGroup selfPane;
        [SerializeField] private CanvasGroup seedingPane;
        [SerializeField] private CanvasGroup growingPane;
        [SerializeField] private CanvasGroup harvestPane;
        [SerializeField] private SeedOptionsController seedOptions;

        [SerializeField] private FlowerBedBehaviour flowerBed;

        [SerializeField] private GlobalGameState gameState;
        [SerializeField] private HarvestPaneController harvestPaneBehaviour;

        public GlobalGameState GameState => gameState;

        private bool hovered;

        public void OnPointerEnter()
        {
            hovered = true;
            ApplyState();
        }

        public void OnPointerExit()
        {
            hovered = false;
            ApplyState();
        }

        private void Awake()
        {
            SetPaneState(selfPane, false);
            ApplyState();
        }

        private void OnEnable()
        {
            flowerBed.Activated.AddListener(OnBedActivated);
            flowerBed.Died.AddListener(OnPlantDied);
            flowerBed.Dying.AddListener(OnPlantDying);
            flowerBed.Seeded.AddListener(OnPlantSeeded);
            flowerBed.Ripened.AddListener(OnPlantRipened);
            flowerBed.Harvested.AddListener(OnPlantHarvested);
            gameState.SeedCountChanged += OnSeedCountChanged;

            nutritionButton.onClick.AddListener(OnNutritionSupply);
            waterButton.onClick.AddListener(OnWaterSupply);
            seedOptions.OnSeedSelected += OnSeedSelected;
        }

        private void OnSeedCountChanged(object sender, EventArgs e)
        {
            ApplyState();
        }

        private void OnDisable()
        {
            flowerBed.Activated.RemoveListener(OnBedActivated);
            flowerBed.Died.RemoveListener(OnPlantDied);
            flowerBed.Dying.RemoveListener(OnPlantDying);
            flowerBed.Seeded.RemoveListener(OnPlantSeeded);
            flowerBed.Ripened.RemoveListener(OnPlantRipened);
            flowerBed.Harvested.RemoveListener(OnPlantHarvested);
            gameState.SeedCountChanged -= OnSeedCountChanged;

            nutritionButton.onClick.RemoveListener(OnNutritionSupply);
            waterButton.onClick.RemoveListener(OnWaterSupply);
            seedOptions.OnSeedSelected -= OnSeedSelected;
        }

        private void OnBedActivated()
        {
            EnablePane(selfPane);
            EnablePane(seedingPane);
        }

        private void OnSeedSelected(object sender, PlantDefinition e)
        {
            if (gameState.TakeSeed(e))
            {
                flowerBed.PlantSeed(e);
            }
        }

        private void OnWaterSupply()
        {
            flowerBed.AddWaterOnce();
        }

        private void OnNutritionSupply()
        {
            flowerBed.AddNutritionOnce();
        }

        void SetPaneState(CanvasGroup g, bool state, float alphaDisabled = 0, float alphaEnabled = 1)
        {
            if (state)
            {
                g.alpha = alphaEnabled;
                g.blocksRaycasts = true;
            }
            else
            {
                g.alpha = alphaDisabled;
                g.blocksRaycasts = false;
            }
        }

        void DisablePane(CanvasGroup g)
        {
            SetPaneState(g, false);
        }

        void EnablePane(CanvasGroup g)
        {
            SetPaneState(g, true);
        }

        private void OnPlantSeeded()
        {
            DisablePane(seedingPane);
            EnablePane(growingPane);
        }

        private void OnPlantRipened()
        {
            DisablePane(growingPane);
            EnablePane(harvestPane);
        }

        private void OnPlantHarvested()
        {
            DisablePane(harvestPane);
            EnablePane(seedingPane);
        }

        private void OnPlantDying()
        {
            DisablePane(harvestPane);
            DisablePane(seedingPane);
            DisablePane(growingPane);
        }

        private void OnPlantDied()
        {
            EnablePane(seedingPane);
        }

        [UsedImplicitly]
        public void HarvestPlantSeeds()
        {
            flowerBed.HarvestPlant();
            DisablePane(harvestPane);
        }

        [UsedImplicitly]
        public void SellPlantFruit()
        {
            flowerBed.SellPlant();
            DisablePane(harvestPane);
        }

        private FlowerBedBehaviour.FlowerBedState state;
        public event EventHandler FlowBedStateChanged;

        public void UpdateState(FlowerBedBehaviour.FlowerBedState nextState)
        {
            if (nextState == state)
            {
                return;
            }

            state = nextState;
            FlowBedStateChanged?.Invoke(this, EventArgs.Empty);
            ApplyState();
        }

        void ApplyState()
        {
            if (!flowerBed || state == FlowerBedBehaviour.FlowerBedState.Inactive)
            {
                SetPaneState(selfPane, false);
            }
            else
            {
                if (state == FlowerBedBehaviour.FlowerBedState.Empty && !gameState.HaveSeeds)
                {
                    SetPaneState(selfPane, false);
                }
                else
                {
                    SetPaneState(selfPane, hovered, 0.5f);
                }
                SetPaneState(seedingPane, state == FlowerBedBehaviour.FlowerBedState.Empty);
                SetPaneState(growingPane, state == FlowerBedBehaviour.FlowerBedState.Planted);
                SetPaneState(harvestPane, state == FlowerBedBehaviour.FlowerBedState.Ripe);
                if (state == FlowerBedBehaviour.FlowerBedState.Ripe)
                {
                    harvestPaneBehaviour.OnPaneVisible();
                }
            }
        }

        private void Update()
        {
            UpdateState(flowerBed.State);

            if (flowerBed.State == FlowerBedBehaviour.FlowerBedState.Planted ||
                flowerBed.State == FlowerBedBehaviour.FlowerBedState.Ripe)
            {
                nutritionSlider.value = flowerBed.NutritionLevel;
                waterSlider.value = flowerBed.WaterLevel;
                healthLevel.value = flowerBed.Plant.Health;
            }
        }
    }
}