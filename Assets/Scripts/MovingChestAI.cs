using UnityEngine;
using System.Collections.Generic;

public class MovingChestAI : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 3f;
    public float turnSpeed = 5f;
    public float waypointTolerance = 0.5f;

    [Header("Waypoints")]
    public List<Transform> waypoints;

    private int currentWPIndex = 0;
    private bool isMovingForward = true;

    void Start()
    {
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWP = waypoints[currentWPIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetWP.position, moveSpeed * Time.deltaTime);

        Vector3 dir = targetWP.position - transform.position;
        dir.y = 0; // Abaikan tinggi
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }

        if (Vector3.Distance(transform.position, targetWP.position) <= waypointTolerance)
        {
            UpdateWaypointIndex();
        }
    }

    void UpdateWaypointIndex()
    {
        if (isMovingForward)
        {
            if (currentWPIndex >= waypoints.Count - 1)
            {
                isMovingForward = false; 
                currentWPIndex--;
            }
            else
            {
                currentWPIndex++;
            }
        }
        else
        {
            if (currentWPIndex <= 0)
            {
                isMovingForward = true; 
                currentWPIndex++;
            }
            else
            {
                currentWPIndex--;
            }
        }
    }
}