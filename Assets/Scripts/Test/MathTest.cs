using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    public Transform plane;

    private Ray _ray;

    void Start()
    {
        Plane p = new Plane(Vector3.up, Vector3.down* 3);
        Debug.Log(p);
        plane.up = p.normal;
        plane.position = p.normal * -p.distance;

        _ray = new Ray(Vector3.zero, new Vector3(-1, -1f, -1));


        Debug.Log(RayCastPlane(p, _ray).magnitude);
        if (p.Raycast(_ray, out var enter))
        {
            Debug.Log("system: " + enter);
        }
    }

    private Vector3 RayCastPlane(Plane plane, Ray ray)
    {
        float denominator = Vector3.Dot(ray.direction, plane.normal);
        if (Math.Abs(denominator) < float.Epsilon)
        {
            Debug.Log("不相交");
            return Vector3.zero;
        }

        float t = -(Vector3.Dot(ray.origin, plane.normal) + plane.distance) / denominator;
        return ray.origin + t * ray.direction;
    }

    void Update()
    {
        Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction * 10, Color.yellow);
    }
}