using UnityEngine;

public class GameOfLifeMaster : MonoBehaviour
{
    public ComputeShader GameOfLifeShader;
    private RenderTexture _target;
    public Texture InitStateTexture;
    public int _width;
    public int _height;

    private void OnRenderImage(RenderTexture src, RenderTexture dest){
        SetShaderParameters();
        Render(dest);
    }

    private void Render(RenderTexture destination){
        InitRenderTexture();

        GameOfLifeShader.SetTexture(0 , "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(_width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(_height / 8.0f);
        GameOfLifeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_target, destination);
    }

    private void InitRenderTexture(){
        if (_target == null || _target.width != _width || _target.height != _height){
            if (_target != null){
                _target.Release();
            }

            _target = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }

    private void SetShaderParameters(){
        GameOfLifeShader.SetFloat("_Width", (float)_width);
        GameOfLifeShader.SetFloat("_Height", (float)_height);
        GameOfLifeShader.SetBool("_DrawNextFrame", true); // TODO: control framerate from update
        if(_target == null){
            GameOfLifeShader.SetTexture(0, "_StateTexture", InitStateTexture);
        }else{
            GameOfLifeShader.SetTexture(0, "_StateTexture", _target.toTexture2D());
        }
    }
}

public static class ExtensionMethods{
    public static Texture toTexture2D(this RenderTexture rTex){
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0,0,rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }
}
