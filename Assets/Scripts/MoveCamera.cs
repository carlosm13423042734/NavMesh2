using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private float targetDistance = 5.0f;
    [SerializeField]
    private Transform target;
    [SerializeField]
    public Vector3 vectorOffset;
    private float x = 0.0f;
    public float y = 0.0f;

    float velocityX = 80.0f;
    float velocityY = 80.0f;

    float minY = 0.0f;
    float maxY = 60.0f;


  
    private void LateUpdate()
    {
        this.targetDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100.0f;

        x += Input.GetAxis("Mouse X") * Time.deltaTime * velocityX;
        y += Input.GetAxis("Mouse Y") * Time.deltaTime * velocityY;

        y = Mathf.Clamp(y, minY, maxY);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        this.transform.rotation = rotation;

        Vector3 newPosition = target.position - (rotation * Vector3.forward * this.targetDistance);
        this.transform.rotation = rotation;
        this.transform.position = newPosition;


    }
}
