using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GrowGame.Data
{
    public class SeedListEntryController : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        public void Configure(PlantDefinition plantDefinition)
        {
            text.text = "0";
            icon.sprite = plantDefinition.Image;
            Plant = plantDefinition;
        }

        public PlantDefinition Plant { get; private set; }

        public void UpdateSeedCount(int seedCount)
        {
            text.text = $"{seedCount}";
        }
    }
}