using System;
using GrowGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GrowGame
{
    public class SeedButtonController : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image icon;
        private PlantDefinition plant;
        public event EventHandler<PlantDefinition> OnClicked;

        public void Configure(PlantDefinition plant)
        {
            this.plant = plant;
            text.text = plant.Label;
            icon.sprite = plant.Image;
        }

        private void OnDisable()
        {
            var bt = GetComponent<Button>();
            if (bt)
                bt.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (plant)
            {
                OnClicked?.Invoke(this, plant);
            }
        }

        private void OnEnable()
        {
            var bt = GetComponent<Button>();
            if (bt)
                bt.onClick.AddListener(OnClick);
        }
    }
}