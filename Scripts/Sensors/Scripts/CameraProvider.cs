using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraProvider : MonoBehaviour
{
    [SerializeField]
    [Tooltip ("The desired vertical resolution in pixels")]
    protected uint ResolutionHeight;
    [SerializeField]
    [Tooltip ("The desired horizontal resolution in pixels")]
    protected uint ResolutionWidth;
    [SerializeField]
    [Tooltip ("The Camera this script will use to capture data")]
    protected Camera Camera;

    protected Rect Rect;
    protected Texture2D CapturedImage;
    protected RenderTexture CapturedTexture;
    protected byte[] ImageData;

    public virtual void Start()
    {
        Rect = new Rect(0, 0, ResolutionWidth, ResolutionHeight);
        CapturedTexture = new RenderTexture((int)ResolutionWidth, (int)ResolutionHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        CapturedTexture.Create();
        CapturedImage = new Texture2D((int)ResolutionWidth, (int)ResolutionHeight, TextureFormat.RGB24, false);
    }
    
    private void Update()
    {
        if(ToolsTime.DeltaTime > 0)
            CaptureImage();
    }

    public abstract void CaptureImage();

    protected void renderTextureTo2DTexture()
    {
        RenderTexture.active = CapturedTexture;
        CapturedImage.ReadPixels(Rect, 0, 0);
        CapturedImage.Apply();
    }

    public uint GetWidth()
    {
        return ResolutionWidth;
    }

    public uint GetHeight()
    {
        return ResolutionHeight;
    }

    public byte[] GetRawData()
    {
        return ImageData;
    }

}
