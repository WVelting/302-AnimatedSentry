using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public float visionDistance = 30;

    [Range(1, 20)]
    public int roundsPerSecond = 5;

    public PointAt boneShoulderRight;
    public PointAt boneShoulderLeft;
    public PointAt boneSpineTop;

    public TargetableObject target { get; private set; }
    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownSelect = 0;
    private float cooldownAttack = 0;
    private CameraController cam;

    void Start()
    {
        cam = FindObjectOfType<CameraController>();
    }


    void Update()
    {
        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownSelect -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim)
        {
            if(target != null) 
            {
                Vector3 toTarget = target.transform.position - transform.position;
                toTarget.y = 0;
                if(toTarget.magnitude > 3 && !CanSeeThing(target)) target = null;
            }
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownSelect <= 0) PickATarget();
        }
        else target = null;

        if(boneShoulderLeft) boneShoulderLeft.target = target ? target.transform : null;
        if(boneShoulderRight) boneShoulderRight.target = target ? target.transform : null;
        if(boneSpineTop) boneSpineTop.target = target ? target.transform : null;


        DoAttack();

    }

    void ScanForTargets()
    {

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach (TargetableObject thing in things)
        {
            if (CanSeeThing(thing)) validTargets.Add(thing);

        }
        cooldownScan = .01f;

    }

    private bool CanSeeThing(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position;
        if (vToThing.sqrMagnitude > (visionDistance * visionDistance))
        {
            return false;
        }
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        //within 180 degrees
        if (alignment < .4f) return false;

        //check for occlusion...
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = vToThing;

        Debug.DrawRay(ray.origin, ray.direction * visionDistance);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, visionDistance))
        {

            bool canSee = false;
            Transform xform = (hit.transform);

            do {
                if(xform.gameObject == thing.gameObject) 
                {
                    canSee = true;
                    break;
                }
                xform = xform.parent;

            } while(xform != null);

            if(!canSee) return false;

            

        }


        return true;
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
        boneShoulderLeft.transform.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.transform.localEulerAngles += new Vector3(-30, 0, 0);

        if(cam) cam.Shake(.5f);
    }

    void PickATarget()
    {

        if (target) return;

        float closestDistanceSoFar = visionDistance * visionDistance;

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
        cooldownSelect = .01f;
    }
}
