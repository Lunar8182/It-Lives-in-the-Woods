using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCam : MonoBehaviour
{
    //sensitivity for the mouse movement
    public float sensX;
    public float sensY;

    public Transform orientation;

    //rotation variables for the camera
    float xRotation;
    float yRotation;

    private void Start()
    {
        //locks the cursor to the middle of the screen on start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //get mouse input and multiply it by the sensitivity and delta time to make it frame rate independent
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //add the rotation to the camera rotation variables
        yRotation += mouseX;
        xRotation -= mouseY;

        //clamp the x rotation to prevent the camera from flipping over
        //and also allow 90 degree rotation up and down and left and right
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate the camera and the orientation of the player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
