using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public Camera camera;
    public float MAX_SPEED = 4f;
    public float MAX_FORCE = 0.1f;
    public float DECELERATE_RADIUS = 10f;

    public Vector3 velocity { get; set; }

    public Vector3 acceleration { get; set; }

    public Vector3 target { private set; get; }

    private List<SteerBehaviour> _steerList = new List<SteerBehaviour>();

    /// <summary>
    /// 自身距离地面的高度
    /// </summary>
    private float _heightOffGround = 0.46f;

    void Start()
    {
        // _steerList.Add(new SeekSteer());
        _steerList.Add(new ArriveSteer());
        _heightOffGround = transform.position.y;
    }

    void Update()
    {
        for (int i = 0; i < _steerList.Count; ++i)
        {
            var force = _steerList[i].CalcSteerForce(this);
            ApplyForce(force);
        }

        velocity += acceleration;
        if (velocity.magnitude > MAX_SPEED)
        {
            velocity = velocity.normalized * MAX_SPEED;
        }

        transform.position += velocity * Time.deltaTime;
        acceleration = Vector3.zero;

        TurnAround();

        GetTarget();
    }

    private void TurnAround()
    {
        var dir = velocity.normalized;
        dir.y = 0;
        transform.rotation  = Quaternion.FromToRotation(Vector3.forward, dir);
    }

    private void GetTarget()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit rch, 100, 1 << LayerMask.NameToLayer($"Ground")))
            {
                Debug.DrawLine(ray.origin, rch.point, Color.yellow);
                var tar = rch.point;
                tar.y = _heightOffGround;
                target = tar;
            }
        }
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }
}