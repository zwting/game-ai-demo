using UnityEngine;

public abstract class SteerBehaviour
{
    public abstract Vector3 CalcSteerForce(Vehicle vehicle);
}

public class SeekSteer : SteerBehaviour
{
    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        Vector3 desired = vehicle.target - vehicle.transform.position;
        desired = desired.normalized * vehicle.MAX_SPEED;
        return Vector3.ClampMagnitude(desired - vehicle.velocity, vehicle.MAX_FORCE);
    }
}


public class ArriveSteer : SteerBehaviour
{
    public override Vector3 CalcSteerForce(Vehicle vehicle)
    {
        Vector3 desired = vehicle.target - vehicle.transform.position;

        float dis = desired.magnitude;
        if (dis < vehicle.DECELERATE_RADIUS)
        {
            desired = desired.normalized * ((dis / vehicle.DECELERATE_RADIUS) * vehicle.MAX_SPEED);
        }
        else
        {
            desired = desired.normalized * vehicle.MAX_SPEED;
        }

        return Vector3.ClampMagnitude(desired - vehicle.velocity, vehicle.MAX_FORCE);
    }
}