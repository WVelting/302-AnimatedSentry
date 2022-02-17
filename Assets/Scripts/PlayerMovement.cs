using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    CharacterController pawn;
    public Camera cam;
    public float walkSpeed = 5;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    void Update()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        
        
        if(cam) {
            if(v!=0 || h!= 0){
                float playerYaw = transform.eulerAngles.y;
                float camYaw = cam.transform.eulerAngles.y;

                if(camYaw>playerYaw + 180) camYaw -= 360;

                Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);
                transform.rotation = AniMath.Ease(transform.rotation, targetRotation, .01f);
            }
        }

        Vector3 moveDir = transform.forward * v + transform.right * h;
        if(moveDir.sqrMagnitude > 1) moveDir.Normalize();

        pawn.SimpleMove(moveDir * walkSpeed);

        

    }
}
