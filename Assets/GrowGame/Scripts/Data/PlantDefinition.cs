using NaughtyAttributes;
using UnityEngine;

namespace GrowGame.Data
{
    [CreateAssetMenu(menuName = "GroGame/Plant Definition")]
    public class PlantDefinition : ScriptableObject
    {
        [SerializeField] private string label;
        [SerializeField] private Sprite image;

        [SerializeField] private float growTimeInSeconds;
        [SerializeField] private float salePrice;
        [SerializeField] private int seedCount;

        [SerializeField] private PlantBehaviour plantPrefab;

        [SerializeField] private AnimationCurve nutritionByAge;
        [SerializeField] private AnimationCurve waterByAge;
        [SerializeField] private AnimationCurve waterBySunlight;
        [SerializeField] private AnimationCurve shadeByAge;

        [SerializeField] private AnimationCurve growthWaterFactor;
        [SerializeField] private AnimationCurve growthNutritionFactor;
        [SerializeField] private AnimationCurve growthSunFactor;

        public float SalePrice => salePrice;

        public int SeedCount => seedCount;

        public string Label => label;

        public Sprite Image => image;

        public PlantBehaviour PlantPrefab => plantPrefab;

        public float GrowTimeInSeconds => Mathf.Max(growTimeInSeconds, 1f);

        public float RequiredNutrition(float age)
        {
            return nutritionByAge.Evaluate(age);
        }

        public float RequiredWater(float age, float sunlight)
        {
            return waterByAge.Evaluate(age) * waterBySunlight.Evaluate(age);
        }

        public float ProducedShade(float age)
        {
            return shadeByAge.Evaluate(age);
        }

        public float ComputeGrowth(float sun, float water, float nutrition)
        {
            var growthSun = growthSunFactor.Evaluate(sun);
            var growthWater = growthWaterFactor.Evaluate(water);
            var growthNutrition = growthNutritionFactor.Evaluate(nutrition);
            // Debug.Log($"Growth: {growthSun} | {growthWater} | {growthNutrition}");
            return Mathf.Min(growthNutrition, Mathf.Min(growthSun, growthWater));
        }
    }
}