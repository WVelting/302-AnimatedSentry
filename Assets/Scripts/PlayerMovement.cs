using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    CharacterController pawn;
    PlayerTargeting targetingScript;
    PlayerHealthManager health;
    
    public Camera cam;
    public float walkSpeed = 5;

    [Range(-10, -1)]
    public float gravMult = -1;
    public float jumpImpulse = 5;


    public Transform boneLegLeft;
    public Transform boneLegRight;
    public Transform boneShoulderLeft;
    public Transform boneShoulderRight;
    public Transform boneHip;
    public Transform boneSpine;
    public GameObject decoy;
    public GameObject head;

    private Vector3 inputDir;
    private float velocityVertical = 0;

    private float cooldownJumpWindow = 0;
    private bool movingForward;
    private float speed = 10;
    private float deathCountdown = 3;
    public bool IsGrounded {
        get {
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }
    private bool isDead = false;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();
        health = GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) health.playerHealth -= 250;
        if(health.playerHealth <=0) PlayerDie();
    
        else{

            if(Input.GetButton("Fire3"))
            {
                walkSpeed = 10;
                boneSpine.localRotation = AniMath.Ease(boneSpine.localRotation, Quaternion.Euler(25, 0, 0), .01f);
                speed = 20;

            } 
            else{
                boneSpine.localRotation = AniMath.Ease(boneSpine.localRotation, Quaternion.identity, .01f);
                walkSpeed = 5;
                speed = 10;
            }
            bool isDecoy = FindObjectOfType<Decoy>();

            if(cooldownJumpWindow > 0) cooldownJumpWindow -=Time.deltaTime;

            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            
            bool playerIsAiming = (targetingScript && targetingScript.playerWantsToAim && targetingScript.target);
            
            if(playerIsAiming){
                
                Vector3 toTarget = targetingScript.target.transform.position - transform.position;
                toTarget.Normalize();
                Quaternion worldRot = Quaternion.LookRotation(toTarget);

                Vector3 euler = worldRot.eulerAngles;
                euler.x = 0;
                euler.z = 0;
                worldRot.eulerAngles = euler;

                transform.rotation = AniMath.Ease(transform.rotation, worldRot, .01f);
            }

            else if(cam) {
                if(v!=0 || h!= 0){
                    float playerYaw = transform.eulerAngles.y;
                    float camYaw = cam.transform.eulerAngles.y;

                    while(camYaw>playerYaw + 180) camYaw -= 360;
                    while(camYaw<playerYaw - 180) camYaw += 360;

                    Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0);
                    Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);
                    transform.rotation = AniMath.Ease(playerRotation, targetRotation, .01f);
                }
            }

            bool wantsToJump = Input.GetButtonDown("Jump");
            if(pawn.isGrounded && wantsToJump) velocityVertical = jumpImpulse;

            inputDir = transform.forward * v + transform.right * h;
            if(inputDir.sqrMagnitude > 1) inputDir.Normalize();

            velocityVertical += gravMult * Time.deltaTime;

            Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;

            pawn.Move(moveAmount * Time.deltaTime);
            if(pawn.isGrounded) {
                velocityVertical = 0;
                cooldownJumpWindow = .5f;
                WalkAnim();
            }
            
            
            

            if(Input.GetButtonDown("Decoy"))
            {
                if(!isDecoy)
                {
                    Instantiate(decoy, transform.position, transform.rotation);
                    pawn.Move(transform.right * 5);

                }

            }
        }

    }

    void WalkAnim()
    {
        float degrees = 30;
        


        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        if(alignment == 1) movingForward = true;
        else movingForward = false;
        print(movingForward);
        alignment = Mathf.Abs(alignment);
        degrees = AniMath.Lerp(10, 30, alignment);
        
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

        
        if(inputDir != Vector3.zero){
        
            float offsetY = Mathf.Sin(Time.time * speed) * .05f;
            if(boneHip) boneHip.localPosition = new Vector3(0, offsetY, 0);
            if(!targetingScript.playerWantsToAim && alignment == 1)
            {
                if(movingForward)
                {
                    if(boneShoulderLeft) boneShoulderLeft.localRotation = Quaternion.AngleAxis(-wave+90, axis);
                    if(boneShoulderRight) boneShoulderRight.localRotation = Quaternion.AngleAxis(wave+90, axis);

                }

                else{
                    if(boneShoulderLeft) boneShoulderLeft.localRotation = Quaternion.AngleAxis(-wave-90, axis);
                    if(boneShoulderRight) boneShoulderRight.localRotation = Quaternion.AngleAxis(wave-90, axis);

                }
            }
        } 

        if(inputDir == Vector3.zero)
        {
            float offsetY = Mathf.Sin(Time.time * speed) * .01f;
            if(boneSpine)boneSpine.localPosition += new Vector3(0, offsetY, 0);
        }
            

    }

    void AirAnim()
    {
        if(boneShoulderLeft) boneShoulderLeft.localRotation = AniMath.Ease(boneShoulderLeft.localRotation, Quaternion.Euler(35, 0, 0), .01f);
        if(boneShoulderRight) boneShoulderRight.localRotation = AniMath.Ease(boneShoulderRight.localRotation, Quaternion.Euler(-35, 0, 0), .01f);        
        if(boneLegLeft) boneLegLeft.localRotation = AniMath.Ease(boneLegLeft.localRotation, Quaternion.Euler(35, 0, 0), .01f);
        if(boneLegRight) boneLegRight.localRotation = AniMath.Ease(boneLegRight.localRotation, Quaternion.Euler(-35, 0, 0), .01f);


    }

    public void PlayerDie()
    {
        walkSpeed = 0;
        transform.localRotation = AniMath.Ease(transform.localRotation, Quaternion.Euler(90,0,0), .05f);
        if(!isDead)
        {
        transform.localPosition -= new Vector3(0, .5f, 0);
        head.AddComponent<Rigidbody>();
        isDead = true;
        }
        deathCountdown -= Time.deltaTime;
        if (deathCountdown <= 0) Destroy(this.gameObject);
        
        
        
        

    }
}
