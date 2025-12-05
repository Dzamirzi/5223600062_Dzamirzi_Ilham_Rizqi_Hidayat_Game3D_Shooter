using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referensi Player")]
    public PlayerHealth playerHealth;
    public Weapon playerWeapon;

    [Header("Konfigurasi Musuh")]

    public GameObject normalEnemyPrefab; 
    public GameObject hardEnemyPrefab;   

    [Header("Probabilitas Spawn")]
    [Range(0, 100)]
    public float hardEnemyChance = 30f; 

    [Header("Spawner Settings")]
    public Transform[] spawnPoints;
    public int maxEnemiesOnMap = 5;
    public static int currentEnemyCount = 0;
    public float spawnInterval = 5f;

    [Header("Jalur Patroli (Waypoint)")]
    public List<Transform> globalWaypoints;

    [Header("UI")]
    public GameObject upgradeUIPanel;
    public int killsToUpgrade = 10;
    private int killsSinceLastUpgrade = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (upgradeUIPanel) upgradeUIPanel.SetActive(false);
        Time.timeScale = 1f;

        currentEnemyCount = 0;

        InvokeRepeating("TrySpawnEnemy", 0.5f, spawnInterval);
    }

    void TrySpawnEnemy()
    {
        if (currentEnemyCount < maxEnemiesOnMap)
        {
            GameObject prefabToSpawn;
            float randomRoll = Random.Range(0f, 100f);

            if (randomRoll < hardEnemyChance)
            {
                prefabToSpawn = hardEnemyPrefab;
            }
            else
            {
                prefabToSpawn = normalEnemyPrefab;
            }

            int spawnIndex = Random.Range(0, spawnPoints.Length);

            if (prefabToSpawn != null && spawnPoints.Length > 0)
            {
                GameObject newEnemy = Instantiate(prefabToSpawn, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);

                EnemyAI_FSM enemyScript = newEnemy.GetComponent<EnemyAI_FSM>();
                if (enemyScript != null)
                {
                    enemyScript.SetWaypoints(globalWaypoints);
                }

                currentEnemyCount++;
            }
        }
    }

    public void EnemyHasDied()
    {
        currentEnemyCount--;
        killsSinceLastUpgrade++;
        if (killsSinceLastUpgrade >= killsToUpgrade)
        {
            killsSinceLastUpgrade = 0;
            StartUpgradeProcess();
        }
    }

    void StartUpgradeProcess() { upgradeUIPanel.SetActive(true); Time.timeScale = 0f; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    void EndUpgradeProcess() { upgradeUIPanel.SetActive(false); Time.timeScale = 1f; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    public void UpgradePlayerHP() { playerHealth.maxHealth += 20; playerHealth.TakeDamage(-20); EndUpgradeProcess(); }
    public void UpgradePlayerAttack() { playerWeapon.damage += 5; EndUpgradeProcess(); }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
}