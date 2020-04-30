using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DeltaRobot.utils
{
    public class ServiceConverter : IConverter
    {
        public ServiceConverter() { }

        public double ConvertDegreesToRadians(double angle)
        {
            double formula = angle * Math.PI / 180;
            return formula;
        }

        public double ConvertRadiansToDegrees(double angle)
        {
            double formula = (angle * 180.0) / Math.PI;
            return formula;
        }

        public float ConvertDoubleToFloat(double value)
        {
            return Convert.ToSingle(ConvertToTwoDecimalPlaces(value));
        }

        public double ConvertToTwoDecimalPlaces(double value)
        {
            return Math.Round(value * 100) / 100;
        }
    }
}
