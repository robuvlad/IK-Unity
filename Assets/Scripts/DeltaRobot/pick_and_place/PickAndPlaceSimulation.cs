using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DeltaRobot.pick_and_place
{
    public class PickAndPlaceSimulation : MonoBehaviour
    {
        private Slider[] sliders;

        public void Init(Slider[] sliders)
        {
            this.sliders = sliders;
            StartCoroutine(MoveDeltaRobot());
        }

        public IEnumerator MoveDeltaRobot()
        {
            while (DeltaRobotIKUtils.NO_OF_TOYS > 0)
            {
                StartCoroutine(PickAndPlace());
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_BOXES);
                DeltaRobotIKUtils.NO_OF_TOYS -= 1;
            }
        }

        private IEnumerator PickAndPlace()
        {
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_RIGHT; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[0].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_DOWN; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[2].value -= DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_UP; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[2].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_LEFT; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[0].value -= DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_DOWN; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[2].value -= DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_UP; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[2].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_RIGHT; i++)
            {
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                sliders[0].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
            }
        }
    }
}
