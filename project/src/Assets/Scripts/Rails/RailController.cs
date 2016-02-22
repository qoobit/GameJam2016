using UnityEngine;
using System.Collections.Generic;

public class RailController : MonoBehaviour
{
    public Transform target;
    public bool loop = false;

    public List<RailWaypoint> railWaypoints;
    public int currentWaypointIndex = -1;
    public float currentWaypointStartTime = 0f;


    void Start()
    {
        initializeRailWaypoints();
    }

    void Update()
    {
        updateTargetPosition();
    }

    private void initializeRailWaypoints()
    {
        railWaypoints = new List<RailWaypoint>(GetComponentsInChildren<RailWaypoint>());
        railWaypoints.Sort(delegate (RailWaypoint a, RailWaypoint b)
        {
            return a.index.CompareTo(b.index);
        });

        if (currentWaypointIndex < 0)
        {
            currentWaypointIndex = 0;
            currentWaypointStartTime = Time.time;
        }
    }

    private int getNextWaypointIndex()
    {
        if (loop)
            return (currentWaypointIndex + 1) % railWaypoints.Count;

        return (currentWaypointIndex + 1);
    }

    private RailWaypoint getWaypoint(int index)
    {
        if (index >= railWaypoints.Count)
            return null;

        return railWaypoints[index];
    }

    private RailWaypoint getCurrentWaypoint()
    {
        return getWaypoint(currentWaypointIndex);
    }

    private RailWaypoint getNextWaypoint()
    {
        return getWaypoint(getNextWaypointIndex());
    }

    private void updateTargetPosition()
    {
        if (target == null || railWaypoints.Count == 0)
            return;

        RailWaypoint currentWaypoint = getCurrentWaypoint();
        RailWaypoint nextWaypoint = getNextWaypoint();

        if (currentWaypoint == null || nextWaypoint == null)
            return;

        float deltaTime = Time.time - currentWaypointStartTime;
        float travelTime = getTimeToNextWaypoint(currentWaypoint, nextWaypoint);
        while (deltaTime > travelTime)
        {
            deltaTime -= travelTime;
            currentWaypointStartTime = Time.time;

            currentWaypointIndex = getNextWaypointIndex();
            currentWaypoint = nextWaypoint;
            nextWaypoint = getNextWaypoint();
            if (nextWaypoint == null)
            {
                travelTime = 0f;
                break;
            }
            travelTime = getTimeToNextWaypoint(currentWaypoint, nextWaypoint);
        }
        target.position = interpolatePosition(currentWaypoint, nextWaypoint, deltaTime, travelTime);
    }

    private float getTimeToNextWaypoint(RailWaypoint currentWaypoint, RailWaypoint nextWaypoint)
    {
        switch (nextWaypoint.type)
        {
            case RailWaypoint.Type.TIME:
                return nextWaypoint.value;

            case RailWaypoint.Type.VELOCITY:
                return (nextWaypoint.transform.position - currentWaypoint.transform.position).magnitude / nextWaypoint.value;

            default:
                return 0f;
        }
    }

    private Vector3 interpolatePosition(RailWaypoint currentWaypoint, RailWaypoint nextWaypoint, float deltaTime, float travelTime)
    {
        //Have a look at this
        //http://answers.unity3d.com/questions/12689/moving-an-object-along-a-bezier-curve.html

        if (nextWaypoint == null)
            return currentWaypoint.transform.position;

        if (travelTime <= 0f)
            return nextWaypoint.transform.position;

        return Vector3.Lerp(currentWaypoint.transform.position, nextWaypoint.transform.position, deltaTime / travelTime);
    }
}
