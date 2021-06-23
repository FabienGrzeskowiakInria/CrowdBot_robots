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
using UnityEngine.Rendering;
using System;
using System.IO;
using UnityEngine;

public class PointCloudProvider : CameraProvider
{
    [Range(1, 8)]
    public uint firstDisplay = 2;
    public uint ThreadCount = 8;

    public float frequency_update = 5; //Hz
    private float last_update = 0;
    // PointCloud
    private Byte[] Points;

    private Byte[] RGB;

    private Byte[] Depth;

    private Byte[] Depth2;

    private byte[] current_x;
    private byte[] current_y;
    private byte[] current_z;

    private float FOV_x = 90f * Mathf.Deg2Rad;
    private float FOV_y = 121.2845f * Mathf.Deg2Rad;
    private float near = 0.1f;
    private float far = 50;

    [Header("Shader to compute the depthmap")]
    public Shader depthShader;

    public enum ReplacementMode
    {
        Rgb = 0,
        DepthCompressed = 1,
        DepthMultichannel = 2,
    };

    // pass configuration
    private CapturePass[] capturePasses = new CapturePass[] {
        new CapturePass() { mode = ReplacementMode.Rgb },
        new CapturePass() { mode = ReplacementMode.DepthMultichannel }
    };

    struct CapturePass
    {
        // configuration
        public ReplacementMode mode;
        public CapturePass(string name_) { mode = ReplacementMode.Rgb; camera = null; }

        // impl
        public Camera camera;
    };


    // Use this for initialization
    public override void Start()
    {
        base.Start();
        // use real camera to capture final image
        capturePasses[0].camera = GetComponent<Camera>();
        if (capturePasses[0].camera == null) capturePasses[0].camera = GetComponentInChildren<Camera>();
        for (int q = 1; q < capturePasses.Length; q++)
            capturePasses[q].camera = CreateHiddenCamera(capturePasses[q].mode, GetComponentInChildren<Camera>().gameObject);

        Points = new Byte[ResolutionWidth * ResolutionHeight * 16];
        current_x = new byte[4];
        current_y = new byte[4];
        current_z = new byte[4];
        OnCameraChange();

        System.Threading.ThreadPool.SetMaxThreads((int)ThreadCount, (int)ThreadCount);

    }
    public override void CaptureImage()
    {
        // Save();
    }


    void LateUpdate()
    {
        if (ToolsTime.DeltaTime > 0)
        {
            last_update += ToolsTime.DeltaTime;
        }
        if (last_update > 1 / frequency_update)
        {
            OnCameraChange();
            Save();
            last_update = 0;
        }
    }

    private Camera CreateHiddenCamera(ReplacementMode mode, GameObject cam)
    {
        var go = new GameObject("cam" + mode.ToString(), typeof(Camera));
        go.hideFlags = HideFlags.HideAndDontSave;

        go.transform.parent = cam.transform;

        var newCamera = go.GetComponent<Camera>();
        return newCamera;
    }
    static private void SetupCameraWithReplacementShader(Camera cam, Shader shader, ReplacementMode mode)
    {
        SetupCameraWithReplacementShader(cam, shader, mode, Color.black);
    }

