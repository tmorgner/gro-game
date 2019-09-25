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
        private float growth;

        private ActivationTimer timer;
        private Animator animator;

        public PlantDefinition Definition => definition;

        private readonly Dictionary<PlantPart, List<PlantPartBehaviour>> parts;
        private readonly List<PlantPartBehaviour> partsLinear;


        private PlantState state;

        public PlantBehaviour()
        {
            parts = new Dictionary<PlantPart, List<PlantPartBehaviour>>();
            partsLinear = new List<PlantPartBehaviour>();
        }

        protected virtual void Awake()
        {
            var rawParts = GetComponentsInChildren<PlantPartBehaviour>();
            foreach (var p in rawParts)
            {
                p.OnHarmed.AddListener(UpdateHealth);
                partsLinear.Add(p);

                if (parts.TryGetValue(p.Part, out var list))
                {
                    Debug.Log("Added " + p.Part + " part " + p.name);
                    list.Add(p);
                }
                else
                {
                    Debug.Log("Found " + p.Part + " part " + p.name);
                    parts.Add(p.Part, new List<PlantPartBehaviour>() {p});
                }
            }

            Debug.Log("Found " + partsLinear.Count + " parts");

            animator = GetComponent<Animator>();
            timer = new ActivationTimer();
            timer.Start();

        }

        private void Start()
        {
            Health = ComputeHealth();
        }

        private void UpdateHealth()
        {
            Health = ComputeHealth();
            if (Health <= 0)
            {
                UpdatePlantState(PlantState.Dead);

                // Get rid of all dead parts so that the bugs dont attack zombies
                for (var i = partsLinear.Count - 1; i >= 0; i--)
                {
                    var p = partsLinear[i];
                    if (p.Health <= 0)
                    {
                        partsLinear.RemoveAt(i);
                    }
                }
            }
        }

        private float ComputeHealth()
        {
            if (state == PlantState.Dead)
            {
                Debug.Log("Plant is dead");
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
                else
                {
                    Debug.LogError("No seed part found?");
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

            Debug.Log("Plant health: " + count + " - " + sum);
            if (count == 0)
            {
                return 0;
            }
            else
            {
                return sum / count;
            }
        }

        public float Growth => growth;
        // public float NormalizedAge => Mathf.Clamp01(ageInSeconds / definition.GrowTimeInSeconds);

        PlantState GetStateForRelativeTime(float plantGrowth)
        {
            if (state == PlantState.Dead)
            {
                return PlantState.Dead;
            }

            if (plantGrowth < 0.25f)
            {
                return PlantState.Seed;
            }

            if (plantGrowth < 0.5f)
            {
                return PlantState.YoungPlant;
            }

            if (plantGrowth < 0.75f)
            {
                return PlantState.Mature;
            }

            if (plantGrowth < 1f)
            {
                return PlantState.Ripening;
            }

            return PlantState.ReadyForHarvest;
        }

        float NormalizedAnimatorTime
        {
            get
            {
                // we know we have 4 segments in the plant's life. This is a modulo-4 to get a normalized time (0..1)
                // for feeding into the animator.
                return (Growth * 4) - Mathf.Floor(Growth * 4);
            }
        }

        public float Health { get; private set; }

        void SumCount(List<PlantPartBehaviour> partList, out float sum, out int count)
        {
            sum = 0;
            count = 0;
            foreach (var plantPartBehaviour in partList)
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

        public void TickGrowth(float water, float nutrition, float sunlight)
        {

            timer.Update();

            var growthFactorRaw = Definition.ComputeGrowth(sunlight, water, nutrition);
            growth = Mathf.Clamp01(growth + growthFactorRaw * Definition.GrowTimeInSeconds * timer.DeltaTime);

            var nextState = GetStateForRelativeTime(Growth);
            UpdatePlantState(nextState);

            animator.SetFloat(normalizedTimeId, NormalizedAnimatorTime);
        }

        public bool ChoosePlantPart(out PlantPartBehaviour b)
        {
            if (partsLinear.Count == 0)
            {
                b = null;
                return false;
            }

            var random = UnityEngine.Random.Range(0, partsLinear.Count);
            b = this.partsLinear[random];
            return true;
        }
    }
}