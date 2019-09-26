using System;
using NaughtyAttributes;
using UnityEngine;

namespace GrowGame
{
    public class DialogController: MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        private static int dialogOpenCount;

        public static bool IsDialogOpen => dialogOpenCount > 0;

        [ShowNativeProperty]
        public int DialogOpenCount => dialogOpenCount;

        private void Awake()
        {
            if (canvasGroup.interactable)
            {
                dialogOpenCount += 1;
            }
            else
            {
                dialogOpenCount = Math.Max(0, dialogOpenCount - 1);
            }
        }

        [Button]
        public virtual void ShowDialog()
        {
            if (dialogOpenCount > 0)
            {
                return;
            }

            dialogOpenCount += 1;
            
            Time.timeScale = 0;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        [Button]
        public virtual void HideDialog()
        {
            dialogOpenCount -= 1;

            Time.timeScale = 1;
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideDialog();
            }

        }
    }
}