using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DeltaRobot.init
{
    public interface IInitialize
    {
        void InitTriangles();
        void InitSliderValuesIK();
        void InitSliderValuesFK();
    }
}
