using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.Universal
{
    [ExecuteAlways]
    public class CustomRenderManager : MonoBehaviour
    {
        public static void CollectRenderersFromCurrentScene()
        {
            if (s_Instance == null)
            {
                return;
            }

            s_Instance.CollectRenderersFromCurrentSceneInternal();
        }

        public static void SortRenderers()
        {
            if (s_Instance == null)
            {
                return;
            }
        }

        public static List<Renderer> GetCollectedRenderers()
        {
            if (s_Instance == null)
            {
                return null;
            }

            return s_Instance._AllRenderers;
        }

        [SerializeField] private List<Renderer> _AllRenderers = new List<Renderer>();

        private void OnEnable()
        {
            if (s_Instance != null)
            {
                return;
            }

            s_Instance = this;
        }

        private void OnDisable()
        {
            if (s_Instance != this)
            {
                return;
            }

            s_Instance = null;
        }

#if UNITY_EDITOR
        [ContextMenu("Collect Renderers")]
#endif
        private void CollectRenderersFromCurrentSceneInternal()
        {
            _AllRenderers.Clear();

            var rootGOs = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGO in rootGOs)
            {
                _AllRenderers.AddRange(rootGO.GetComponentsInChildren<Renderer>());
            }
        }

        private static CustomRenderManager s_Instance = null;
    }
}