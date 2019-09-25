using System;
using GrowGame.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GrowGame
{
    public class PlantPartBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlantPart part;

        [SerializeField]
        private UnityEvent onHarmed;

        [SerializeField]
        private UnityEvent onDestroyed;

        [ShowNonSerializedField]
        private float health;

        [ShowNonSerializedField]
        private PlantBehaviour plantBehaviour;

        [ShowNonSerializedField]
        private PlantDefinition plantDefinition;

        public PlantPart Part => part;

        public UnityEvent OnHarmed => onHarmed;

        public UnityEvent OnDestroyed => onDestroyed;

        private void Awake()
        {
            health = 1;
        }

        private void Start()
        {
            plantBehaviour = GetComponentInParent<PlantBehaviour>();
            if (plantBehaviour)
            {
                plantDefinition = plantBehaviour.Definition;
            }
        }

        public float Health => health;

        public void OnDamageReceived(float damage)
        {
            health = Mathf.Clamp01(health - damage);
            onHarmed?.Invoke();

            if (health <= 0)
            {
                onDestroyed?.Invoke();
            }
        }
    }
}