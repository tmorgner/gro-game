using System.Collections.Generic;
using GrowGame.Data;
using UnityEngine;

namespace GrowGame
{
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