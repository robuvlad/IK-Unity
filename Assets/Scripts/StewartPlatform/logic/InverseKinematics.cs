using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.StewartPlatform.logic
{
    public class InverseKinematics : ScriptableObject
    {
        private Transform[] topLegs, bottomLegs, finalLegs, children;
        private Transform endEffector, baseEffector;

        public void Init(Transform[] topLegs, Transform[] bottomLegs, Transform[] finalLegs, Transform[] children, Transform endEffector, Transform baseEffector)
        {
            this.topLegs = topLegs;
            this.bottomLegs = bottomLegs;
            this.finalLegs = finalLegs;
            this.children = children;
            this.endEffector = endEffector;
            this.baseEffector = baseEffector;
        }

        public void DoInverseKinematics(float phi_roll, float theta_pitch, float psi_yaw)
        {
            float phi = FromDegreesToRadians(phi_roll);
            float theta = FromDegreesToRadians(theta_pitch);
            float psi = FromDegreesToRadians(psi_yaw);

            Vector4 col1 = GetOneColumn(Mathf.Cos(psi) * Mathf.Cos(theta), Mathf.Sin(psi) * Mathf.Sin(theta), -Mathf.Sin(theta), 0.0f);
            Vector4 col2 = GetOneColumn(-Mathf.Sin(psi) * Mathf.Cos(phi) + Mathf.Cos(psi) * Mathf.Sin(theta) * Mathf.Sin(phi),
                Mathf.Cos(psi) * Mathf.Cos(phi) + Mathf.Sin(psi) * Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta) * Mathf.Sin(phi), 0.0f);
            Vector4 col3 = GetOneColumn(Mathf.Sin(psi) * Mathf.Sin(phi) + Mathf.Cos(psi) * Mathf.Sin(theta) * Mathf.Cos(phi),
                -Mathf.Cos(psi) * Mathf.Sin(phi) + Mathf.Sin(psi) * Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Cos(theta) * Mathf.Cos(phi), 0.0f);
            Vector4 col4 = GetOneColumn(0.0f, 0.0f, 0.0f, 1.0f);
            Matrix4x4 matrix = GetMatrix4x4(col1, col2, col3, col4);

            Vector3 p = GetP();

            for (int i = 0; i < bottomLegs.Length; i++)
            {
                Vector3 a = GetA(bottomLegs[i]);
                Vector3 b = GetB(topLegs[i]);
                Vector3 bFrame0 = GetBFrame0(b, matrix);
                Vector3 s = GetS(p, a, bFrame0);
                DrawIK(s, finalLegs[i], bottomLegs[i], children[i]);
            }
        }

        private Vector4 GetOneColumn(float xValue, float yValue, float zValue, float wValue)
        {
            Vector4 column = new Vector4(xValue, yValue, zValue, wValue);
            return column;
        }

        private Matrix4x4 GetMatrix4x4(Vector4 col1, Vector4 col2, Vector4 col3, Vector4 col4)
        {
            Matrix4x4 matrix = new Matrix4x4(col1, col2, col3, col4);
            return matrix;
        }

        private Vector3 GetP()
        {
            return endEffector.position - baseEffector.position;
        }

        private Vector3 GetA(Transform legGO)
        {
            return legGO.position - baseEffector.position;
        }

        private Vector3 GetB(Transform upGO)
        {
            return upGO.position - endEffector.position;
        }

        private Vector3 GetBFrame0(Vector3 b, Matrix4x4 matrix)
        {
            Vector4 bFrame1 = new Vector4(b.x, b.y, b.z, 0.0f);
            Vector4 matrixResult = matrix * bFrame1;
            Vector3 bFrame0 = new Vector3(matrixResult.x, matrixResult.y, matrixResult.z);
            return bFrame0;
        }

        private Vector3 GetS(Vector3 p, Vector3 a, Vector3 bFrame0)
        {
            return p - a + bFrame0;
        }

        private void DrawIK(Vector3 s, Transform currentFinalLeg, Transform currentLeg, Transform currentChild)
        {
            float sMagnitude = s.magnitude;
            float distance = (currentFinalLeg.position - currentLeg.position).magnitude;
            float difference = sMagnitude - distance;

            currentChild.localPosition = new Vector3(0.0f, 0.0f + difference / currentFinalLeg.parent.localScale.y, 0.0f);
        }

        private float FromDegreesToRadians(float theta)
        {
            return theta * Mathf.PI / 180.0f;
        }
    }
}
