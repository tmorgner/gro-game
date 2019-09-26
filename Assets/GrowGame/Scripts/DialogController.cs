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

        [Button]
        public virtual void ShowDialog()
        {
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