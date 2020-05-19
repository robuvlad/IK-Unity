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
        private GameObject trajectory;
        private Transform endEffector;

        public void Init(Slider[] sliders, GameObject trajectory, Transform endEffector)
        {
            this.sliders = sliders;
            this.trajectory = trajectory;
            this.endEffector = endEffector;
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
            // MOVE RIGHT
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_RIGHT; i++) {
                MoveTrajectoryRight();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotRight();
            }

            // MOVE DOWN
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_DOWN; i++) {
                MoveTrajectoryDown();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotDown();
            }

            // MOVE UP
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_UP; i++) {
                MoveTrajectoryUp();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotUp();
            }

            // MOVE LEFT
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_LEFT; i++) {
                MoveTrajectoryLeft();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotLeft();
            }

            // MOVE DOWN
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_DOWN; i++) {
                MoveTrajectoryDown();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotDown();
            }

            // MOVE UP
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_UP; i++) {
                MoveTrajectoryUp();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotUp();
            }

            // MOVE RIGHT
            for (int i = 0; i < DeltaRobotIKUtils.MOVE_RIGHT; i++) {
                MoveTrajectoryRight();
                yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
                MoveRobotRight();
            }
        }

        private void MoveTrajectoryRight()
        {
            Vector3 pos = GetVector3Trajectory() + new Vector3(0.0f, 0.0f, 0.0f);
            trajectory.transform.position = new Vector3(pos.x + DeltaRobotIKUtils.SLIDER_INCREMENT, pos.y, pos.z);
        }

        private void MoveRobotRight()
        {
            sliders[0].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
        }

        private void MoveTrajectoryDown()
        {
            Vector3 pos = GetVector3Trajectory();
            trajectory.transform.position = new Vector3(pos.x, pos.y - DeltaRobotIKUtils.SLIDER_INCREMENT, pos.z);
        }

        private void MoveRobotDown()
        {
            sliders[2].value -= DeltaRobotIKUtils.SLIDER_INCREMENT;
        }

        private void MoveTrajectoryUp()
        {
            Vector3 pos = GetVector3Trajectory();
            trajectory.transform.position = new Vector3(pos.x, pos.y + DeltaRobotIKUtils.SLIDER_INCREMENT, pos.z);
        }

        private void MoveRobotUp()
        {
            sliders[2].value += DeltaRobotIKUtils.SLIDER_INCREMENT;
        }

        private void MoveTrajectoryLeft()
        {
            Vector3 pos = GetVector3Trajectory();
            trajectory.transform.position = new Vector3(pos.x - DeltaRobotIKUtils.SLIDER_INCREMENT, pos.y, pos.z);
        }

        private void MoveRobotLeft()
        {
            sliders[0].value -= DeltaRobotIKUtils.SLIDER_INCREMENT;
        }

        private Vector3 GetVector3Trajectory()
        {
            return trajectory.transform.position;
        }
    }
}
