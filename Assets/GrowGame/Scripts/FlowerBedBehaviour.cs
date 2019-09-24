using System;
using GrowGame.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GrowGame
{
    public class FlowerBedBehaviour: MonoBehaviour
    {
        public enum FlowerBedState
        {
            Inactive, Empty, Planted, Ripe
        }

        [SerializeField]
        [BoxGroup("Internal Components")]
        [Required]
        private GameObject plantAnchor;

        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent seeded;
        
        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent ripened;

        [SerializeField]
        [BoxGroup("Events")]
        private UnityEvent harvested;
        
        [SerializeField]
        [BoxGroup("Events")]
        [Tooltip("This event is thrown when the plant started dying.")]
        private UnityEvent dying;

        [SerializeField]
        [BoxGroup("Events")]
        [Tooltip("This event is thrown when the plant finished dying.")]
        private UnityEvent died;

        public UnityEvent Seeded => seeded;

        public UnityEvent Ripened => ripened;

        public UnityEvent Harvested => harvested;

        public UnityEvent Dying => dying;

        public UnityEvent Died => died;

        [ShowNativeProperty]
        public FlowerBedState State { get; private set; }

        [ShowNonSerializedField]
        private PlantBehaviour plant;

        public void PlantSeed(PlantDefinition seed)
        {
            if (State != FlowerBedState.Empty)
            {
                throw new InvalidOperationException();
            }

            plant = Instantiate(seed.PlantPrefab, Vector3.zero, Quaternion.identity, plantAnchor.transform);
            State = FlowerBedState.Planted;
            Seeded?.Invoke();
        }

        public void PlantDying()
        {
            State = FlowerBedState.Inactive;
            Dying?.Invoke();
        }

        public void PlantDied()
        {
            State = FlowerBedState.Empty;
            Died?.Invoke();
            if (plant)
            {
                Destroy(plant);
                plant = null;
            }
        }

        public void PlantRipened()
        {
            State = FlowerBedState.Ripe;
            Ripened?.Invoke();
        }

        public void PlantHarvested()
        {
            plant.Harvested();
            State = FlowerBedState.Empty;
            Harvested?.Invoke();
        }

        // todo: Resource adding and consumption

    }
}