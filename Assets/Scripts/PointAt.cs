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
        startRotation = transform.localRotation;
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        
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
            

            switch(aimOrientation)
            {
                case Axis.Forward: fromVector = Vector3.down; break;
                case Axis.Backward: fromVector = Vector3.up; break;
                case Axis.Left: fromVector = Vector3.forward; break;
                case Axis.Right: fromVector = Vector3.back; break;
                case Axis.Up: fromVector = Vector3.left; break;
                case Axis.Down: fromVector = Vector3.right; break;
            }

            Quaternion worldRot = Quaternion.FromToRotation(fromVector, vToTarget);
            Quaternion localRot = worldRot;
            if(transform.parent) localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            
            Vector3 euler = localRot.eulerAngles;
            if(lockAxisX) euler.x = startRotation.eulerAngles.x;
            if(lockAxisY) euler.y = startRotation.eulerAngles.y;
            if(lockAxisZ) euler.z = startRotation.eulerAngles.z;

            localRot.eulerAngles = euler;


            goalRotation = localRot;
        }
        else goalRotation = startRotation;
        transform.localRotation = AniMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}
