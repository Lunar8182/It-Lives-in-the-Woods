using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //creates a variable for the camera position
    public Transform cameraPostion;

    private void Update()
    {
        //updates the camera position to the position of the cameraPostion variable
        //which is on the orientation of the player.
        transform.position = cameraPostion.position;
    }
}
