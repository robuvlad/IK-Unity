using Assets.Scripts.DeltaRobot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DeltaRobot.ik
{
    public class LegsRotation : ScriptableObject
    {
        private ServiceConverter serviceConverter;

        private Transform[] topLPivots;
        private Transform[] parallelograms;
        private Transform[] PPoints;
        private Transform endEffector;

        public void Init(ServiceConverter serviceConverter, Transform[] topLPivots, Transform[] parallelograms, Transform[] PPoints, Transform endEffector)
        {
            this.serviceConverter = serviceConverter;
            this.topLPivots = topLPivots;
            this.parallelograms = parallelograms;
            this.PPoints = PPoints;
            this.endEffector = endEffector;
        }

        public void RotateLegs(double[] thetas)
        {
            double angle1_d = serviceConverter.ConvertRadiansToDegrees(thetas[0]);
            double angle2_d = serviceConverter.ConvertRadiansToDegrees(thetas[1]);
            double angle3_d = serviceConverter.ConvertRadiansToDegrees(thetas[2]);

            float angle1 = serviceConverter.ConvertDoubleToFloat(angle1_d);
            float angle2 = serviceConverter.ConvertDoubleToFloat(angle2_d);
            float angle3 = serviceConverter.ConvertDoubleToFloat(angle3_d);

            topLPivots[0].rotation = Quaternion.Euler(new Vector3(-angle1, topLPivots[0].eulerAngles.y, topLPivots[0].eulerAngles.z));
            topLPivots[1].rotation = Quaternion.Euler(new Vector3(-angle2, topLPivots[1].eulerAngles.y, topLPivots[1].eulerAngles.z));
            topLPivots[2].rotation = Quaternion.Euler(new Vector3(-angle3, topLPivots[2].eulerAngles.y, topLPivots[2].eulerAngles.z));
        }

        public void RotateParallelograms()
        {
            Vector3 firstTarget = GetFirstParallelogram();
            Vector3 secondTarget = GetSecondParallelogram();
            Vector3 thirdTarget = GetThirdParallelogram();
            RotateObjectTowards(parallelograms[0], firstTarget);
            RotateObjectTowards(parallelograms[1], firstTarget);
            RotateObjectTowards(parallelograms[2], secondTarget);
            RotateObjectTowards(parallelograms[3], secondTarget);
            RotateObjectTowards(parallelograms[4], thirdTarget);
            RotateObjectTowards(parallelograms[5], thirdTarget);
        }

        private Vector3 GetFirstParallelogram()
        {
            float x = -(parallelograms[0].position.x - PPoints[0].position.x - 0.5f);
            float y = -(parallelograms[0].position.y - endEffector.position.y);
            float z = PPoints[0].position.z - parallelograms[0].position.z;
            Vector3 target = new Vector3(x, y, z);
            return target;
        }

        private Vector3 GetSecondParallelogram()
        {
            float x = -(parallelograms[2].position.x - PPoints[1].position.x + 0.25f);
            float y = -(parallelograms[2].position.y - endEffector.position.y);
            float z = -(parallelograms[2].position.z - PPoints[1].position.z - 0.4325f);
            Vector3 target = new Vector3(x, y, z);
            return target;
        }

        private Vector3 GetThirdParallelogram()
        {
            float x = PPoints[2].position.x - parallelograms[4].position.x - 0.25f;
            float y = -(parallelograms[4].position.y - endEffector.position.y);
            float z = -(parallelograms[4].position.z - PPoints[2].position.z + 0.4325f);
            Vector3 target = new Vector3(x, y, z);
            return target;
        }

        private void RotateObjectTowards(Transform currentObject, Vector3 target)
        {
            currentObject.rotation = Quaternion.LookRotation(target);
        }
    }
}
