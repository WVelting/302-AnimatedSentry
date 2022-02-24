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

    private Quaternion startRotation;
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
            
            Vector3 fromVector;
            switch (aimOrientation)
            {
                case Axis.Forward: fromVector = Vector3.forward; break;
                case Axis.Backward: fromVector = Vector3.back; break;
                case Axis.Right: fromVector = Vector3.right; break;
                case Axis.Left: fromVector = Vector3.left; break;
                case Axis.Up: fromVector = Vector3.up; break;
                case Axis.Down: fromVector = Vector3.down; break;

                
            }
            transform.rotation = Quaternion.FromToRotation(Vector3.down, vToTarget);
            
        }
        else transform.localRotation = startRotation;
    }
}
