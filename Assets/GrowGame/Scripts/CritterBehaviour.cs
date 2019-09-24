using GrowGame.Data;
using UnityEngine;

namespace GrowGame
{
    public class CritterBehaviour : MonoBehaviour
    {
        [SerializeField]
        private CritterDefinition definition;

        [SerializeField]
        private PlantPartBehaviour target;
    }
}