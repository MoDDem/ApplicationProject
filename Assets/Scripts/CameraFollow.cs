using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 initalOffset;
    private Vector3 cameraPosition;

    void Start()
    {
        initalOffset = Camera.main.transform.position - gameObject.transform.position;
    }

    void FixedUpdate()
    {
        cameraPosition = gameObject.transform.position + initalOffset;
        Camera.main.transform.position = cameraPosition;
    }
}
