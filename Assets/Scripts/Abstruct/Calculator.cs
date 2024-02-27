using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TemperatureSimulator
{
    public abstract class Calculator
    {
        protected Config config;
        protected float[] u;
        protected float[] _uNew;
        protected float _coef;
        protected float _currentTime = 0;

        public float[] U { get => u; set => u = value; }
        public Config Config { get => config; set => config= value; }

        public virtual void Init(Config config) { }

        public virtual bool CalculateStep() { return true; }
        public virtual void SetSlice(int coord, int num) { }

        public Color ConvertTemperatureToColor(float temperature)
        {
            float ratio = temperature / Config.MaxTemperature;
            return new Color(ratio, 1 - ratio, 1 - ratio, 1f);
        }

        public int GetPos(int x, int y, int z)
        {
            return x * Config.Size * Config.Size + y * Config.Size + z;
        }
    }
}