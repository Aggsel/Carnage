using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/textureOverlay")]
public sealed class textureOverlay : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    //[Tooltip("Controls the intensity of the effect.")]
    //public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    Material m_Material;

    public bool IsActive() => m_Material != null && m_Texture.value != null && m_Texture != null; //&& intensity.value > 0f;
    public RenderTextureParameter m_Texture = new RenderTextureParameter(null);
    public ClampedFloatParameter threshold = new ClampedFloatParameter(0f, 0f, 1f);

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > HDRP Default Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Hidden/Shader/textureOverlay";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume textureOverlay is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        //m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_OverlayTexture", m_Texture.value);
        m_Material.SetTexture("_InputTexture", source);
        m_Material.SetFloat("_Threshold", threshold.value);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
