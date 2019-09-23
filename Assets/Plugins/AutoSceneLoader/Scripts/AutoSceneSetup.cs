using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RabbitStewdio.Unity.AutoSceneLoader
{
    public class AutoSceneSetup : MonoBehaviour
    {
        public SceneLoadMonitor monitor;
        public List<SceneReference> Scenes;
        public bool Async;
        public float progress;

        public void Start()
        {
            Debug.Log("Auto scene loader started: ");
            StartCoroutine(LoadAll());
        }

        IEnumerator LoadAll()
        {
            yield return new WaitForFixedUpdate();

            var scenes = new List<SceneReference>();
            foreach (var sceneReference in Scenes)
            {
                if (!monitor.IsLoaded(sceneReference.SceneName))
                {
                    scenes.Add(sceneReference);
                    Debug.Log("Auto scene loader will load " + sceneReference.SceneName);
                }
            }

            if (Async)
            {
                StartCoroutine(LoadScenesAsync(scenes));
            }
            else
            {
                var timeScale = Time.timeScale;
                Time.timeScale = 0;
                try
                {
                    foreach (var sr in scenes)
                    {
                        if (sr == null || string.IsNullOrEmpty(sr.SceneName))
                        {
                            continue;
                        }
                        SceneManager.LoadScene(sr.SceneName, LoadSceneMode.Additive);
                    }
                }
                finally
                {
                    Time.timeScale = timeScale;

                }
            }
        }

        IEnumerator LoadScenesAsync(List<SceneReference> scenes)
        {
            var timeScale = Time.timeScale;
            Time.timeScale = 0;
            try
            {
                var sceneHandles = scenes.Where(s => s != null && !string.IsNullOrEmpty(s.SceneName))
                                         .Select(s => SceneManager.LoadSceneAsync(s.SceneName, LoadSceneMode.Additive))
                                         .ToList();
                var sceneCount = sceneHandles.Count;
                while (sceneHandles.Count > 0)
                {
                    yield return null;
                    var currentProgress = 0f;
                    foreach (var asyncOperation in sceneHandles)
                    {
                        if (asyncOperation.isDone)
                        {
                            sceneHandles.Remove(asyncOperation);
                        }
                        else
                        {
                            currentProgress += asyncOperation.progress;
                        }
                    }

                    progress = currentProgress / sceneCount;
                }
            }
            finally
            {
                Time.timeScale = timeScale;
            }

            progress = 1;
        }
    }
}