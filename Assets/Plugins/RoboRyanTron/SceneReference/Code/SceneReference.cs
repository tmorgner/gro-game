// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/07/2018
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.SceneReference
{
    /// <summary>
    /// Class used to serialize a reference to a scene asset that can be used
    /// at runtime in a build, when the asset can no longer be directly
    /// referenced. This caches the scene name based on the SceneAsset to use
    /// at runtime to load.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        public SceneAsset Scene;
#endif

        [Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
        public string SceneName;

        [SerializeField]
        string sceneGuid;

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (Scene != null)
            {
                string sceneAssetPath = AssetDatabase.GetAssetPath(Scene);
                string sceneAssetGUID = AssetDatabase.AssetPathToGUID(sceneAssetPath);
                sceneGuid = sceneAssetGUID;
                SceneName = sceneAssetPath;
            }
            else
            {
                sceneGuid = null;
                SceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (ReferenceEquals(Scene, null))
            {
                if (!string.IsNullOrEmpty(SceneName))
                {
                    Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneName);
                }
            }

            if (ReferenceEquals(Scene, null) ||
                string.IsNullOrEmpty(SceneName) || 
                sceneGuid == null)
            {
                Scene = null;
                SceneName = null;
            }
#endif
        }
    }
}