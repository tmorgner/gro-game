using UnityEngine.SceneManagement;

namespace RoboRyanTron.SceneReference
{
    public static class SceneReferenceExtensions
    {
        public static void LoadScene(this SceneReference r)
        {
            SceneManager.LoadScene(r.SceneName);
        }
    }
}