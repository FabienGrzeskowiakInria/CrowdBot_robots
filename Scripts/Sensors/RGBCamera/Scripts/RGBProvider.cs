using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBProvider : CameraProvider
{
    [SerializeField]
    [Tooltip("Set the compression quality in percent")]
    private int qualityLevel = 50;


    public override void CaptureImage()
    {
        Camera.targetTexture = CapturedTexture;
        Camera.Render();
        renderTextureTo2DTexture();
        ImageData = CapturedImage.EncodeToJPG(qualityLevel);
        Camera.targetTexture = null;
        RenderTexture.active = null;
        CapturedTexture.Release();
    }

}
