using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    private RenderTexture _target;
    private Camera _camera;
    public Texture SkyboxTexture;
    private uint _currentSample = 0;
    private Material _addMaterial;
    [Range(0,20)]
    public int _maxReflections;
    private int m_maxReflections;
    public Light DirectionalLight;

    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        if(transform.hasChanged){
            _currentSample = 0;
            transform.hasChanged = false;
        }
        
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height){
            _currentSample = 0;
        }

        if (_maxReflections != m_maxReflections){
            m_maxReflections = _maxReflections;
            _currentSample = 0;
        }

    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        SetShaderParameters();
        Render(dest);
    }

    private void Render(RenderTexture destination){
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        // print($"{threadGroupsX} x {threadGroupsY}");

        if(_addMaterial == null){
            _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        }
        _addMaterial.SetFloat("_Sample", _currentSample);

        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample++;
    }

    private void InitRenderTexture(){
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height){
            if (_target != null){
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }

    private void SetShaderParameters(){
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
        Vector3 l = DirectionalLight.transform.forward;
        RayTracingShader.SetInt("_MaxReflections", _maxReflections);
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(l.x, l.y, l.z, DirectionalLight.intensity));
    }
}
