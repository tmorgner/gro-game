using UnityEngine;

namespace Assets.GrowGame.Scripts.Data
{
    public class CritterDefinition: ScriptableObject
    {
        [SerializeField]
        private float velocity;

        [SerializeField]
        private float damageApplied;

        [SerializeField]
        private PlantPart targetPart;
    }
}