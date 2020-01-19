using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class SteerBehaviour
{
    public abstract Vector3 CalcSteerForce(Vehicle vehicle);

    protected Vector3 Seek(Vehicle vehicle, Vector3 targetPos)
    {
        var desired = targetPos - vehicle.transform.position;
        desired = desired.normalized * vehicle.MAX_SPEED;
        return Vector3.ClampMagnitude(desired - vehicle.velocity, vehicle.MAX_FORCE); 
    }
}

/// <summary>
/// 靠近行为
/// </summary>
public class SeekSteer : SteerBehaviour
{
    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        return Seek(vehicle, vehicle.target);
    }
}


/// <summary>
/// 抵达行为
/// </summary>
public class ArriveSteer : SteerBehaviour
{
    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        Vector3 desired = vehicle.target - vehicle.transform.position;

        float dis = desired.magnitude;
        if (dis < vehicle.DECELERATE_RADIUS)
        {
            desired = desired.normalized * (Curver01(dis / vehicle.DECELERATE_RADIUS) * vehicle.MAX_SPEED);
        }
        else
        {
            desired = desired.normalized * vehicle.MAX_SPEED;
        }

        return Vector3.ClampMagnitude(desired - vehicle.velocity, vehicle.MAX_FORCE);
    }

    private float Curver01(float val)
    {
        val = Mathf.Clamp01(val);
        return Mathf.Sin(val * Mathf.PI - Mathf.PI * 0.5f) * 0.5f + 0.5f;
    }
}

/// <summary>
/// 徘徊行为
/// </summary>
public class WanderSteer : SteerBehaviour
{
    private float _wanderDis = 5f;
    private float _wanderRadius = 2.4f;
    private float _wanderJitter = 60f;

    public Vector3 wanderTarget { private set; get; }

    // private float _angle = 90f;

    public WanderSteer()
    {
        // _angle += Random.Range(-0.3f, 0.3f);
        wanderTarget = new Vector3(0, 0, _wanderRadius);
    }

    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        // _angle += Random.Range(-0.3f, 0.3f);
        // wanderTarget = new Vector3(Mathf.Cos(_angle), 0, Mathf.Sin(_angle)) * _wanderRadius;
        var dj = _wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(Random.Range(-dj, dj), 0, Random.Range(-dj, dj));
        wanderTarget = wanderTarget.normalized * _wanderRadius;
        var desired =
            Vector3.ClampMagnitude(vehicle.velocity.normalized * _wanderDis + wanderTarget, vehicle.MAX_SPEED);

        return Vector3.ClampMagnitude(desired - vehicle.velocity, vehicle.MAX_FORCE);
    }
}

/// <summary>
/// 追逐行为
/// </summary>
public class PursuitSteer : SteerBehaviour
{
    /// <summary>
    /// 面对角度： 在此范围内认为是和目标面对面
    /// </summary>
    private float _faceAngle = 20f;

    private float _cosFaceAngle = 0;

    public PursuitSteer()
    {
        _cosFaceAngle = Mathf.Cos(_faceAngle * Mathf.Deg2Rad);
    }

    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        var targetVehicle = vehicle.TargetVehicle;
        if (targetVehicle == null)
        {
            return Vector3.zero;
        }

        var toTarget = targetVehicle.transform.position - vehicle.transform.position;

        //面对面了
        if (Vector3.Dot(toTarget, vehicle.velocity) > 0
            && Vector3.Dot(targetVehicle.transform.forward, vehicle.transform.forward) < _cosFaceAngle)
        {
            return Seek(vehicle, targetVehicle.transform.position);
        }

        float lookAheadTime = toTarget.magnitude / (vehicle.Speed + targetVehicle.Speed);
        return Seek(vehicle, targetVehicle.transform.position + targetVehicle.velocity * lookAheadTime);
    }
}