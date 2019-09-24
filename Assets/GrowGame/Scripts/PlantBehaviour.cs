using System.Collections.Generic;
using UnityEngine;

namespace Assets.GrowGame.Scripts.Data
{
    public class PlantBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlantDefinition definition;

        private readonly Dictionary<PlantPart, List<PlantPartBehaviour>> parts;

        public PlantBehaviour()
        {
            parts = new Dictionary<PlantPart, List<PlantPartBehaviour>>();
        }

        protected virtual void Awake()
        {
            var rawParts = GetComponentsInChildren<PlantPartBehaviour>();
            foreach (var p in rawParts)
            {
                if (parts.TryGetValue(p.Part, out var list))
                {
                    list.Add(p);
                }
                else
                {
                    parts.Add(p.Part, new List<PlantPartBehaviour>() {p});
                }
            }
        }
    }
}