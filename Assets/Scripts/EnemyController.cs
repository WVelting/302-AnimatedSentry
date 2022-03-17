using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    CharacterController pawn;
    NavMeshAgent agent;

    Transform navTarget;
    

    public PointAt target;
    public Transform fLeftLeg;
    public Transform bLeftLeg;
    public Transform fRightLeg;
    public Transform bRightLeg;
    public Transform body;
    public GameObject gunSwivel;

    public float attackDistance = 10;
    private bool movingForward;
    private Vector3 inputDir;
    private PlayerTargeting player;
    public PointAt gunTarget;
    
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        

        

        player = FindObjectOfType<PlayerTargeting>();

        navTarget = player.transform;

        
    }

    // Update is called once per frame
    void Update()
    {

        if(navTarget) 
        {
            Vector3 vToTarget = navTarget.position - transform.position;
            float disToTarget = vToTarget.sqrMagnitude;
            target.target = navTarget;
            if(disToTarget > (attackDistance*attackDistance)) 
            {
                agent.destination = navTarget.position;
                gunTarget.target = null;
                
                WalkAnim();
            }
            else 
            {
                agent.destination = transform.position;
                
                gunTarget.target = navTarget;
                
            }
        }

        Decoy decoy = FindObjectOfType<Decoy>();
        

        if(decoy) navTarget = decoy.transform;
        else navTarget = player.transform;
        
    }

    void WalkAnim()
    {
        inputDir = transform.forward + transform.right;
        if(inputDir.sqrMagnitude > 1) inputDir.Normalize();

        float degrees = 10;
        float speed = 5;


        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);
        degrees = AniMath.Lerp(-10, 10, alignment);
        
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        fLeftLeg.localRotation = Quaternion.Euler(-wave, 0, 0);
        fRightLeg.localRotation = Quaternion.Euler(-wave, 0, 0);
        bLeftLeg.localRotation = Quaternion.Euler(wave, 0, 0);
        bRightLeg.localRotation = Quaternion.Euler(wave, 0, 0);
        
        float offsetY = Mathf.Sin(Time.time * speed) * .0005f;
        if(body)body.localPosition += new Vector3(0, offsetY, 0);
    }
}
