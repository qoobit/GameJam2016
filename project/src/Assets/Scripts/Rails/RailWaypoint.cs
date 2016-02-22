using UnityEngine;

public class RailWaypoint : MonoBehaviour
{
    public enum Type { TIME, VELOCITY };

    public Type type;
    public float value = 1f;
    public float index = 0f;
}

