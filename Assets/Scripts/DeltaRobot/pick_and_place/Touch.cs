using Assets.Scripts.DeltaRobot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            StartCoroutine(MoveTriggeredObject(other.gameObject.transform));
            isTriggered = true;
        }
    }

    private IEnumerator MoveTriggeredObject(Transform other)
    {
        for (int i = 0; i < DeltaRobotIKUtils.MOVE_UP; i++)
        {
            yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
            other.position = new Vector3(other.position.x, other.position.y + DeltaRobotIKUtils.SLIDER_INCREMENT, other.position.z);
        }
        for (int i = 0; i < DeltaRobotIKUtils.MOVE_LEFT; i++)
        {
            yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
            other.position = new Vector3(other.position.x - DeltaRobotIKUtils.SLIDER_INCREMENT, other.position.y, other.position.z);
        }
        for (int i = 0; i < DeltaRobotIKUtils.MOVE_DOWN; i++)
        {
            yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
            other.position = new Vector3(other.position.x, other.position.y - DeltaRobotIKUtils.SLIDER_INCREMENT, other.position.z);
        }
        isTriggered = false;
    }
}
