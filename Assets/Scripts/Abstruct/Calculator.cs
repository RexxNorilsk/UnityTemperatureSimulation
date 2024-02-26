using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TemperatureSimulator
{
    public abstract class Calculator
    {
        protected Config _config;
        protected float[] _u;
        protected float[] _uNew;
        protected float _coef;
        protected float _currentTime = 0;

        public virtual void Init(Config config) { }

        public virtual bool CalculateStep() { return true; }
        public virtual void SetSlice(int coord, int num) { }

        public Color ConvertTemperatureToColor(float temperature)
        {
            float ratio = temperature / _config.MaxTemperature;
            return new Color(ratio, 1 - ratio, 1 - ratio, 1f);
        }

        public int GetPos(int x, int y, int z)
        {
            return x * _config.Size * _config.Size + y * _config.Size + z;
        }
    }
}