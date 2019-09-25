using System;
using GrowGame.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GrowGame
{
    public class FlowerBedUIController: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        [SerializeField]
        private FlowerBedBehaviour flowerBed;

        private bool hovered;
        private bool active;

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
        }

        private void Awake()
        {
            if (!flowerBed || flowerBed.State == FlowerBedBehaviour.FlowerBedState.Inactive)
            {
                SetPaneState(selfPane, false);
            }
            else
            {
                SetPaneState(selfPane, true);
                SetPaneState(seedingPane, flowerBed.State == FlowerBedBehaviour.FlowerBedState.Empty);
                SetPaneState(growingPane, flowerBed.State == FlowerBedBehaviour.FlowerBedState.Planted);
                SetPaneState(harvestPane, flowerBed.State == FlowerBedBehaviour.FlowerBedState.Ripe);
            }
        }

        private void OnEnable()
        {
            flowerBed.Activated.AddListener(OnBedActivated);
            flowerBed.Died.AddListener(OnPlantDied);
            flowerBed.Dying.AddListener(OnPlantDying);
            flowerBed.Seeded.AddListener(OnPlantSeeded);
            flowerBed.Ripened.AddListener(OnPlantRipened);
            flowerBed.Harvested.AddListener(OnPlantHarvested);

            nutritionButton.onClick.AddListener(OnNutritionSupply);
            waterButton.onClick.AddListener(OnWaterSupply);
            seedOptions.OnSeedSelected += OnSeedSelected;
        }

        private void OnDisable()
        {
            flowerBed.Activated.RemoveListener(OnBedActivated);
            flowerBed.Died.RemoveListener(OnPlantDied);
            flowerBed.Dying.RemoveListener(OnPlantDying);
            flowerBed.Seeded.RemoveListener(OnPlantSeeded);
            flowerBed.Ripened.RemoveListener(OnPlantRipened);
            flowerBed.Harvested.RemoveListener(OnPlantHarvested);

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
            flowerBed.PlantSeed(e);
        }

        private void OnWaterSupply()
        {
            flowerBed.AddWaterOnce();
        }

        private void OnNutritionSupply()
        {
            flowerBed.AddNutritionOnce();
        }

        void SetPaneState(CanvasGroup g, bool state)
        {
            if (state) EnablePane(g);
            else DisablePane(g);
        }

        void DisablePane(CanvasGroup g)
        {
            g.alpha = 0;
            g.blocksRaycasts = false;
        }

        void EnablePane(CanvasGroup g)
        {
            g.alpha = 1;
            g.blocksRaycasts = true;
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

        private void Update()
        {
            if (flowerBed.State == FlowerBedBehaviour.FlowerBedState.Inactive)
            {
                active = false;
                DisablePane(selfPane);
                DisablePane(seedingPane);
                return;
            }

            if (active == false)
            {
                EnablePane(selfPane);
            }

            if (flowerBed.State == FlowerBedBehaviour.FlowerBedState.Planted ||
                flowerBed.State == FlowerBedBehaviour.FlowerBedState.Ripe)
            {
                nutritionSlider.value = flowerBed.NutritionLevel;
                waterSlider.value = flowerBed.NutritionLevel;
                healthLevel.value = flowerBed.Plant.Health;
            }
        }
    }
}