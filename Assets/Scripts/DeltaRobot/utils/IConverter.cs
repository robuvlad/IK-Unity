using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DeltaRobot.utils
{
    public interface IConverter
    {
        double ConvertToTwoDecimalPlaces(double value);
        double ConvertDegreesToRadians(double angle);
        double ConvertRadiansToDegrees(double angle);
        float ConvertDoubleToFloat(double value);
    }
}
