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

    public Transform target;
    
    public Axis aimOrientation = Axis.Backward;
    
    public bool lockAxisX = false; 
    public bool lockAxisY = false; 
    public bool lockAxisZ = false; 

    private Quaternion startRotation;
    private Quaternion goalRotation;
    private Vector3 fromVector = new Vector3();
    
    void Start()
    {
        startRotation = transform.localRotation;

    }

    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if(target != null)
        {
            Vector3 vToTarget = target.position - transform.position;
            
            vToTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);
            Quaternion localRot = worldRot;

            if(transform.parent){

                //convert to local-space:
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }

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