    static private void SetupCameraWithReplacementShader(Camera cam, Shader shader, ReplacementMode mode, Color clearColor)
    {
        var cb = new CommandBuffer();
        cb.SetGlobalFloat("_OutputMode", (int)mode); // @TODO: CommandBuffer is missing SetGlobalInt() method
        cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, cb);
        cam.AddCommandBuffer(CameraEvent.BeforeFinalPass, cb);
        cam.SetReplacementShader(shader, "");
        cam.backgroundColor = clearColor;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.allowHDR = false;
        cam.allowMSAA = false;
    }

    public void OnCameraChange()
    {
        int targetDisplay = (int)firstDisplay;
        var mainCamera = GetComponent<Camera>();
        if (mainCamera == null) mainCamera = GetComponentInChildren<Camera>();
        foreach (var pass in capturePasses)
        {
            if (pass.camera == mainCamera)
                continue;

            // cleanup capturing camera
            pass.camera.RemoveAllCommandBuffers();

            // copy all "main" camera parameters into capturing camera
            pass.camera.CopyFrom(mainCamera);

            // set targetDisplay here since it gets overriden by CopyFrom()
            pass.camera.targetDisplay = targetDisplay++;
        }

        // setup command buffers and replacement shaders
        // SetupCameraWithReplacementShader(capturePasses[1].camera, depthShader, ReplacementMode.DepthCompressed, Color.white);
        SetupCameraWithReplacementShader(capturePasses[1].camera, depthShader, ReplacementMode.DepthMultichannel);
    }

    public void Save()
    {
        // execute as coroutine to wait for the EndOfFrame before starting capture
        StartCoroutine(WaitForEndOfFrameAndSave());
    }


    private IEnumerator WaitForEndOfFrameAndSave()
    {
        yield return new WaitForEndOfFrame();

        foreach (var pass in capturePasses)
        {
            Save(pass.camera, pass.mode);
        }

        PopulatePoints();
    }

    private void PopulatePoints()
    {
        uint step = ResolutionWidth * ResolutionHeight / ThreadCount;
        for (uint thread_index = 0; thread_index < ThreadCount; ++thread_index)
        {
            uint start_index = thread_index * step;
            uint end_index = (thread_index + 1) * step;
            System.Threading.ThreadPool.QueueUserWorkItem(PopulatePointsWorkUnit, new object[] { start_index, end_index });
        }
    }
    private void PopulatePointsWorkUnit(object state)
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
            pt_index = i * 16;

            float dep = (((Depth2[i * 3 + 1] + Depth2[i * 3] / 256f) / 256f * (far - near)) + near);
            if (dep <= (near + 0.05f)) dep = far * 1000f;

            float x = (((float)i / (float)ResolutionHeight) - ResolutionWidth / 2f) / ResolutionWidth;
            float x_real = x + dep * Mathf.Tan(x * (FOV_x / 2f));
            float y = ((float)(i % ResolutionWidth) - ResolutionHeight / 2f) / ResolutionHeight;
            float y_real = y + dep * Mathf.Tan(y * (FOV_y / 2f));

            float dep_real = (float)Math.Sqrt(Math.Pow(dep, 2f) + Math.Pow(x_real - x, 2f) + Math.Pow(y_real - y, 2));
            // float dep = (Depth[i *3] / 256f * (far - near)) + near;
            GetBytesFromFloat(ref current_x, x_real);
            GetBytesFromFloat(ref current_y, y_real);
            GetBytesFromFloat(ref current_z, dep_real);// * Mathf.Cos(x * (FOV_x / 2f)) * Mathf.Cos(y * (FOV_y / 2f)));
            // GetBytesFromFloat(ref current_z, Depth[i * 3]);

            for (uint index = 0; index < 4; ++index)
            {
                Points[pt_index + index] = current_x[index];
                Points[pt_index + index + 4] = current_y[index];
                Points[pt_index + index + 8] = current_z[index];

            }
            Points[pt_index + 12] = RGB[i * 3 + 2];
            Points[pt_index + 13] = RGB[i * 3 + 1];
            Points[pt_index + 14] = RGB[i * 3];
            Points[pt_index + 15] = 255;

        }
    }

    private void Save(Camera cam, ReplacementMode mode)
    {
        var mainCamera = GetComponent<Camera>();
        var depth = 24;

        var renderRT = RenderTexture.GetTemporary((int)ResolutionWidth, (int)ResolutionHeight, depth);
        var tex = new Texture2D((int)ResolutionWidth, (int)ResolutionHeight, TextureFormat.RGB24, false);

        var prevActiveRT = RenderTexture.active;
        var prevCameraRT = cam.targetTexture;

        // render to offscreen texture (readonly from CPU side)
        RenderTexture.active = renderRT;
        cam.targetTexture = renderRT;

        cam.Render();

        // read offsreen texture contents into the CPU readable texture
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        switch (mode)
        {
            case ReplacementMode.Rgb:
                RGB = tex.GetRawTextureData();
                break;
            case ReplacementMode.DepthCompressed:
                Depth = tex.GetRawTextureData();
                break;
            case ReplacementMode.DepthMultichannel:
                Depth2 = tex.GetRawTextureData();
                break;
        }

        // restore state and cleanup
        cam.targetTexture = prevCameraRT;
        RenderTexture.active = prevActiveRT;

        UnityEngine.Object.Destroy(tex);
        RenderTexture.ReleaseTemporary(renderRT);
    }

    private void GetBytesFromFloat(ref byte[] output, float value)
    {
        if (output.Length != 4)
        {
            return;
        }

        float[] val_array = new float[1] { value };
        Buffer.BlockCopy(val_array, 0, output, 0, output.Length);
    }

    private void GetFloatFromByte(ref float output, byte[] value)
    {
        if (value.Length != 4)
        {
            return;
        }

        float[] val_array = new float[1];
        Buffer.BlockCopy(value, 0, val_array, 0, value.Length);
        output = val_array[0];
    }
    public Byte[] GetPoints()
    {
        return Points;
    }
}
