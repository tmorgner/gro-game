using System;
using System.Collections.Generic;
using GrowGame.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GrowGame
{
    [RequireComponent(typeof(Animator))]
    public class PlantBehaviour : MonoBehaviour
    {
        private static readonly int stateId = Animator.StringToHash("State");
        private static readonly int deadId = Animator.StringToHash("Dead");
        private static readonly int normalizedTimeId = Animator.StringToHash("NormalizedTime");

        public enum PlantState
        {
            Seed = 0,
            YoungPlant = 1,
            Mature = 2,
            Ripening = 3,
            ReadyForHarvest = 4,
            Dead = 5,
            Harvested = 6
        }

        [SerializeField] private PlantDefinition definition;

        [SerializeField] private UnityEvent onPlantDied;

        [SerializeField] private UnityEvent onPlantHarvested;

        [SerializeField] private UnityEvent onPlantRipened;
        [SerializeField] private UnityEvent onStateChanged;
        
        [ShowNonSerializedField]
        private float ageInSeconds;

        private Animator animator;

        public PlantDefinition Definition => definition;

        private readonly Dictionary<PlantPart, List<PlantPartBehaviour>> parts;


        private PlantState state;

        private float timeAtAwake;

        public PlantBehaviour()
        {
            parts = new Dictionary<PlantPart, List<PlantPartBehaviour>>();
        }

        protected virtual void Awake()
        {
            var rawParts = GetComponentsInChildren<PlantPartBehaviour>();
            foreach (var p in rawParts)
            {
                p.OnHarmed.AddListener(UpdateHealth);

                if (parts.TryGetValue(p.Part, out var list))
                {
                    list.Add(p);
                }
                else
                {
                    parts.Add(p.Part, new List<PlantPartBehaviour>() {p});
                }
            }

            animator = GetComponent<Animator>();
            ageInSeconds = 0;
            timeAtAwake = Time.time;

            Health = ComputeHealth();
        }

        private void UpdateHealth()
        {
            Health = ComputeHealth();
            if (Health <= 0)
            {
                UpdatePlantState(PlantState.Dead);
            }
        }

        private float ComputeHealth()
        {
            if (state == PlantState.Dead)
            {
                return 0;
            }

            int count = 0;
            float sum = 0;

            if (state == PlantState.Seed)
            {
                if (parts.TryGetValue(PlantPart.Seed, out var seeds))
                {
                    SumCount(seeds, out sum, out count);
                }
            }
            else
            {
                foreach (var p in parts)
                {
                    if (p.Key == PlantPart.Seed)
                    {
                        continue;
                    }

                    SumCount(p.Value, out var tsum, out var tcount);

                    count += tcount;
                    sum += tsum;
                }
            }

            if (count == 0)
            {
                return 0;
            }
            else
            {
                return sum / count;
            }
        }


        public void Update()
        {
            ageInSeconds = Time.time - timeAtAwake;
            var ageInPercent = Mathf.Clamp01(ageInSeconds / definition.GrowTimeInSeconds);
            var nextState = GetStateForRelativeTime(ageInPercent);
            UpdatePlantState(nextState);

            animator.SetFloat(normalizedTimeId, NormalizedAnimatorTime);

        }

        PlantState GetStateForRelativeTime(float ageInPercent)
        {
            if (state == PlantState.Dead)
            {
                return PlantState.Dead;
            }

            if (ageInPercent < 0.25f)
            {
                return PlantState.Seed;
            }

            if (ageInPercent < 0.5f)
            {
                return PlantState.YoungPlant;
            }

            if (ageInPercent < 0.75f)
            {
                return PlantState.Mature;
            }

            if (ageInPercent < 1f)
            {
                return PlantState.Ripening;
            }

            return PlantState.ReadyForHarvest;
        }

        public float NormalizedAnimatorTime
        {
            get
            {
                // we know we have 4 segments in the plant's life.
                var ageInPercent = Mathf.Clamp01(ageInSeconds / definition.GrowTimeInSeconds);
                return (ageInPercent * 4) - Mathf.Floor(ageInPercent * 4);
            }
        }

        public float Health { get; private set; }

        void SumCount(List<PlantPartBehaviour> parts, out float sum, out int count)
        {
            sum = 0;
            count = 0;
            foreach (var plantPartBehaviour in parts)
            {
                sum += plantPartBehaviour.Health;
                count += 1;
            }
        }

        void UpdatePlantState(PlantState newState)
        {
            if (newState == state)
            {
                return;
            }

            state = newState;
            // finally reconfigure the animator.
            animator.SetBool(deadId, state == PlantState.Dead);
            animator.SetInteger(stateId, (int)state);

            onStateChanged?.Invoke();

            switch (newState)
            {
                case PlantState.Seed:
                    break;
                case PlantState.YoungPlant:
                    break;
                case PlantState.Mature:
                    break;
                case PlantState.Ripening:
                    break;
                case PlantState.ReadyForHarvest:
                    onPlantRipened?.Invoke();
                    break;
                case PlantState.Dead:
                    onPlantDied?.Invoke();
                    break;
                case PlantState.Harvested:
                    onPlantHarvested?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public void Harvested()
        {
            if (state != PlantState.ReadyForHarvest)
            {
                return;
            }

            UpdatePlantState(PlantState.Harvested);
        }
    }
}