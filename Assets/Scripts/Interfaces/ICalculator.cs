using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TemperatureSimulator
{
    public interface ICalculator
    {
        public void Init(Config config);
        public bool CalculateStep();
        public void SetSlice(int coord, int num);
    }
}