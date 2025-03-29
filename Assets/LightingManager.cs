using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

public class LightingManager : MonoBehaviour
{
    [SerializeField] ARCameraManager _cameraManager;
    [SerializeField] Light _lightEstimationSource;
    [SerializeField] ARTrackedImageManager _trackedImageManager;
    static readonly int _colorShaderId = Shader.PropertyToID("_Color");

    MeshRenderer[] _meshes;
    Color _color = Color.black;
    Color _colorTarget = Color.black;
    // static readonly int _lightDirShaderId = Shader.PropertyToID("_LightDir");
    //
    // static readonly Vector3 _shadowPlane = Vector3.up;
    // static readonly Vector3 _lightDir = new Vector3(0.02f, -1.0f, -0.02f);
    // float _shadowPlaneOffset = 0.0f;
    // Color _shadowColor;
    // static readonly int _shadowColorShaderId = Shader.PropertyToID("_ShadowColor");
    // static readonly int _shadowMatrixShaderId = Shader.PropertyToID("_ShadowMatrix");
    float _brightnessMultiplier = 2.3f;
    float _lightEstimationLerpSpeed = 1;

    public void Start()
    {
        _meshes = _trackedImageManager.trackedImagePrefab.GetComponentsInChildren<MeshRenderer>();
        Assert.IsNotNull(_meshes, "Tracked Image Prefab does not have a MeshRenderer");

        _cameraManager.frameReceived += OnFrameReceived;
        // SetupShadowSettings();
        Assert.IsNotNull(_lightEstimationSource, "Light Estimation Source is null. It's needed to make AR lighting more realistic by estimating the direction of irl lights");
        _lightEstimationSource.useColorTemperature = true;
    }

    // void SetupShadowSettings()
    // {
    //     _shadowColor = new Color(0, 0, 0, 1);
    // }

    void Update()
    {
        LerpAndApplyLightingAndShadows();
    }

    void LerpAndApplyLightingAndShadows()
    {
        _color = Color.Lerp(_color, _colorTarget, _lightEstimationLerpSpeed * Time.deltaTime);
        // Shader.SetGlobalColor(_albedoShaderId, new Color(_albedoColor.r, _albedoColor.g, _albedoColor.b, 1));
        foreach (var mesh in _meshes)
        {
            mesh.sharedMaterial.SetColor(_colorShaderId, _color);
            mesh.sharedMaterial.SetColor("_SpecColor", _color);
            mesh.sharedMaterial.SetFloat("_Glossiness", _lightEstimationSource.intensity);
        }
        // Shader.SetGlobalVector(_lightDirShaderId, new Vector3(0, 1, 0).normalized);

        // var shadowMatrix = GetShadowMatrix(_shadowPlane, _lightDir, _shadowPlaneOffset);
        // Shader.SetGlobalMatrix(_shadowMatrixShaderId, shadowMatrix);
        // Shader.SetGlobalColor(_shadowColorShaderId, _shadowColor);
    }

    // public static Matrix4x4 GetShadowMatrix(Vector3 shadowPlaneNorm, Vector3 lightVal, float shadowPlaneOffset)
    // {
    //     lightVal = lightVal.normalized;
    //     var lightVec = new Vector4(lightVal.x, lightVal.y, lightVal.z, 0.0f);
    //     var shadowPlane = new Vector4(shadowPlaneNorm.x, shadowPlaneNorm.y, shadowPlaneNorm.z, -shadowPlaneOffset);
    //     var dot = -Vector4.Dot(shadowPlane, lightVec);
    //
    //     var shadowMat = Matrix4x4.identity;
    //     shadowMat.m00 = shadowPlane.x * lightVec.x + dot;
    //     shadowMat.m10 = shadowPlane.x * lightVec.y;
    //     shadowMat.m20 = shadowPlane.x * lightVec.z;
    //     shadowMat.m30 = shadowPlane.x * lightVec.w;
    //
    //     shadowMat.m01 = shadowPlane.y * lightVec.x;
    //     shadowMat.m11 = shadowPlane.y * lightVec.y + dot;
    //     shadowMat.m21 = shadowPlane.y * lightVec.z;
    //     shadowMat.m31 = shadowPlane.y * lightVec.w;
    //
    //     shadowMat.m02 = shadowPlane.z * lightVec.x;
    //     shadowMat.m12 = shadowPlane.z * lightVec.y;
    //     shadowMat.m22 = shadowPlane.z * lightVec.z + dot;
    //     shadowMat.m32 = shadowPlane.z * lightVec.w;
    //
    //     shadowMat.m03 = shadowPlane.w * lightVec.x;
    //     shadowMat.m13 = shadowPlane.w * lightVec.y;
    //     shadowMat.m23 = shadowPlane.w * lightVec.z;
    //     shadowMat.m33 = shadowPlane.w * lightVec.w + dot;
    //
    //     return shadowMat;
    // }

