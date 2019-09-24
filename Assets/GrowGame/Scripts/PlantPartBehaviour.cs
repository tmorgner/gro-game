using UnityEngine;

namespace Assets.GrowGame.Scripts.Data
{
    public class PlantPartBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlantPart part;

        public PlantPart Part => part;
    }
}