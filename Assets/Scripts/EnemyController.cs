using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    CharacterController pawn;
    NavMeshAgent agent;

    Transform navTarget;
    public Transform fLeftLeg;
    public Transform bLeftLeg;
    public Transform fRightLeg;
    public Transform bRightLeg;

    public float visionDistance = 10;
    private bool movingForward;
    private Vector3 inputDir;
    
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        

        PlayerTargeting player = FindObjectOfType<PlayerTargeting>();

        navTarget = player.transform;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(navTarget) 
        {
            Vector3 vToTarget = navTarget.position - transform.position;
            float disToTarget = vToTarget.sqrMagnitude;
            if(disToTarget < (visionDistance*visionDistance)) agent.destination = navTarget.position;
            else agent.destination = transform.position;
        }
    }

    void WalkAnim()
    {
        inputDir = transform.forward + transform.right;
        if(inputDir.sqrMagnitude > 1) inputDir.Normalize();

        float degrees = 30;
        float speed = 10;


        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        if(alignment == 1) movingForward = true;
        else movingForward = false;
        alignment = Mathf.Abs(alignment);
        degrees = AniMath.Lerp(10, 30, alignment);
        
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        fLeftLeg.localRotation = Quaternion.AngleAxis(wave, axis);
        fRightLeg.localRotation = Quaternion.AngleAxis(-wave, axis);
        bLeftLeg.localRotation = Quaternion.AngleAxis(wave, axis);
        bRightLeg.localRotation = Quaternion.AngleAxis(-wave, axis);
    }
}
