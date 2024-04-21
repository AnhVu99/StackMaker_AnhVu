using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 camPos = target.position + offset;
            Vector3 smooothPos = Vector3.Lerp(transform.position, camPos, smoothTime);
            transform.position = smooothPos;
            transform.LookAt(target);
        }
    }
}
