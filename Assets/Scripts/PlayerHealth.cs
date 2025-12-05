using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public TextMeshProUGUI healthText;

    [Header("UI Kalah")]
    public GameObject youLosePanel; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
        youLosePanel.SetActive(false);
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Pemain Mati!");

        // Tampilkan layar "You Lose"
        youLosePanel.SetActive(true);

        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        PlayerShooting shootingScript = GetComponent<PlayerShooting>();
        if (shootingScript != null)
        {
            shootingScript.enabled = false;
        }

        MouseCursor cursorScript = GetComponent<MouseCursor>();
        if (cursorScript != null)
        {
            cursorScript.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}