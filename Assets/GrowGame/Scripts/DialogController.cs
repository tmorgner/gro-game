using System;
using NaughtyAttributes;
using UnityEditor;
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
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif

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

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif
            Time.timeScale = 0;
        }

        [Button]
        public virtual void HideDialog()
        {
            dialogOpenCount -= 1;

            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif
            Time.timeScale = 1;
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