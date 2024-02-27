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
            if (_currentTime >= Config.TimeMax)
            {
                Debug.Log("Finish");
                return true;
            }
            Debug.Log(_currentTime);
            for (int i = 0; i < Config.Size; i++)
            {
                for (int j = 0; j < Config.Size; j++)
                {
                    for (int k = 0; k < Config.Size; k++)
                    {
                        if (i == 0)
                        {
                            SetBorderValue(i, j, k, 0);
                        }
                        else if (i == Config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 1);
                        }
                        else if (j == 0)
                        {
                            SetBorderValue(i, j, k, 2);
                        }
                        else if (j == Config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 3);
                        }
                        else if (k == 0)
                        {
                            SetBorderValue(i, j, k, 4);
                        }
                        else if (k == Config.Size - 1)
                        {
                            SetBorderValue(i, j, k, 5);
                        }
                        else
                        {
                            if (!_isNew)
                            {
                                _uNew[GetPos(i, j, k)] = U[GetPos(i, j, k)] + _coef * (
                                    U[GetPos(i + 1, j, k)] +
                                    U[GetPos(i - 1, j, k)] +
                                    U[GetPos(i, j + 1, k)] +
                                    U[GetPos(i, j - 1, k)] +
                                    U[GetPos(i, j, k + 1)] +
                                    U[GetPos(i, j, k - 1)] -
                                    6 * U[GetPos(i, j, k)]
                                );
                            }
                            else
                            {
                                U[GetPos(i, j, k)] = _uNew[GetPos(i, j, k)] + _coef * (
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
            _currentTime += Config.Tau;
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
                            Config.XTarget.SetPixel(j, i, ConvertTemperatureToColor(U[GetPos(Config.TargetSlice[0], i, j)]));
                            Config.XTarget.Apply();
                            break;
                        case 1:
                            Config.YTarget.SetPixel(i, j, ConvertTemperatureToColor(U[GetPos(i, Config.TargetSlice[1], j)]));
                            Config.YTarget.Apply();
                            break;
                        case 2:
                            Config.ZTarget.SetPixel(i, j, ConvertTemperatureToColor(U[GetPos(i, j, Config.TargetSlice[2])]));
                            Config.ZTarget.Apply();
                            break;
                    }
                }
            }
        }

        private void SetBorderValue(int i, int j, int k, int idBorder)
        {
            U[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), Config.BorderÑonditions[idBorder], Config.H, _currentTime);
            _uNew[GetPos(i, j, k)] = Utilities.Functions(i, j, k, GetPos(i, j, k), Config.BorderÑonditions[idBorder], Config.H, _currentTime);
        }

        public override void Init(Config config)
        {
            Config = config;
            U = new float[Config.Size * Config.Size * Config.Size];
            _uNew = new float[U.Length];
            for (int i = 0; i < U.Length; i++)
            {
                U[i] = 0;
                _uNew[i] = 0;
            }
            _coef = Config.Alfa * Config.Alfa * Config.Tau / (Config.H * Config.H);
        }

        public override void SetSlice(int coord, int num)
        {
            Config.TargetSlice[coord] = (int)Mathf.Clamp(num+Config.Size/2,0, Config.Size-1);
            RedrawSlice(coord);
        }

    }
}