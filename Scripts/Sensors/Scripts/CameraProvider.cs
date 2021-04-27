// # MIT License

// # Copyright (C) 2018-2021  CrowdBot H2020 European project
// # Inria Rennes Bretagne Atlantique - Rainbow - Julien Pettré

// # Permission is hereby granted, free of charge, to any person obtaining
// # a copy of this software and associated documentation files (the
// # "Software"), to deal in the Software without restriction, including
// # without limitation the rights to use, copy, modify, merge, publish,
// # distribute, sublicense, and/or sell copies of the Software, and to
// # permit persons to whom the Software is furnished to do so, subject
// # to the following conditions:

// # The above copyright notice and this permission notice shall be
// # included in all copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// # OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// # LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
