using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    FPSPlayer FPSPlayer;
    RaycastHit hit;
    
    [SerializeField] float viewRangeY = 80; // THE RANGE THE PLAYER CAN LOOK UP AND DOWN IN THE Y AXIS
    [HideInInspector]
    [SerializeField] Transform cameraTransform;
    float rotX;
    float rotY;

    void Start()
    {
        FPSPlayer = FindObjectOfType<FPSPlayer>();
        cameraTransform = Camera.main.transform;
    }
    
    void Update()
    {
        MoveView();
    }

    private void MoveView() // CAMERA ROTATION AND CLAMPING
    {
        rotX += Input.GetAxis("Mouse X") * FPSPlayer.mouseSensitivityX;
        rotY += Input.GetAxis("Mouse Y") * FPSPlayer.mouseSensitivityY;
        rotY = Mathf.Clamp(rotY, -viewRangeY, viewRangeY);
        cameraTransform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
    }

    public void Raycast() // RAYCASTING
    {
        //Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * FPSPlayer.rangeCameraRay);  //RayCast Debug

        Ray myRay = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(myRay, out hit, FPSPlayer.rangeCameraRay))
        {
            Debug.Log(hit.collider.gameObject.name + " has as tag "+ hit.collider.tag);
            if (hit.collider.CompareTag("WorldWeapon") && FPSPlayer.reloading == false)
            {
                FPSPlayer.GrabNewWeapon(hit.collider.GetComponent<Weapon>());
            }
        }
    }

    public void Death()
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        this.enabled = false;
    }

}
