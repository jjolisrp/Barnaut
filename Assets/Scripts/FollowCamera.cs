using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;

    public float baseSpeed = 50;
    public float height = 20;
    public float distance = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.transform.TransformPoint(0, height, -distance);
        transform.position = transform.position + (targetPosition - transform.position) * baseSpeed * Time.deltaTime;
        transform.rotation = target.rotation;
    }
}
