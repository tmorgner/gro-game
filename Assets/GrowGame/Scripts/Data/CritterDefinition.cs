using NaughtyAttributes;
using UnityEngine;

namespace GrowGame.Data
{
    [CreateAssetMenu(menuName = "GroGame/Critter Definition")]
    public class CritterDefinition: ScriptableObject
    {
        [SerializeField]
        private float velocity;

        [SerializeField]
        [Slider(0, 1)]
        private float damageApplied;

        [SerializeField]
        private PlantPart targetPart;

        [SerializeField]
        private CritterBehaviour critterPrefab;

        public PlantPart TargetPart => targetPart;

        public CritterBehaviour CritterPrefab => critterPrefab;
    }
}