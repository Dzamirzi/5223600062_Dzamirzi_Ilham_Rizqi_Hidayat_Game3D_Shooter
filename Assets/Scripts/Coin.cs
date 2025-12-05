using UnityEngine;
using Lean.Pool;

public class Coin : MonoBehaviour
{
    public AudioClip pickupSound;

    // Pastikan Collider di prefab Koin Anda dicentang "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        // Cek jika yang menyentuh adalah Player
        if (other.CompareTag("Player"))
        {
            // Putar suara di lokasi koin
            if (pickupSound != null)
            {
                // Gunakan PlayClipAtPoint agar suara tetap ada
                // bahkan setelah koin hancur
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Hancurkan koin menggunakan LeanPool (seperti skrip Anda)
            LeanPool.Despawn(this.gameObject);
        }
    }
}