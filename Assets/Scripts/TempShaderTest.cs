using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TempShaderTest : MonoBehaviour
{
    public ComputeShader shader;
    public RawImage target;
    public int size = 256;
    public float a = 0.1f;
    public float tau;
    public float h;
    public bool parallel = false;
    
    private float coef;
    private RenderTexture _textureParallel;
    private Texture2D _textureOriginal;
    private float[] u;
    private ComputeBuffer uBuffer;
    private int kernelHandle = 0;
    private float[] uCurrent;
    private bool run = false;

    public float[] GetNowDataU()
    {
        uBuffer.GetData(u);
        for (int i = 0; i < size; i++)
            uCurrent[i] = u[size * size / 2 + i];
        return uCurrent;
    }

    int GetPos(int x, int y, int z)
    {
        return x * size * size + y * size + z;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCalc();
        if (run && Time.frameCount%10==0)
        {
            if (parallel)
            {
                shader.Dispatch(kernelHandle, _textureParallel.width / 8, _textureParallel.height / 8, _textureParallel.height / 8);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {


                            if (i == 0 || i == size - 1 || j == 0 || j == size - 1 || k == 0 || k == size - 1)
                            {
                                u[GetPos(i, j, k)] = 200;
                            }
                            else
                            {
                                u[GetPos(i, j, k)] = u[GetPos(i, j, k)] + coef * (
                                    u[GetPos(i + 1, j, k)] +
                                    u[GetPos(i - 1, j, k)] +
                                    u[GetPos(i, j + 1, k)] +
                                    u[GetPos(i, j - 1, k)] +
                                    u[GetPos(i, j, k + 1)] +
                                    u[GetPos(i, j, k - 1)] -
                                    6 * u[GetPos(i, j, k)]
                                );
                            }
                        }
                        _textureOriginal.SetPixel(i, j, ConvertTemperatureToColor(u[GetPos(i,j, 1)]));
                    }
                }
                _textureOriginal.Apply();
            }
        }
    }

    public Color ConvertTemperatureToColor(float temperature)
    {
        float ratio = temperature/ 200.0f;
        return new Color(ratio, 1 - ratio, 1 - ratio, 1f);
    }

private void StartCalc()
    {
        uCurrent = new float[size];
        u = new float[size * size * size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    u[GetPos(i,j,k)] = 0;
                }
            }
        }

        _textureParallel = new RenderTexture(size, size, 24);
        _textureParallel.enableRandomWrite = true;
        _textureParallel.Create();
        _textureOriginal = new Texture2D(size, size);
        _textureOriginal.Apply();
        if (parallel)
            target.texture = _textureParallel;
        else
            target.texture = _textureOriginal;
        uBuffer = new ComputeBuffer(u.Length, sizeof(float));
        coef = a * a * tau / (h * h);
        Debug.Log($"Coef: {coef}");
        kernelHandle = shader.FindKernel("CSMain");
        shader.SetTexture(kernelHandle, "Result", _textureParallel);
        shader.SetFloat("Resolution", _textureParallel.width);
        shader.SetInt("size", size);
        uBuffer.SetData(u);
        shader.SetBuffer(0, "u", uBuffer);
        shader.SetFloat("coef", coef);
        run = true;
        

    }
}
