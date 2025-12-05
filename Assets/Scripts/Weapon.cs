using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Lean.Pool;
using TMPro;

public class Weapon : MonoBehaviour
{
    public bool isFullAuto;
    public float shootDelay;
    public float damage;
    public float range;
    public GameObject bulletShell;
    public GameObject muzzleFlashFX;
    public GameObject impactFX;
    public GameObject audioPrefab;
    public Transform cameraTransform;
    public Transform muzzleFlashSpawnPoint;
    public Transform bulletSpawnPoint;
    public AudioClip shotClip;
    public AudioClip impactSound;

    bool isShoot;

    [Header("Amunisi & UI")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Referensi UI")]
    public TextMeshProUGUI ammoText;

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    private void Update()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * range, Color.red);
    }

    public void Shoot()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (isFullAuto)
        {
            if (!isShoot)
            {
                StartCoroutine(IEShootDelay());
                IEnumerator IEShootDelay()
                {
                    ShootRay();
                    currentAmmo--;
                    UpdateAmmoText();
                    isShoot = true;
                    yield return new WaitForSeconds(shootDelay);
                    isShoot = false;
                }
            }
        }
        else
        {
            if (!isShoot)
            {
                ShootRay();
                currentAmmo--;
                UpdateAmmoText();
                isShoot = true;
            }
        }
    }

    void ShootRay()
    {
        RaycastHit hit;

        LeanPool.Spawn(muzzleFlashFX, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
        LeanPool.Spawn(bulletShell, bulletSpawnPoint.position, RandomQuaternion());
        GameObject audioClone = LeanPool.Spawn(audioPrefab, muzzleFlashSpawnPoint.position,
            quaternion.identity);
        AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
        audioSourceClone.clip = shotClip;
        audioSourceClone.Play();

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            LeanPool.Spawn(impactFX, hit.point, RandomQuaternion());
            audioClone = LeanPool.Spawn(audioPrefab, hit.point, quaternion.identity);
            audioSourceClone = audioClone.GetComponent<AudioSource>();
            audioSourceClone.clip = impactSound;
            audioSourceClone.Play();

            if (hit.transform.tag == "Enemy")
            {
                HealthManager hitHealthManager = hit.transform.GetComponent<HealthManager>();
                hitHealthManager.TakeDamage(damage);
            }
        }
    }

    Quaternion RandomQuaternion()
    {
        return new Quaternion(Random.Range(-360, 360), Random.Range(-360, 360),
                Random.Range(-360, 360), Random.Range(-360, 360));
    }

    public void StopShoot()
    {
        isShoot = false;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        UpdateAmmoText();

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            if (isReloading)
            {
                ammoText.text = "RELOADING...";
            }
            else
            {
                ammoText.text = currentAmmo + " / " + maxAmmo;
            }
        }
    }
}