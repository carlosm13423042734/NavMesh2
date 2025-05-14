using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float velocity;
    public float force;
    private float randomOffset;

    private float initialPos;

    void Start()
    {
        randomOffset = Random.Range(0f, 7f);
        initialPos = transform.position.x;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = initialPos + Mathf.Sin(Time.time * velocity + randomOffset) * force;
        transform.position = pos;
    }
}


