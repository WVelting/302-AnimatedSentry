using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public float visionDistance = 30;

    public TargetableObject target {get; private set;}
    public bool playerWantsToAim {get; private set;} = false;

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownSelect = 0;

    void Start()
    {
        
    }

    void Update()
    {
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownSelect -= Time.deltaTime;

        if(playerWantsToAim) 
        {
            if(cooldownScan <= 0) ScanForTargets();
            if(cooldownSelect <= 0) PickATarget();
        }
        else target = null;

        print(target);
        
    }

    void ScanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach(TargetableObject thing in things)
        {
            Vector3 vToThing = thing.transform.position - transform.position;
            if(vToThing.sqrMagnitude < visionDistance * visionDistance) 
            {
                float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

                //within 180 degrees
                if(alignment > .4f) validTargets.Add(thing);
            }
        }
        
    }

    void PickATarget()
    {

        if(target) return;
        cooldownSelect = .5f;

        float closestDistanceSoFar = visionDistance;

        foreach(TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            float dis = vToThing.sqrMagnitude;

            if(dis<closestDistanceSoFar) 
            {
                closestDistanceSoFar = dis;
                target = thing;
            }

        }
    }
}
