using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    public Transform target;
    private Camera cam;

    private float pitch;
    private float yaw;
    public float dollyDis = 10;

    public float mouseSense = 5;
    public float scrollSense = 2;
    public Vector3 lookOffset = new Vector3(0, 1, 0);


    public float percentToLerp = .01f;
    void Start()
    {
       cam = GetComponentInChildren<Camera>(); 
       Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = - Input.GetAxisRaw("Mouse Y");

        yaw += mX * mouseSense;
        pitch += mY * mouseSense;
        pitch = Mathf.Clamp(pitch, -45, 45);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        dollyDis += Input.mouseScrollDelta.y * scrollSense;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        cam.transform.localPosition = AniMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -dollyDis), .02f);

        
    }

    private void LateUpdate(){
        if(target){
            transform.position = AniMath.Ease(transform.position, target.position + lookOffset, percentToLerp);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
