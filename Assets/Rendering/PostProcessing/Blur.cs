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
        var rt = context.GetScreenSpaceTemporaryRT();
        context.command.BlitFullscreenTriangle(context.source, rt, sheet, 0);
        context.command.BlitFullscreenTriangle(rt, context.destination, sheet, 1);
    }
}