    void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        HandleLighting(args);
    }

    void HandleLighting(ARCameraFrameEventArgs args)
    {
        _lightEstimationSource.intensity = args.lightEstimation.averageBrightness ?? 1;
        _lightEstimationSource.colorTemperature = args.lightEstimation.averageColorTemperature ?? 5800;

        _colorTarget = ColorTemperatureToRGB(_lightEstimationSource.colorTemperature) * _lightEstimationSource.intensity * _brightnessMultiplier;
    }

    public static Color ColorTemperatureToRGB(float kelvin)
    {
        var rounded = ((int)kelvin / 100) * 100;
        return rounded switch
        {
            < 100 => new Color(1.00f, 0.21f, 0.00f, 1f),
            100 => new Color(1.00f, 0.21f, 0.00f, 1f),
            200 => new Color(1.00f, 0.22f, 0.00f, 1f),
            300 => new Color(1.00f, 0.28f, 0.00f, 1f),
            400 => new Color(1.00f, 0.33f, 0.00f, 1f),
            500 => new Color(1.00f, 0.36f, 0.00f, 1f),
            600 => new Color(1.00f, 0.40f, 0.00f, 1f),
            700 => new Color(1.00f, 0.43f, 0.00f, 1f),
            800 => new Color(1.00f, 0.45f, 0.00f, 1f),
            900 => new Color(1.00f, 0.47f, 0.00f, 1f),
            1000 => new Color(1.00f, 0.49f, 0.00f, 1f),
            1100 => new Color(1.00f, 0.51f, 0.00f, 1f),
            1200 => new Color(1.00f, 0.54f, 0.07f, 1f),
            1300 => new Color(1.00f, 0.56f, 0.13f, 1f),
            1400 => new Color(1.00f, 0.58f, 0.17f, 1f),
            1500 => new Color(1.00f, 0.60f, 0.21f, 1f),
            1600 => new Color(1.00f, 0.62f, 0.25f, 1f),
            1700 => new Color(1.00f, 0.63f, 0.28f, 1f),
            1800 => new Color(1.00f, 0.65f, 0.31f, 1f),
            1900 => new Color(1.00f, 0.66f, 0.34f, 1f),
            2000 => new Color(1.00f, 0.68f, 0.37f, 1f),
            2100 => new Color(1.00f, 0.69f, 0.40f, 1f),
            2200 => new Color(1.00f, 0.71f, 0.42f, 1f),
            2300 => new Color(1.00f, 0.72f, 0.45f, 1f),
            2400 => new Color(1.00f, 0.73f, 0.47f, 1f),
            2500 => new Color(1.00f, 0.75f, 0.49f, 1f),
            2600 => new Color(1.00f, 0.76f, 0.52f, 1f),
            2700 => new Color(1.00f, 0.77f, 0.54f, 1f),
            2800 => new Color(1.00f, 0.78f, 0.56f, 1f),
            2900 => new Color(1.00f, 0.79f, 0.58f, 1f),
            3000 => new Color(1.00f, 0.80f, 0.60f, 1f),
            3100 => new Color(1.00f, 0.81f, 0.62f, 1f),
            3200 => new Color(1.00f, 0.82f, 0.64f, 1f),
            3300 => new Color(1.00f, 0.83f, 0.66f, 1f),
            3400 => new Color(1.00f, 0.84f, 0.68f, 1f),
            3500 => new Color(1.00f, 0.84f, 0.69f, 1f),
            3600 => new Color(1.00f, 0.85f, 0.71f, 1f),
            3700 => new Color(1.00f, 0.86f, 0.73f, 1f),
            3800 => new Color(1.00f, 0.87f, 0.75f, 1f),
            3900 => new Color(1.00f, 0.87f, 0.76f, 1f),
            4000 => new Color(1.00f, 0.88f, 0.78f, 1f),
            4100 => new Color(1.00f, 0.89f, 0.79f, 1f),
            4200 => new Color(1.00f, 0.89f, 0.81f, 1f),
            4300 => new Color(1.00f, 0.90f, 0.82f, 1f),
            4400 => new Color(1.00f, 0.91f, 0.84f, 1f),
            4500 => new Color(1.00f, 0.91f, 0.85f, 1f),
            4600 => new Color(1.00f, 0.92f, 0.86f, 1f),
            4700 => new Color(1.00f, 0.93f, 0.88f, 1f),
            4800 => new Color(1.00f, 0.93f, 0.89f, 1f),
            4900 => new Color(1.00f, 0.94f, 0.90f, 1f),
            5000 => new Color(1.00f, 0.94f, 0.91f, 1f),
            5100 => new Color(1.00f, 0.95f, 0.93f, 1f),
            5200 => new Color(1.00f, 0.95f, 0.94f, 1f),
            5300 => new Color(1.00f, 0.96f, 0.95f, 1f),
            5400 => new Color(1.00f, 0.96f, 0.96f, 1f),
            5500 => new Color(1.00f, 0.96f, 0.97f, 1f),
            5600 => new Color(1.00f, 0.97f, 0.98f, 1f),
            5700 => new Color(1.00f, 0.98f, 0.99f, 1f),
            5800 => new Color(1.00f, 0.98f, 1.00f, 1f),
            5900 => new Color(0.99f, 0.97f, 1.00f, 1f),
            6000 => new Color(0.98f, 0.96f, 1.00f, 1f),
            6100 => new Color(0.97f, 0.96f, 1.00f, 1f),
            6200 => new Color(0.96f, 0.95f, 1.00f, 1f),
            6300 => new Color(0.95f, 0.95f, 1.00f, 1f),
            6400 => new Color(0.94f, 0.95f, 1.00f, 1f),
            6500 => new Color(0.94f, 0.94f, 1.00f, 1f),
            6600 => new Color(0.93f, 0.94f, 1.00f, 1f),
            6700 => new Color(0.92f, 0.93f, 1.00f, 1f),
            6800 => new Color(0.91f, 0.93f, 1.00f, 1f),
            6900 => new Color(0.91f, 0.93f, 1.00f, 1f),
            7000 => new Color(0.90f, 0.92f, 1.00f, 1f),
            7100 => new Color(0.89f, 0.92f, 1.00f, 1f),
            7200 => new Color(0.89f, 0.91f, 1.00f, 1f),
            7300 => new Color(0.88f, 0.91f, 1.00f, 1f),
            7400 => new Color(0.88f, 0.91f, 1.00f, 1f),
            7500 => new Color(0.87f, 0.90f, 1.00f, 1f),
            7600 => new Color(0.87f, 0.90f, 1.00f, 1f),
            7700 => new Color(0.86f, 0.90f, 1.00f, 1f),
            7800 => new Color(0.85f, 0.90f, 1.00f, 1f),
            7900 => new Color(0.85f, 0.89f, 1.00f, 1f),
            8000 => new Color(0.85f, 0.89f, 1.00f, 1f),
            8100 => new Color(0.84f, 0.89f, 1.00f, 1f),
            8200 => new Color(0.84f, 0.88f, 1.00f, 1f),
            8300 => new Color(0.83f, 0.88f, 1.00f, 1f),
            8400 => new Color(0.83f, 0.88f, 1.00f, 1f),
            8500 => new Color(0.82f, 0.87f, 1.00f, 1f),
            8600 => new Color(0.82f, 0.87f, 1.00f, 1f),
            8700 => new Color(0.82f, 0.87f, 1.00f, 1f),
            8800 => new Color(0.81f, 0.87f, 1.00f, 1f),
            8900 => new Color(0.81f, 0.87f, 1.00f, 1f),
            9000 => new Color(0.81f, 0.86f, 1.00f, 1f),
            9100 => new Color(0.80f, 0.86f, 1.00f, 1f),
            9200 => new Color(0.81f, 0.85f, 1.00f, 1f),
            9300 => new Color(0.81f, 0.85f, 1.00f, 1f),
            9400 => new Color(0.81f, 0.85f, 1.00f, 1f),
            9500 => new Color(0.80f, 0.85f, 1.00f, 1f),
            9600 => new Color(0.80f, 0.85f, 1.00f, 1f),
            9700 => new Color(0.80f, 0.85f, 1.00f, 1f),
            9800 => new Color(0.80f, 0.84f, 1.00f, 1f),
            9900 => new Color(0.79f, 0.84f, 1.00f, 1f),
            10000 => new Color(0.79f, 0.84f, 1.00f, 1f),
            10100 => new Color(0.79f, 0.84f, 1.00f, 1f),
            10200 => new Color(0.78f, 0.84f, 1.00f, 1f),
            10300 => new Color(0.78f, 0.84f, 1.00f, 1f),
            10400 => new Color(0.78f, 0.83f, 1.00f, 1f),
            10500 => new Color(0.78f, 0.83f, 1.00f, 1f),
            10600 => new Color(0.78f, 0.83f, 1.00f, 1f),
            10700 => new Color(0.77f, 0.83f, 1.00f, 1f),
            10800 => new Color(0.77f, 0.83f, 1.00f, 1f),
            10900 => new Color(0.77f, 0.82f, 1.00f, 1f),
            11000 => new Color(0.76f, 0.82f, 1.00f, 1f),
            11100 => new Color(0.75f, 0.81f, 1.00f, 1f),
            11200 => new Color(0.74f, 0.80f, 1.00f, 1f),
            11300 => new Color(0.73f, 0.79f, 1.00f, 1f),
            11400 => new Color(0.72f, 0.78f, 1.00f, 1f),
            11500 => new Color(0.71f, 0.77f, 1.00f, 1f),
            11600 => new Color(0.70f, 0.76f, 1.00f, 1f),
            11700 => new Color(0.69f, 0.75f, 1.00f, 1f),
            11800 => new Color(0.68f, 0.74f, 1.00f, 1f),
            11900 => new Color(0.67f, 0.73f, 1.00f, 1f),
            12000 => new Color(0.66f, 0.72f, 1.00f, 1f),
            12100 => new Color(0.65f, 0.71f, 1.00f, 1f),
            12200 => new Color(0.64f, 0.70f, 1.00f, 1f),
            12300 => new Color(0.63f, 0.69f, 1.00f, 1f),
            12400 => new Color(0.62f, 0.68f, 1.00f, 1f),
            12500 => new Color(0.61f, 0.67f, 1.00f, 1f),
            12600 => new Color(0.60f, 0.66f, 1.00f, 1f),
            12700 => new Color(0.59f, 0.65f, 1.00f, 1f),
            12800 => new Color(0.58f, 0.64f, 1.00f, 1f),
            12900 => new Color(0.57f, 0.63f, 1.00f, 1f),
            > 12900 => new Color(0.43f, 0.54f, 1.00f, 1f),
        };
    }

    private void OnDestroy()
    {
        _cameraManager.frameReceived -= OnFrameReceived;
    }
}
