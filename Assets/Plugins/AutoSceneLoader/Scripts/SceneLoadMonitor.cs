using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RabbitStewdio.Unity.AutoSceneLoader
{
    [CreateAssetMenu(menuName = "Tools/Scene Load Monitor")]
    public class SceneLoadMonitor : ScriptableObject
    {
        readonly HashSet<Scene> scenes;
        readonly HashSet<string> scenesByPath;

        public SceneLoadMonitor()
        {
            scenes = new HashSet<Scene>();
            scenesByPath = new HashSet<string>();
        }

        public bool IsLoaded(Scene s)
        {
            return scenes.Contains(s);
        }

        public bool IsLoaded(string scenePath)
        {
            return scenesByPath.Contains(scenePath);
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnLoaded;

            scenes.Clear();
        }

        void OnEnable()
        {
            scenes.Clear();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        void OnSceneUnLoaded(Scene arg0)
        {
            scenes.Remove(arg0);
            scenesByPath.Remove(arg0.path);
        }

        void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            scenesByPath.Add(arg0.path);
            scenes.Add(arg0);
        }
    }
}