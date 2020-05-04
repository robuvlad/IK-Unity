using Assets.Scripts.DeltaRobot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherBox : MonoBehaviour
{
    [SerializeField] GameObject firstMachine = null;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Move(gameObject.transform));
    }

    private IEnumerator Move(Transform current)
    {
        for (int i = 0; i < DeltaRobotIKUtils.MOVE_MACHINE; i++)
        {
            yield return new WaitForSeconds(DeltaRobotIKUtils.TIME_BETWEEN_MOTION);
            gameObject.transform.position = new Vector3(current.position.x, current.position.y, current.position.z - DeltaRobotIKUtils.SLIDER_INCREMENT * 2);

            Vector3 firstPos = firstMachine.transform.position;
            firstMachine.transform.position = new Vector3(firstPos.x, firstPos.y, firstPos.z - DeltaRobotIKUtils.SLIDER_INCREMENT * 2);
        }
    }

}
