using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f; // Rotasi lebih cepat agar respon hindaran gesit

    [Header("Smart Whiskers Settings")]
    public float lookAhead = 3f; // Panjang kumis tengah
    public float sideLookAhead = 2f; // Panjang kumis samping (lebih pendek)
    public float avoidForce = 20f; // Kekuatan menghindar (harus lebih besar dari moveSpeed)
    public LayerMask obstacleMask; // Layer Tembok

    // Sudut kumis samping (misal 45 derajat)
    private float sideRayAngle = 45f;

    // Variable internal
    private Vector3 currentTargetPosition;
    private bool hasTarget = false;

    public void MoveTo(Vector3 targetPosition)
    {
        currentTargetPosition = targetPosition;
        hasTarget = true;
    }

    public void Stop()
    {
        hasTarget = false;
    }

    void Update()
    {
        if (!hasTarget) return;

        // 1. Arah keinginan dasar (menuju Waypoint/Player)
        Vector3 desiredDirection = (currentTargetPosition - transform.position).normalized;

        // 2. Hitung Hindaran (Smart Whiskers)
        Vector3 avoidanceDirection = ComputeSmartWhiskers();

        // 3. Logika Prioritas:
        // Jika ada bahaya (avoidance tidak nol), arahkan gerakan didominasi oleh hindaran
        // Jika aman, arahkan gerakan penuh ke target
        Vector3 finalDirection;

        if (avoidanceDirection != Vector3.zero)
        {
            // Campur arah target (lemah) dengan arah hindaran (kuat)
            finalDirection = (desiredDirection * 0.2f) + (avoidanceDirection * 1.5f);
        }
        else
        {
            // Aman, jalan lurus
            finalDirection = desiredDirection;
        }

        finalDirection.Normalize();

        // 4. Eksekusi
        ApplyMovement(finalDirection);
    }

    Vector3 ComputeSmartWhiskers()
    {
        Vector3 avoidVec = Vector3.zero;
        RaycastHit hit;

        // --- KUMIS 1: TENGAH (Prioritas Tertinggi) ---
        // Deteksi tembok tepat di depan
        bool hitCenter = Physics.Raycast(transform.position, transform.forward, out hit, lookAhead, obstacleMask);

        // Visual Debug (Hijau = Aman, Merah = Bahaya)
        Debug.DrawRay(transform.position, transform.forward * lookAhead, hitCenter ? Color.red : Color.green);

        if (hitCenter)
        {
            // Pantulkan arah menjauh dari permukaan tembok (Normal)
            // Kita pakai Vector3.Reflect agar pantulannya natural
            avoidVec += hit.normal * avoidForce;
        }

        // --- SETUP KUMIS SAMPING ---
        // Hitung arah kumis kiri dan kanan berdasarkan rotasi musuh saat ini
        Vector3 rightRayDir = Quaternion.Euler(0, sideRayAngle, 0) * transform.forward;
        Vector3 leftRayDir = Quaternion.Euler(0, -sideRayAngle, 0) * transform.forward;

        // --- KUMIS 2: KANAN ---
        bool hitRight = Physics.Raycast(transform.position, rightRayDir, out hit, sideLookAhead, obstacleMask);
        Debug.DrawRay(transform.position, rightRayDir * sideLookAhead, hitRight ? Color.red : Color.yellow);

        if (hitRight)
        {
            // Jika kanan kena, dorong ke KIRI (negatif transform.right)
            avoidVec -= transform.right * (avoidForce / 2);
        }

        // --- KUMIS 3: KIRI ---
        bool hitLeft = Physics.Raycast(transform.position, leftRayDir, out hit, sideLookAhead, obstacleMask);
        Debug.DrawRay(transform.position, leftRayDir * sideLookAhead, hitLeft ? Color.red : Color.yellow);

        if (hitLeft)
        {
            // Jika kiri kena, dorong ke KANAN (positif transform.right)
            avoidVec += transform.right * (avoidForce / 2);
        }

        return avoidVec;
    }

    void ApplyMovement(Vector3 direction)
    {
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Putar badan
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        // Gerak maju
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}