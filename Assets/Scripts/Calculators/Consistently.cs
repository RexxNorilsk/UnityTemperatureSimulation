using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TemperatureSimulator
{
    public class Consistently : ICalculator
    {
        private Config _config;
        private float[] _u;
        private float[] _uNew;
        private float _coef;
        private float _currentTime = 0;
        private bool _isNew = false;
        private int GetPos(int x, int y, int z)
        {
            return x * _config.Size * _config.Size + y * _config.Size + z;
        }
        private Color ConvertTemperatureToColor(float temperature)
        {
            float ratio = temperature / 200.0f;
            return new Color(ratio, 1 - ratio, 1 - ratio, 1f);
        }

        private float Functions(int x, int y, int z, float valueInPos, int funcId)
        {
            switch (funcId)
            {
                case 0:
                    return 200;
                case 1:
                    return 0;
                case 2:
                    return 200 * (x * _config.H);
                case 3:
                    return 200 * (y * _config.H);
                case 4:
                    return 200 * (z * _config.H);
                case 5:
                    return 200*x*y;
            }
            return 0;
        }

        public bool CalculateStep()
        {
            if (_currentTime >= _config.TimeMax)
            {
                Debug.Log("Finish");
                return true;
            }
            Debug.Log(_currentTime);
            for (int i = 0; i < _config.Size; i++)
            {
                for (int j = 0; j < _config.Size; j++)
                {
                    for (int k = 0; k < _config.Size; k++)
                    {
                        if (i == 0)
                        {
                            _u[GetPos(i, j, k)] = Functions(i,j,k, GetPos(i, j, k), _config.BorderÑonditions[0]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[0]);
                        }
                        else if (i == _config.Size - 1) {
                            _u[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[1]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[1]);
                        }
                        else if (j == 0)
                        {
                            _u[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[2]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[2]);
                        }
                        else if (j == _config.Size - 1)
                        {
                            _u[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[3]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[3]);
                        }
                        else if (k == 0)
                        {
                            _u[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[4]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[4]);
                        }
                        else if (k == _config.Size - 1) {
                            _u[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[5]);
                            _uNew[GetPos(i, j, k)] = Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[5]);
                        }
                        else
                        {
                            if (!_isNew)
                            {
                                _uNew[GetPos(i, j, k)] = _u[GetPos(i, j, k)] + _coef * (
                                    _u[GetPos(i + 1, j, k)] +
                                    _u[GetPos(i - 1, j, k)] +
                                    _u[GetPos(i, j + 1, k)] +
                                    _u[GetPos(i, j - 1, k)] +
                                    _u[GetPos(i, j, k + 1)] +
                                    _u[GetPos(i, j, k - 1)] -
                                    6 * _u[GetPos(i, j, k)]
                                );
                            }
                            else
                            {
                                _u[GetPos(i, j, k)] = _uNew[GetPos(i, j, k)] + _coef * (
                                       _uNew[GetPos(i + 1, j, k)] +
                                       _uNew[GetPos(i - 1, j, k)] +
                                       _uNew[GetPos(i, j + 1, k)] +
                                       _uNew[GetPos(i, j - 1, k)] +
                                       _uNew[GetPos(i, j, k + 1)] +
                                       _uNew[GetPos(i, j, k - 1)] -
                                       6 * _uNew[GetPos(i, j, k)]
                                   );
                            }
                        }
                        
                    }
                }
            }
            for (int i = 0; i < _config.Size; i++)
            {
                for (int j = 0; j < _config.Size; j++)
                {
                    _config.XTarget.SetPixel(j, i, ConvertTemperatureToColor(_uNew[GetPos(_config.TargetSlice[0], i, j)]));
                    _config.YTarget.SetPixel(i, j, ConvertTemperatureToColor(_uNew[GetPos(i, _config.TargetSlice[1], j)]));
                    _config.ZTarget.SetPixel(i, j, ConvertTemperatureToColor(_uNew[GetPos(i, j, _config.TargetSlice[2])]));
                }
            }
            _isNew = !_isNew;
            _config.XTarget.Apply();
            _config.YTarget.Apply();
            _config.ZTarget.Apply();
            _currentTime += _config.Tau;
            return false;
        }

        public void Init(Config config)
        {
            _config = config;
            _u = new float[_config.Size * _config.Size * _config.Size];
            _uNew = new float[_u.Length];
            for (int i = 0; i < _u.Length; i++)
            {
                _u[i] = 0;
                _uNew[i] = 0;
            }
            _coef = _config.Alfa * _config.Alfa * _config.Tau / (_config.H * _config.H);
        }

        public void SetSlice(int coord, int num)
        {
            _config.TargetSlice[coord] = (int)Mathf.Clamp(num+_config.Size/2,0, _config.Size-1);

            for (int i = 0; i < _config.Size; i++)
            {
                for (int j = 0; j < _config.Size; j++)
                {
                    switch (coord)
                    {
                        case 0:
                            _config.XTarget.SetPixel(j, i, ConvertTemperatureToColor(_u[GetPos(_config.TargetSlice[0], i, j)]));
                            break;
                        case 1:
                            _config.YTarget.SetPixel(i, j, ConvertTemperatureToColor(_u[GetPos(i, _config.TargetSlice[1], j)]));
                            break;
                        case 2:
                            _config.ZTarget.SetPixel(i, j, ConvertTemperatureToColor(_u[GetPos(i, j, _config.TargetSlice[2])]));
                            break;
                    }
                }
            }
            _config.XTarget.Apply();
            _config.YTarget.Apply();
            _config.ZTarget.Apply();
        }

    }
}