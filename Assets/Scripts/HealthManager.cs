using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.UI; // Wajib untuk Slider

public class HealthManager : MonoBehaviour
{
    [Header("Pengaturan Dasar")]
    public float setHealth;
    private float currentHealth;

    [Header("Logika Drop Koin (Probabilitas)")]
    public GameObject coinPrefab;
    [Range(0f, 1f)]
    public float coinDropProbability = 0.5f;

    [Header("UI Musuh")]
    public Slider healthBarSlider; 

    float CURRENTHEALTH
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                OnCurrentHealthChanged();
            }
        }
    }

    private void Start()
    {
        currentHealth = setHealth;
        UpdateEnemyHealthBar(); 
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        CURRENTHEALTH -= damage;
    }

    void OnCurrentHealthChanged()
    {
        UpdateEnemyHealthBar();

        if (CURRENTHEALTH <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnemyHasDied();
            }

            float randomRoll = Random.Range(0f, 1f);
            if (randomRoll <= coinDropProbability)
            {
                if (coinPrefab != null)
                {
                    LeanPool.Spawn(coinPrefab, transform.position, coinPrefab.transform.rotation);
                }
            }

            Destroy(gameObject);
        }
    }

    void UpdateEnemyHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / setHealth;
        }
    }
}