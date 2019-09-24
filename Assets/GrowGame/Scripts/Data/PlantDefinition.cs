using NaughtyAttributes;
using UnityEngine;

namespace GrowGame.Data
{
    [CreateAssetMenu(menuName = "GroGame/Plant Definition")]
    public class PlantDefinition : ScriptableObject
    {
        [SerializeField] private float growTimeInSeconds;

        [MinMaxSlider(0, 1)]
        [SerializeField] private Vector2 waterRequirement;
        
        [MinMaxSlider(0, 1)]
        [SerializeField] private Vector2 nutritionRequirement;
        
        [MinMaxSlider(0, 1)]
        [SerializeField] private Vector2 sunRequirement;

        [SerializeField] private PlantBehaviour plantPrefab;

        public PlantBehaviour PlantPrefab => plantPrefab;

        public float GrowTimeInSeconds => Mathf.Min(growTimeInSeconds, 1f);
    }
}