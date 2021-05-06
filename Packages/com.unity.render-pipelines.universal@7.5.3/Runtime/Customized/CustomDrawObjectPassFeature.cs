using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    public class CustomDrawObjectPassFeature : ScriptableRendererFeature
    {
        [SerializeField] private Settings _Settings = new Settings();

        public override void Create()
        {
            m_ScriptablePass = new CusomDrawObjectPass(_Settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (UniversalRenderPipeline.asset._UTKCustomized)
            {
                renderer.EnqueuePass(m_ScriptablePass);
            }            
        }

        private CusomDrawObjectPass m_ScriptablePass;

        [System.Serializable]
        private class Settings
        {
            public string PassTag = "RenderObjectsFeature";
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
            public int SubMeshIndex = 0;
            public int ShaderPassIndex = 0;
        }

        private class CusomDrawObjectPass : ScriptableRenderPass
        {
            public CusomDrawObjectPass(Settings settings)
            {
                m_ProfilerTag = settings.PassTag;
                m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
                m_SubMeshIndex = settings.SubMeshIndex;
                m_ShaderPassIndex = settings.ShaderPassIndex;
                this.renderPassEvent = settings.Event;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                using (new ProfilingScope(cmd, m_ProfilingSampler))
                {
                    var renderers = CustomRenderManager.GetCollectedRenderers();

                    if (renderers != null && renderers.Count > 0)
                    {
                        foreach (var renderer in renderers)
                        {
                            for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
                            {
                                var material = renderer.sharedMaterials[i];

                                cmd.DrawRenderer(renderer, material, i, m_ShaderPassIndex);
                            }
                        }
                    }
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            private string m_ProfilerTag;
            private ProfilingSampler m_ProfilingSampler;
            private int m_SubMeshIndex;
            private int m_ShaderPassIndex;
        }
    }
}