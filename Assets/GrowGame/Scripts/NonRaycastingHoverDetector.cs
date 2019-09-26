using UnityEngine;
using UnityEngine.Events;

namespace GrowGame
{
    public class NonRaycastingHoverDetector : MonoBehaviour
    {
        [SerializeField] UnityEvent pointerEntered;
        [SerializeField] private UnityEvent pointerExited;

        public UnityEvent PointerEntered => pointerEntered;

        public UnityEvent PointerExited => pointerExited;

        private bool hovered;

        public bool Hovered
        {
            get => hovered;
            set
            {
                if (hovered == value)
                {
                    return;
                }

                hovered = value;
                if (hovered)
                {
                    PointerEntered?.Invoke();
                }
                else
                {
                    PointerExited?.Invoke();
                }
            }
        }

        private void Update()
        {
            if (DialogController.IsDialogOpen)
            {
                return;
            }

            var rb = transform as RectTransform;

            // Undocumented: Camera as null means use ScreenSpace overlay. Gotta love Unity's documentation!
            Hovered = RectTransformUtility.RectangleContainsScreenPoint(rb, Input.mousePosition, null);
        }
    }
}