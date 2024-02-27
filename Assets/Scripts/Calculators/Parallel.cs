using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TemperatureSimulator
{
    public class Parallel : Calculator
    {
        private ComputeShader _shader;
        private int _kernelHandle;

        private ComputeBuffer _uBuffer, _uNewBuffer, _borderBuffer, _slicesBuffer;
        private bool _isNew = false;
        private RenderTexture[] _textures;
        public override bool CalculateStep()
        {
            if (_currentTime >= Config.TimeMax)
            {
                Debug.Log("Finish");
                return true;
            }

            Debug.Log(_currentTime);
            _shader.Dispatch(_kernelHandle, Config.Size / 8, Config.Size / 8, Config.Size / 8);
            for (int i = 0; i < 3; i++)
            {
                RedrawSlice(i);
            }
            _isNew = !_isNew;
            _currentTime += Config.Tau;
            _shader.SetFloat("currentTime", _currentTime);
            _shader.SetBool("isNew", _isNew);
            return false;
        }

        private void RedrawSlice(int id)
        {
            for (int i = 0; i < Config.Size; i++)
            {
                for (int j = 0; j < Config.Size; j++)
                {
                    switch(id)
                    {
                        case 0:
                            
                            Graphics.CopyTexture(_textures[0],0,0, Config.XTarget, 0,0);
                            break;
                        case 1:
                            Graphics.CopyTexture(_textures[1], 0, 0, Config.YTarget, 0, 0);
                            break;
                        case 2:
                            Graphics.CopyTexture(_textures[2], 0, 0, Config.ZTarget, 0, 0);
                            break;
                    }
                }
            }
        }

        private void SetBorderValue(int i, int j, int k, int idBorder)
        {
            U[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), Config.Border—onditions[idBorder], Config.H, _currentTime);
            _uNew[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), Config.Border—onditions[idBorder], Config.H, _currentTime);
        }

        public override void Init(Config config)
        {
            Config = config;
            _textures = new RenderTexture[3];
            
            for (int i = 0; i < _textures.Length; i++)
            {
                _textures[i] = new RenderTexture(Config.Size, Config.Size, 24);
                _textures[i].enableRandomWrite = true;
                _textures[i].filterMode = FilterMode.Point;
                _textures[i].Create();
            }
            U = new float[Config.Size * Config.Size * Config.Size];
            _uNew = new float[U.Length];
            for (int i = 0; i < U.Length; i++)
            {
                U[i] = 0;
                _uNew[i] = 0;
            }
            _coef = Config.Alfa * Config.Alfa * Config.Tau / (Config.H * Config.H);
            _shader = Resources.Load<ComputeShader>("Shaders/Temperature");
            _uBuffer = new ComputeBuffer(U.Length, sizeof(float));
            _uNewBuffer = new ComputeBuffer(_uNew.Length, sizeof(float));
            _borderBuffer = new ComputeBuffer(Config.Border—onditions.Length, sizeof(int));
            _slicesBuffer = new ComputeBuffer(Config.TargetSlice.Length, sizeof(int));
            
            _kernelHandle = _shader.FindKernel("CSMain");
            _shader.SetTexture(_kernelHandle, "ResultX", _textures[0]);
            _shader.SetTexture(_kernelHandle, "ResultY", _textures[1]);
            _shader.SetTexture(_kernelHandle, "ResultZ", _textures[2]);
            _shader.SetFloat("Resolution", Config.Size);
            _shader.SetFloat("h", Config.H);
            _shader.SetFloat("temperatureMax", Config.MaxTemperature);
            _shader.SetInt("size", Config.Size);
            _uBuffer.SetData(U);
            _uBuffer.SetData(_uNew);
            _slicesBuffer.SetData(Config.TargetSlice);
            _borderBuffer.SetData(Config.Border—onditions);
            _shader.SetBuffer(0, "u", _uBuffer);
            _shader.SetBuffer(0, "uNew", _uNewBuffer);
            _shader.SetBuffer(0, "borders", _borderBuffer);
            _shader.SetBuffer(0, "slices", _slicesBuffer);
            _shader.SetFloat("coef", _coef);

        }

        public override void SetSlice(int coord, int num)
        {
            Config.TargetSlice[coord] = (int)Mathf.Clamp(num+Config.Size/2,0, Config.Size-1);
            _slicesBuffer.SetData(Config.TargetSlice);
            _shader.SetBuffer(0, "slices", _slicesBuffer);
            RedrawSlice(coord);
        }

    }
}