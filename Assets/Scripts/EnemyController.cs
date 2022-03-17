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
    public GameObject mechBase;
    public GameObject gunSwivel;
    public GameObject laser;
    public GameObject gunArm;

    public float attackDistance = 10;
    public float visionDistance = 50;
    public float attackCountdown = 10;
    private bool movingForward;
    private Vector3 inputDir;
    private PlayerTargeting player;
    public PointAt gunTarget;
    private bool isDead = false;
    private float deathCountdown = 5;
    
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

        if(isDead)
        {
            deathCountdown -= Time.deltaTime;
            if(deathCountdown<= 0) Destroy(this.gameObject);
        }
        else
        {
            if(navTarget) 
            {
                Vector3 vToTarget = navTarget.position - transform.position;
                float disToTarget = vToTarget.sqrMagnitude;
                target.target = navTarget;
                if(disToTarget > (attackDistance*attackDistance)) 
                {
                    attackCountdown = 10;
                    agent.destination = navTarget.position;
                    gunTarget.target = null;
                    target.target = player.transform;
                    
                    WalkAnim();
                }
                else if (disToTarget < (visionDistance*visionDistance))
                {
                    agent.destination = transform.position;
                    target.target = null;
                    gunTarget.target = navTarget.transform;
                    attackCountdown -= Time.deltaTime;
                    if(attackCountdown<=0) 
                    {
                        Instantiate(laser, gunTarget.transform.position, gunTarget.transform.localRotation);
                        attackCountdown = 10;
                    }
                    
                }
                else
                {
                    float offsetY = Mathf.Sin(Time.time * 5) * .0005f;
                    if(body)body.localPosition += new Vector3(0, offsetY, 0);
                }
            }

            Decoy decoy = FindObjectOfType<Decoy>();
            

            if(decoy) navTarget = decoy.transform;
            else navTarget = player.transform;

        }
        
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

    public void EnemyDie()
    {
        mechBase.AddComponent<Rigidbody>();
        gunSwivel.AddComponent<Rigidbody>();
        body.gameObject.AddComponent<Rigidbody>();
        gunArm.AddComponent<Rigidbody>();
        attackCountdown = 100;
        isDead = true;
    }
}
