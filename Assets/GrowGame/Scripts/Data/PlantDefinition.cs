using NaughtyAttributes;
using UnityEngine;

namespace Assets.GrowGame.Scripts.Data
{
    public class PlantDefinition : ScriptableObject
    {
        [SerializeField] private float growTimeInTicks;
        [Slider(0,1)]
        [SerializeField] private Vector2 waterRequirement;
        [Slider(0, 1)]
        [SerializeField] private Vector2 nutritionRequirement;
        [Slider(0, 1)]
        [SerializeField] private Vector2 sunRequirement;
    }
}