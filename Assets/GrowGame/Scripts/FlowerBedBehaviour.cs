using System.Collections.Generic;
using UnityEngine;

namespace Assets.GrowGame.Scripts.Data
{
    public class FlowerBedBehaviour: MonoBehaviour
    {
        [SerializeField]
        PlantBehaviour plant;
    }

    public class GlobalGameState: MonoBehaviour
    {
        [SerializeField]
        private List<FlowerBedBehaviour> flowerBeds;
        [SerializeField]
        private List<CritterDefinition> availableCritters;
        [SerializeField]
        private List<PlantDefinition> availablePlants;

    }
}