using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public float visionDistance = 30;

    [Range(1, 20)]
    public int roundsPerSecond = 5;

    public Transform boneShoulderRight;
    public Transform boneShoulderLeft;

    public TargetableObject target { get; private set; }
    public bool playerWantsToAim { get; private set; } = false;
    public bool playerWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownSelect = 0;
    private float cooldownAttack = 0;


    void Update()
    {
        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownSelect -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim)
        {
            if(target != null) if(!CanSeeThing(target)) target = null;
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownSelect <= 0) PickATarget();
        }
        else target = null;

        print(target);

        DoAttack();

    }

    void ScanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach (TargetableObject thing in things)
        {
            if (CanSeeThing(thing)) validTargets.Add(thing);

        }

    }

    private bool CanSeeThing(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position;
        if (vToThing.sqrMagnitude > visionDistance * visionDistance)
        {
            return false;
        }
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        //within 180 degrees
        if (alignment > .4f) return true;
        else return false;

    }

    void DoAttack()
    {
        if(cooldownAttack > 0) return;
        if(!playerWantsToAim) return;
        if(!playerWantsToAttack) return;
        if(target == null) return;
        if(!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        //spawn projectiles....
        //or take health away from target
        boneShoulderLeft.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.localEulerAngles += new Vector3(-30, 0, 0);

    }

    void PickATarget()
    {

        if (target) return;
        cooldownSelect = .5f;

        float closestDistanceSoFar = visionDistance;

        foreach (TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            float dis = vToThing.sqrMagnitude;

            if (dis < closestDistanceSoFar)
            {
                closestDistanceSoFar = dis;
                target = thing;
            }

        }
    }
}
