using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAI_FSM : MonoBehaviour
{
    public enum AIState { Patroli, Kejar, Serang }
    public AIState currentState;

    [Header("Referensi")]
    public EnemyMovement movement;
    public MeshRenderer meshRenderer;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    [Header("Waypoint System")]

    public List<Transform> waypoints;
    private int currentWPIndex = 0;
    public float waypointTolerance = 1.0f;

    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;

    [Header("Damage")]
    public float contactDamage = 10f;
    public float damageCooldown = 1f;
    private float lastDamageTime;

    [Header("Visual")]
    public Material patrolMat;
    public Material chaseMat;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        switch (currentState)
        {
            case AIState.Patroli:
                HandlePatrol();
                break;
            case AIState.Kejar:
                HandleChase();
                break;
            case AIState.Serang:
                HandleAttack();
                break;
        }
    }

    void HandlePatrol()
    {
        if (meshRenderer) meshRenderer.material = patrolMat;
        movement.moveSpeed = patrolSpeed;

        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            currentState = AIState.Kejar;
            return;
        }

        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWP = waypoints[currentWPIndex];
        movement.MoveTo(targetWP.position);

        if (Vector3.Distance(transform.position, targetWP.position) <= waypointTolerance)
        {
            currentWPIndex = (currentWPIndex + 1) % waypoints.Count;
        }
    }

    void HandleChase()
    {
        if (meshRenderer) meshRenderer.material = chaseMat;
        movement.moveSpeed = chaseSpeed;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= attackRange) { currentState = AIState.Serang; return; }
        if (dist > detectionRange * 1.5f) { currentState = AIState.Patroli; return; }

        movement.MoveTo(playerTransform.position);
    }

    void HandleAttack()
    {
        movement.Stop(); 

        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange * 1.2f)
        {
            currentState = AIState.Kejar;
            return;
        }

        if (Time.time >= lastDamageTime + damageCooldown)
        {
            if (playerHealth) playerHealth.TakeDamage(contactDamage);
            lastDamageTime = Time.time;
            Debug.Log("Serang Player!");
        }
    }

    public void SetWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
    }
}