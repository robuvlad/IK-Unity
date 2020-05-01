using Assets.Scripts.StewartPlatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.rotation
{
    public class ServiceRotationFK : ScriptableObject, IRotation
    {
        private Transform[] topLegs, bottomLegs, finalLegs, children;
        private Slider[] sliders;

        public void Init(Transform[] topLegs, Transform[] bottomLegs, Transform[] finalLegs, Transform[] children, Slider[] sliders)
        {
            this.topLegs = topLegs;
            this.bottomLegs = bottomLegs;
            this.finalLegs = finalLegs;
            this.children = children;
            this.sliders = sliders;
        }

        public void RotateLegs()
        {
            for (int i = 0; i < StewartPlatformFKUtils.NO_OF_LEGS; i++)
            {
                Vector3 direction = topLegs[i].position - bottomLegs[i].position;
                bottomLegs[i].rotation = Quaternion.LookRotation(direction);

                float smallLegDistance = (finalLegs[i].position - bottomLegs[i].position).magnitude;
                float bigLegDistance = (topLegs[i].position - bottomLegs[i].position).magnitude;
                children[i].localPosition = new Vector3(0.0f, (bigLegDistance - smallLegDistance) / 2.0f, 0.0f);

                sliders[i].value = (bigLegDistance - smallLegDistance) / 2.0f;
            }
        }
    }
}
