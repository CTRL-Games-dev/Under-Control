using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using PostProcessAttribute = UnityEngine.Rendering.PostProcessing.PostProcessAttribute;
using System;
[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.AfterStack, "Outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    public FloatParameter thickness = new FloatParameter { value = 1f };
    public FloatParameter depthMin = new FloatParameter { value = 0f };
    public FloatParameter depthMax = new FloatParameter { value = 1f };
}
public class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Outline"));
        sheet.properties.SetFloat("_Thickness", settings.thickness);
        sheet.properties.SetFloat("_DepthMin", settings.depthMin);
        sheet.properties.SetFloat("_DepthMax", settings.depthMax);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}