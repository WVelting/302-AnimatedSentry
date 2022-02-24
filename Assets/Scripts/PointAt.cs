using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Axis
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down
}
public class PointAt : MonoBehaviour
{

    public Axis aimOrientation = Axis.Backward;
    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;

    private Quaternion startRotation;
    private Quaternion goalRotation;
    private Vector3 fromVector = new Vector3();
    private PlayerTargeting playerTargeting;
    void Start()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        startRotation = transform.localRotation;

    }

    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if(playerTargeting && playerTargeting.target && playerTargeting.playerWantsToAim)
        {
            Vector3 vToTarget = playerTargeting.target.transform.position - transform.position;
            
            vToTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            //convert to local-space:
            Quaternion localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;

            Vector3 euler = localRot.eulerAngles;
            if(lockAxisX) euler.x = startRotation.eulerAngles.x;
            if(lockAxisY) euler.y = startRotation.eulerAngles.y;
            if(lockAxisZ) euler.z = startRotation.eulerAngles.z;


            goalRotation = localRot;
        }
        else goalRotation = startRotation;

        transform.localRotation = AniMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}
