using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Aerobox.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FlareRenderer), PostProcessEvent.BeforeStack, "Aerobox/Flare")]
    public sealed class Flare : PostProcessEffectSettings
    {
        //const int MAX_DOWNSAMPLE = 1;
        public TextureParameter spectralLut = new TextureParameter();

        [Header("Resolutions (Apply on start)")]
        public IntParameter Res1 = new IntParameter() { value = 1 }; //4
        public IntParameter Res2 = new IntParameter() { value = 1 }; //4
        public IntParameter Res3 = new IntParameter() { value = 1 }; //4
        public IntParameter ResDownsampled = new IntParameter() { value = 1 }; //2
        public IntParameter ResRadialWarped = new IntParameter() { value = 1 }; //5
        public IntParameter ResGhosts = new IntParameter() { value = 1 }; //4
        public IntParameter ChromaticAberration = new IntParameter() { value = 1 }; //2-1

        [Header("Parameters")]
        [Range(0, 500)]
        public FloatParameter Intensity1 = new FloatParameter() { value = 4 };
        [Range(0, 500)]
        public FloatParameter Intensity2 = new FloatParameter() { value = 1 };

        [Space]
        public BoolParameter debugView = new BoolParameter();
    }

    public sealed class FlareRenderer : PostProcessEffectRenderer<Flare>
    {
        private const int MAX_DOWNSAMPLE = 1;
        private RenderTexture cSource;
        private RenderTexture[] renderTextures, renderTextures1;
        private RenderTexture downsampled, radialWarped, ghosts, aberration;

        public override void Init()
        {
            cSource = new RenderTexture(Screen.width / settings.Res1, Screen.height / settings.Res1, 0, RenderTextureFormat.RGB111110Float);
            cSource.Create();

            renderTextures = new RenderTexture[MAX_DOWNSAMPLE];
            for (int i = 0; i < MAX_DOWNSAMPLE; i++)
            {
                renderTextures[i] = new RenderTexture(Mathf.Max(Screen.width / settings.Res2 >> i, 1), Mathf.Max(Screen.height / settings.Res2 >> i, 1), 0, RenderTextureFormat.RGB111110Float);
                renderTextures[i].Create();
            }

            renderTextures1 = new RenderTexture[MAX_DOWNSAMPLE];
            for (int i = 0; i < MAX_DOWNSAMPLE; i++)
            {
                renderTextures1[i] = new RenderTexture(Mathf.Max(Screen.width / settings.Res3 >> i, 1), Mathf.Max(Screen.height / settings.Res3 >> i, 1), 0, RenderTextureFormat.ARGB32);
                renderTextures1[i].Create();
            }

            downsampled = new RenderTexture(Mathf.Max(Screen.width / settings.ResDownsampled >> MAX_DOWNSAMPLE, 4),
                Mathf.Max(Screen.height / settings.ResDownsampled >> MAX_DOWNSAMPLE, 1), 0, RenderTextureFormat.RGB111110Float);
            downsampled.Create();

            radialWarped = new RenderTexture(Mathf.Max(Screen.width / settings.ResRadialWarped >> MAX_DOWNSAMPLE, 1),
                Mathf.Max(Screen.height / settings.ResRadialWarped >> MAX_DOWNSAMPLE, 1), 0, RenderTextureFormat.ARGB32);
            radialWarped.Create();

            ghosts = new RenderTexture(Mathf.Max(Screen.width / settings.ResGhosts >> MAX_DOWNSAMPLE, 1),
                Mathf.Max(Screen.height / settings.ResGhosts >> MAX_DOWNSAMPLE, 1), 0, RenderTextureFormat.ARGB32);
            ghosts.Create();

            aberration = new RenderTexture(Mathf.Max(Screen.width / settings.ChromaticAberration >> MAX_DOWNSAMPLE, 1),
                Mathf.Max(Screen.height / settings.ChromaticAberration >> MAX_DOWNSAMPLE, 1), 0, RenderTextureFormat.ARGB32);
            aberration.Create();
        }

        public override void Release()
        {
            if (cSource) cSource.Release();
            if (renderTextures != null)
                for (int i = 0; i < MAX_DOWNSAMPLE; i++)
                {
                    if (renderTextures[i])
                        renderTextures[i].Release();
                }
            if (renderTextures1 != null)
                for (int i = 0; i < MAX_DOWNSAMPLE; i++)
                {
                    if (renderTextures1[i])
                        renderTextures1[i].Release();
                }
            if (downsampled != null) downsampled.Release();
            if (radialWarped != null) radialWarped.Release();
            if (ghosts != null) ghosts.Release();
            if (aberration != null) aberration.Release();
        }

        private const int BOX_UP_PASS = 0;
        private const int H_BLUR_PASS = 1;
        private const int V_BLUR_PASS = 2;
        //private const int LERP_PASS = 3;
        private const int RADIAL_WARP_PASS = 4;
        private const int GHOST_PASS = 5;
        private const int ADD_PASS = 6;
        private const int ABERRATION_PASS = 7;

        private const string _flare = "Hidden/Aerobox/Flare";
        private readonly Shader _shader = Shader.Find(_flare);
        private readonly int _DirtIntensity = Shader.PropertyToID("_DirtIntensity");
        private readonly int _AddTexture = Shader.PropertyToID("_AddTexture");
        private readonly int _Delta = Shader.PropertyToID("_Delta");
        private readonly int _Add = Shader.PropertyToID("_Add");
        private readonly int _ChromaticAberration_Spectrum = Shader.PropertyToID("_ChromaticAberration_Spectrum");

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(_shader);
            sheet.properties.SetFloat(_DirtIntensity, Mathf.Pow(MAX_DOWNSAMPLE, 3));

            context.command.BlitFullscreenTriangle(context.source, cSource);
            context.command.BlitFullscreenTriangle(cSource, renderTextures[0]);

            sheet.properties.SetFloat(_Delta, 1);
            for (int i = 1; i < MAX_DOWNSAMPLE; i++)
            {
                context.command.BlitFullscreenTriangle(renderTextures[i - 1], renderTextures1[i - 1], sheet, H_BLUR_PASS);
                context.command.BlitFullscreenTriangle(renderTextures1[i - 1], renderTextures[i], sheet, V_BLUR_PASS);
            }
            context.command.BlitFullscreenTriangle(renderTextures[MAX_DOWNSAMPLE - 1], downsampled);

            context.command.BlitFullscreenTriangle(downsampled, radialWarped, sheet, RADIAL_WARP_PASS);
            context.command.BlitFullscreenTriangle(downsampled, ghosts, sheet, GHOST_PASS);

            if (radialWarped == null) Init();
            sheet.properties.SetTexture(_AddTexture, radialWarped);
            sheet.properties.SetFloat(_Add, settings.Intensity1);
            context.command.BlitFullscreenTriangle(ghosts, aberration, sheet, ADD_PASS);
            
            if ((Texture)settings.spectralLut)
                sheet.properties.SetTexture(_ChromaticAberration_Spectrum, settings.spectralLut);
            context.command.BlitFullscreenTriangle(aberration, renderTextures[MAX_DOWNSAMPLE - 1], sheet, ABERRATION_PASS);
            context.command.BlitFullscreenTriangle(renderTextures[MAX_DOWNSAMPLE - 1], renderTextures1[MAX_DOWNSAMPLE - 1], sheet, H_BLUR_PASS);
            context.command.BlitFullscreenTriangle(renderTextures1[MAX_DOWNSAMPLE - 1], renderTextures[MAX_DOWNSAMPLE - 1], sheet, V_BLUR_PASS);

            sheet.properties.SetFloat(_Delta, 0.5f);
            for (int i = MAX_DOWNSAMPLE - 1; i > 0; i--)
            {
                context.command.BlitFullscreenTriangle(renderTextures[i], renderTextures[i - 1], sheet, BOX_UP_PASS);
            }
            context.command.BlitFullscreenTriangle(renderTextures[0], renderTextures1[0], sheet, BOX_UP_PASS);

            if (settings.debugView)
            {
                context.command.BlitFullscreenTriangle(renderTextures[0], context.destination);
                return;
            }

            context.command.BlitFullscreenTriangle(renderTextures1[0], renderTextures[0], sheet, V_BLUR_PASS);
            context.command.BlitFullscreenTriangle(renderTextures[0], renderTextures1[0], sheet, H_BLUR_PASS);
            
            //if (renderTextures1[0] == null)
            //{
            //    Init();
            //}
            sheet.properties.SetTexture(_AddTexture, renderTextures1[0]);
            sheet.properties.SetFloat(_Add, settings.Intensity2);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, ADD_PASS);
        }
    }
}