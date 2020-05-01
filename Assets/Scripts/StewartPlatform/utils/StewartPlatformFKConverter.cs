using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StewartPlatform.utils
{
    public class StewartPlatformFKConverter
    {
        private const int VALUE_CONVERTER = 10000;

        public StewartPlatformFKConverter() { }

        public double ConvertFloatToDouble(float value)
        {
            decimal dec = new decimal(value);
            double doubleValue = (double)dec;
            return ConvertToFourDecimalPlaces(value);
        }

        public float ConvertDoubleToFloat(double value)
        {
            return Convert.ToSingle(ConvertToFourDecimalPlaces(value));
        }

        public double ConvertToFourDecimalPlaces(double value)
        {
            return (Math.Round(value * VALUE_CONVERTER)) / VALUE_CONVERTER;
        }

        public double ConvertDegreesToRadians(double angle)
        {
            double formula = angle * Math.PI / 180;
            return formula;
        }

        public double ConvertRadiansToDegrees(double angle)
        {
            double formula = angle * 180 / Math.PI;
            return formula;
        }
    }
}
