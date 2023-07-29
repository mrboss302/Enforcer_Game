using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootingActions {LookRight, LookLeft, JumpRight, JumpLeft, SlidingRight, SlidingLeft }

public class ActionTrigger : MonoBehaviour
{
    public ShootingActions motionAction;
    public float activeDistance = 15;
    public int allowBullet = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            GameManager.Instance.Player.MotionAction(this);
            //gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * activeDistance + Vector3.up * 1);
        Gizmos.DrawSphere(transform.position + Vector3.right * activeDistance, 1);
    }
}
