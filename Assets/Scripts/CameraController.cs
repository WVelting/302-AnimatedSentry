using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    public PlayerTargeting player;
    private Camera cam;

    private float pitch;
    private float yaw;
    public float dollyDis = 10;

    public float mouseSense = 5;
    public float scrollSense = 2;
    public Vector3 lookOffset = new Vector3(0, 1, 0);

    //how many seconds to shake...
    private float shakeTimer = 0;


    public float percentToLerp = .01f;
    void Start()
    {
       cam = GetComponentInChildren<Camera>(); 
       Cursor.lockState = CursorLockMode.Confined;

       if(player == null)
       {
           PlayerTargeting script = FindObjectOfType<PlayerTargeting>();
           if(script != null) player = script;
       }
    }

    void Update()
    {
        bool isAiming = false;

        if(player && player.target && player.playerWantsToAim) isAiming = true;
        
        float playerYaw = AniMath.AngleWrapDegrees(yaw, player.transform.eulerAngles.y);
        

        //set rig rotation
        if (isAiming)
        {
            

            Quaternion tempTarget = Quaternion.Euler(0, playerYaw, 0);

            transform.rotation = AniMath.Ease(transform.rotation, tempTarget, .01f);

        }
        else{
            float mX = Input.GetAxisRaw("Mouse X");
            float mY = - Input.GetAxisRaw("Mouse Y");

            yaw += mX * mouseSense;
            pitch += mY * mouseSense;

            //if (yaw> 360) yaw -= 360;
            //if (yaw< 360) yaw += 360;

            pitch = Mathf.Clamp(pitch, -10, 45);


            transform.rotation = AniMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .005f);
        }

    
        dollyDis += Input.mouseScrollDelta.y * scrollSense;
        dollyDis = Mathf.Clamp(dollyDis, 3, 10);

        float tempZ = isAiming? 2 : dollyDis;

        cam.transform.localPosition = AniMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -tempZ), .02f);

        //4. rotate camera object
        if(isAiming){

            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;
            Quaternion worldRot = Quaternion.LookRotation(vToAimTarget);

            Quaternion localRot = worldRot;
            if(cam.transform.parent)
            {
                localRot = Quaternion.Inverse(cam.transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;
            euler.z = 0;
            localRot.eulerAngles = euler;

            cam.transform.localRotation = AniMath.Ease(cam.transform.localRotation, localRot, .005f);
        }
        else{
            cam.transform.localRotation = AniMath.Ease(cam.transform.localRotation, Quaternion.identity, .005f);
        }

        UpdateShake();
    }

    void UpdateShake()
    {
        if(shakeTimer < 0) return;

        shakeTimer -= Time.deltaTime;

        float p = shakeTimer / 1;
        p = p*p;
        p = AniMath.Lerp(1, .98f, p);

        Quaternion randomRot = AniMath.Lerp(Random.rotation, Quaternion.identity, p);
        cam.transform.localRotation  *= randomRot;
    }

    public void Shake(float time)
    {
        if(time > shakeTimer) shakeTimer = time;
    }

    private void LateUpdate(){
        if(player){
            transform.position = AniMath.Ease(transform.position, player.transform.position + lookOffset, percentToLerp);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
