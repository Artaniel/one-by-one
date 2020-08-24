using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "Custom/Blur")]
public sealed class Blur : PostProcessEffectSettings
{
    [Range(0f, 0.1f), Tooltip("Blur effect intensity.")]
    public FloatParameter blurSize = new FloatParameter { value = 0.05f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value && blurSize.value > 0f;
    }
}

public sealed class BlurRenderer : PostProcessEffectRenderer<Blur>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Blur"));
        sheet.properties.SetFloat("_BlurSize", settings.blurSize);
        int tex = Shader.PropertyToID("_MainTex");
        context.GetScreenSpaceTemporaryRT(context.command, tex);
        context.command.BlitFullscreenTriangle(context.source, tex, sheet, 0);
        context.command.BlitFullscreenTriangle(tex, context.destination, sheet, 1);
    }
}