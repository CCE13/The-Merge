using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Cubes;

public class CameraFollow : MonoBehaviour
{
    public Vector3 camPivot = Vector3.zero;
    public Vector3 camRotation = new Vector3(45, 35, 0);

    public float camSpeed = 5.0f;
    public float camDistance = 5.0f;
    public float zoomDistance;
    public Vector3 camOffset;

    public GameObject target;

    private Vector3 newPos;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }
    private void Update()
    {
        //zooms the camera in when the dialogue is running.
        if (DialogueController.isRunning)
        {
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, zoomDistance, Time.deltaTime * camSpeed);
            return;
        }
        else
        {
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, camDistance, Time.deltaTime * camSpeed);
        }


        if(mainCamera.orthographicSize != camDistance) { return; }

        //moves the camera to the target position.
        camPivot = target.transform.position;
        newPos = camPivot;

        //sets the rotation of the camera.
        transform.eulerAngles = camRotation;

        
        if (mainCamera.orthographic)
        {
            newPos += -transform.forward * camDistance * 4F;
            newPos += camOffset;
            mainCamera.orthographicSize = camDistance;
        }

        //moves the camera to the new position which takes in a offset as well
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * camSpeed);

    }
}