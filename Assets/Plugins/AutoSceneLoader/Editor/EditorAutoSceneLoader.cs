using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RabbitStewdio.Unity.AutoSceneLoader.Editor
{
    [InitializeOnLoad]
    public class EditorAutoSceneLoader
    {
        const string AutoLoadToggleMenuName = "Tools/Automatically Load Dependent Scenes";

        static volatile bool scenesLoadPending;

        static EditorAutoSceneLoader()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
            EditorApplication.update += OnUpdate;

            for (var s = 0; s < EditorSceneManager.loadedSceneCount; s += 1)
            {
                var scene = SceneManager.GetSceneAt(s);
                OnSceneOpened(scene, OpenSceneMode.Single);
            }
        }

        public static bool IsAutoLoadDependentScenes()
        {
            var isAutoLoadDependentScenes = EditorPrefs.GetBool("VTKitchenSimulator.Autoloader.Enabled", false);
            Menu.SetChecked(AutoLoadToggleMenuName, isAutoLoadDependentScenes);
            return isAutoLoadDependentScenes;
        }

        [MenuItem(AutoLoadToggleMenuName, false)]
        public static void ToggleAutoLoadDependentScenes()
        {
            EditorPrefs.SetBool("VTKitchenSimulator.Autoloader.Enabled", !IsAutoLoadDependentScenes());
            Menu.SetChecked(AutoLoadToggleMenuName, IsAutoLoadDependentScenes());
        }

        static void Log(string log)
        {
            if (true)
            {
                return;
            }
#pragma warning disable 162
            Debug.Log("[EditorAutoSceneLoader] " + log);
#pragma warning restore 162
        }

        static bool IsSceneLoaded(string path)
        {
            var count = EditorSceneManager.loadedSceneCount;
            for (var s = 0; s < count; s += 1)
            {
                var scene = SceneManager.GetSceneAt(s);
                if (string.Equals(scene.path, path))
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("Tools/Load Dependent Scenes")]
        public static void LoadDependentScenes()
        {
            var setup = Object.FindObjectsOfType<AutoSceneSetup>();
            foreach (var autoSceneSetup in setup)
            {
                Debug.Log("Found " + autoSceneSetup);
                var scenesToLoad = autoSceneSetup.Scenes;
                if (scenesToLoad != null)
                {
                    foreach (var s in scenesToLoad)
                    {
                        if (s == null || string.IsNullOrEmpty(s.SceneName))
                        {
                            continue;
                        }
                        else if (IsSceneLoaded(s.SceneName))
                        {
                            Log("Scene " + s.SceneName + " is already loaded.");
                            continue;
                        }

                        Log("Scene " + s.SceneName + " will be loaded ..");
                        EditorSceneManager.OpenScene(s.SceneName, OpenSceneMode.Additive);
                    }
                }
            }
        }

        static void OnUpdate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (scenesLoadPending)
            {
                if (IsAutoLoadDependentScenes())
                {
                    Log("Will load dependent scenes now ..");
                    LoadDependentScenes();
                }
                else
                {
                    Log("Wont load scenes");
                }

                scenesLoadPending = false;
            }
        }

        static void OnSceneClosed(Scene scene)
        {
            Log("Closed editor scene " + scene.path);
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (Application.isPlaying)
            {
                return;
            }

            Log("Opened editor scene " + scene.path);
            scenesLoadPending = true;
            Log("PENDING Opened editor scene " + scene.path + " " + scenesLoadPending);
        }
    }
}