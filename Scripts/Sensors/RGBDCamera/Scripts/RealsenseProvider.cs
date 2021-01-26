using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class RealsenseProvider : CameraProvider
{
    [SerializeField]
    [Tooltip ("Material containing the shader to compute the depthmap")]
    private Material Mat;

    [SerializeField]
    [Tooltip ("Maximum number of threads in the threadpool")]
    private uint ThreadCount = 8;
    [SerializeField]
    [Tooltip ("Maximum angle above which depth values will be discarded")]
    [Range (0f, 90f)]
    private float MaxAngle = 60f;
    
    private Texture2D RGBImage;
    private Byte[] CV_Tex;
    private Byte[] Points;
    private Color32[] RGBValues;

	// Use this for initialization
	public override void Start () {
        base.Start();
        Camera.depthTextureMode = DepthTextureMode.DepthNormals;
        RGBImage = new Texture2D((int)ResolutionWidth, (int)ResolutionHeight, TextureFormat.RGB24, false, true);
        CV_Tex = new Byte[ResolutionWidth * ResolutionHeight * 2];
        RGBValues = new Color32[ResolutionWidth * ResolutionHeight];
        Points = new Byte[ResolutionWidth * ResolutionHeight * 16];
        System.Threading.ThreadPool.SetMaxThreads((int)ThreadCount, (int)ThreadCount);
    }

    public override void CaptureImage()
    {
        Camera.Render();
        // Camera.targetTexture = CapturedTexture;
        // Mat.SetFloat("u_MaxAngle", MaxAngle);
        // Camera.RenderWithShader(Shader.Find("Custom/Depth"),"RenderType");
        // PopulatePoints();
        // renderTextureTo2DTexture();
        // ImageData = CapturedImage.GetRawTextureData();
        // ImageData = CapturedImage.EncodeToJPG();
        // ConvertToCvFormat();
        // Camera.targetTexture = null;
        // RenderTexture.active = null;
        // CapturedTexture.Release();
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Mat.SetFloat("u_MaxAngle", MaxAngle);

        RGBImage.ReadPixels(Rect, 0, 0);
        RGBImage.Apply();
        PopulatePoints();
        Graphics.Blit(source, CapturedTexture, Mat);
        renderTextureTo2DTexture();
        ImageData = CapturedImage.GetRawTextureData();
        ConvertToCvFormat();
        Graphics.Blit(CapturedTexture, destination);
    }


    private void ConvertToCvFormat()
    {
        uint step = ResolutionWidth * ResolutionHeight / ThreadCount;
        for (uint i = 0; i <ThreadCount; ++i)
        {
            uint start_cv = i * step * 2;
            uint start_tex = i * step * 3;
            uint end_tex = (i + 1) * step * 3;
            System.Threading.ThreadPool.QueueUserWorkItem(ConversionWorkUnit, new object[] { start_cv, start_tex, end_tex });
        }
    }

    private void ConversionWorkUnit(object state)
    {
        object[] array = state as object[];
        uint cv_index = Convert.ToUInt32(array[0]);
        uint start_tex = Convert.ToUInt32(array[1]);
        uint end_tex = Convert.ToUInt32(array[2]);
        for (uint i = start_tex; i < end_tex; ++i)
        {
            if (i % 3 < 2)
            {
                CV_Tex[cv_index] = ImageData[i];
                ++cv_index;
            }
        }
    }

    private void GetBytesFromFloat(ref byte[] output, float value)
    {
        if (output.Length != 4)
        {
            return;
        }
        uint tmp = (uint)value;
        // unsafe
        // {
        //     tmp = *((uint*)&value);
        // }
        output[0] = (byte)(tmp & 0xFF);
        output[1] = (byte)((tmp >> 8) & 0xFF);
        output[2] = (byte)((tmp >> 16) & 0xFF);
        output[3] = (byte)((tmp >> 24) & 0xFF);
    }

    private void PopulatePointWorkUnit(object state)
    {
        object[] array = state as object[];
        uint start_index = Convert.ToUInt32(array[0]);
        uint end_index = Convert.ToUInt32(array[1]);

        uint pt_index;
        byte[] current_x = new byte[4];
        byte[] current_y = new byte[4];
        byte[] current_z = new byte[4];
        for (uint i = start_index; i < end_index; ++i)
        {
            // Not using the available BitConverter to not clog up the garbage collector
            GetBytesFromFloat(ref current_x, (i % ResolutionHeight) / 1000f);
            GetBytesFromFloat(ref current_y, (i % ResolutionWidth) / 1000f);
            GetBytesFromFloat(ref current_z, (256 * CV_Tex[i * 2 + 1] + CV_Tex[i * 2]) / 1000f);
            // 16 bytes per point. In order :
            // 4 bytes for x
            // 4 bytes for y
            // 4 bytes for z
            // 4 bytes for rgb
            pt_index = i * 16;
            for (uint index = 0; index < 4; ++index)
            {
                Points[pt_index + index] = current_x[index];
                Points[pt_index + index + 4] = current_y[index];
                Points[pt_index + index + 8] = current_z[index];
            }
            Points[pt_index + 12] = RGBValues[i].r;
            Points[pt_index + 13] = RGBValues[i].g;
            Points[pt_index + 14] = RGBValues[i].b;
            Points[pt_index + 15] = 0;
        }
    }

    private void PopulatePoints()
    {
        uint step = ResolutionWidth * ResolutionHeight / ThreadCount;
        for (uint thread_index = 0; thread_index < ThreadCount; ++thread_index)
        {
            uint start_index = thread_index * step;
            uint end_index = (thread_index + 1) * step;
            System.Threading.ThreadPool.QueueUserWorkItem(PopulatePointWorkUnit, new object[] { start_index, end_index });
        }
    }

    public Byte[] GetCvData()
    {
        return CV_Tex;
    }

    public Byte[] GetPoints()
    {
        return Points;
    }
}
