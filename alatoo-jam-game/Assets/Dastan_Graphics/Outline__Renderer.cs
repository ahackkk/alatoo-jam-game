using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{
    [System.Serializable]
    public class OutlineSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material outlineMaterial;
    }

    public OutlineSettings settings = new OutlineSettings();
    private FullPass renderPass;

    public override void Create() {
        renderPass = new FullPass(settings.outlineMaterial);
        renderPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (settings.outlineMaterial != null) {
            renderer.EnqueuePass(renderPass);
        }
    }

    class FullPass : ScriptableRenderPass {
        private Material mat;
        public FullPass(Material m) { mat = m; }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (mat == null) return;
            CommandBuffer cmd = CommandBufferPool.Get("ScreenSpaceOutline");
            
            // Используем современный метод отрисовки для URP
            Blit(cmd, ref renderingData, mat);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
