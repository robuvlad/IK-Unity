using Assets.Scripts.StewartPlatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.StewartPlatform.rotation
{
    public class ServiceRotationIK : ScriptableObject, IRotation
    {
        private Transform[] topLegs;
        private Transform[] bottomLegs;

        public void Init(Transform[] topLegs, Transform[] bottomLegs)
        {
            this.topLegs = topLegs;
            this.bottomLegs = bottomLegs;
        }

        public void RotateLegs()
        {
            for (int i = 0; i < StewartPlatformIKUtils.NUMBER_OF_LEGS; i++)
            {
                Vector3 direction = topLegs[i].position - bottomLegs[i].position;
                bottomLegs[i].rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
