using System;
using NaughtyAttributes;
using UnityEngine;

namespace GrowGame
{
    public class DialogController: MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Button]
        public void ShowDialog()
        {
            Time.timeScale = 0;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        [Button]
        public void HideDialog()
        {
            Time.timeScale = 1;
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideDialog();
            }

        }
    }
}