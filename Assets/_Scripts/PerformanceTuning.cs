using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PerformanceTuning : MonoBehaviour
{
    public PostProcessVolume postVolume;

    private Camera cam;
    private PostProcessLayer postProcessing;
    private PostProcessProfile postProfile;
    private Bloom fx_bloom;
    private ChromaticAberration fx_chromaticAberration;
    private Trive.Rendering.StochasticReflections fx_stochasticReflections;
    private MotionBlur fx_motionBlur;
    private Aerobox.Rendering.PostProcessing.Flare fx_flare;
    private AmplifyOcclusionEffect amplifyAO;
    private MiniEngineAO.AmbientOcclusion miniEngineAO;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.forceIntoRenderTexture = false;
        QualitySettings.maxQueuedFrames = 1;

        postProcessing = GetComponent<PostProcessLayer>();
        postProfile = postVolume.profile;
        postProfile.TryGetSettings<Bloom>(out fx_bloom);
        postProfile.TryGetSettings<ChromaticAberration>(out fx_chromaticAberration);
        postProfile.TryGetSettings<Trive.Rendering.StochasticReflections>(out fx_stochasticReflections);
        postProfile.TryGetSettings<MotionBlur>(out fx_motionBlur);
        postProfile.TryGetSettings<Aerobox.Rendering.PostProcessing.Flare>(out fx_flare);

        amplifyAO = GetComponent<AmplifyOcclusionEffect>();
        miniEngineAO = GetComponent<MiniEngineAO.AmbientOcclusion>();
    }
}