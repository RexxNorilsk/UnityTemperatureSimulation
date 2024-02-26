using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TemperatureSimulator
{
    public class Consistently : Calculator
    {
        
        private bool _isNew = false;
        

        public override bool CalculateStep()
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
                            SetBorderValue(i, j, k, 0);
                        }
                        else if (i == _config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 1);
                        }
                        else if (j == 0)
                        {
                            SetBorderValue(i, j, k, 2);
                        }
                        else if (j == _config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 3);
                        }
                        else if (k == 0)
                        {
                            SetBorderValue(i, j, k, 4);
                        }
                        else if (k == _config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 5);
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
            for (int i = 0; i < 3; i++)
            {
                RedrawSlice(i);
            }
            _isNew = !_isNew;
            _currentTime += _config.Tau;
            return false;
        }

        private void RedrawSlice(int id)
        {
            for (int i = 0; i < _config.Size; i++)
            {
                for (int j = 0; j < _config.Size; j++)
                {
                    switch(id)
                    {
                        case 0:
                            _config.XTarget.SetPixel(j, i, ConvertTemperatureToColor(_u[GetPos(_config.TargetSlice[0], i, j)]));
                            _config.XTarget.Apply();
                            break;
                        case 1:
                            _config.YTarget.SetPixel(i, j, ConvertTemperatureToColor(_u[GetPos(i, _config.TargetSlice[1], j)]));
                            _config.YTarget.Apply();
                            break;
                        case 2:
                            _config.ZTarget.SetPixel(i, j, ConvertTemperatureToColor(_u[GetPos(i, j, _config.TargetSlice[2])]));
                            _config.ZTarget.Apply();
                            break;
                    }
                }
            }
        }

        private void SetBorderValue(int i, int j, int k, int idBorder)
        {
            _u[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[idBorder], _config.H);
            _uNew[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), _config.BorderÑonditions[idBorder], _config.H);
        }

        public override void Init(Config config)
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

        public override void SetSlice(int coord, int num)
        {
            _config.TargetSlice[coord] = (int)Mathf.Clamp(num+_config.Size/2,0, _config.Size-1);
            RedrawSlice(num);
        }

    }
}