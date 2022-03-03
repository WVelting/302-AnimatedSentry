using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    CharacterController pawn;
    public Camera cam;
    public float walkSpeed = 5;

    [Range(-10, -1)]
    public float gravMult = -1;
    public float jumpImpulse = 5;


    public Transform boneLegLeft;
    public Transform boneLegRight;

    private Vector3 inputDir;
    private float velocityVertical = 0;

    private float cooldownJumpWindow = 0;
    public bool IsGrounded {
        get {
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }


    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    void Update()
    {

        if(cooldownJumpWindow > 0) cooldownJumpWindow -=Time.deltaTime;

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        
        
        if(cam) {
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
            velocityVertical = -1;
            cooldownJumpWindow = .5f;
        }

        WalkAnim();
    }

    void WalkAnim()
    {
        float degrees = 30;
        float speed = 10;


        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);
        degrees = AniMath.Lerp(10, 30, alignment);
        
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

    }
}
