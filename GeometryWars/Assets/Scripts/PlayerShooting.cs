using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public enum WeaponType { Basic, DoubleShot, SpreadShot, TailGun }

public class PlayerShooting : MonoBehaviour
{
    public WeaponType currentWeapon = WeaponType.Basic;
    public GameObject bulletPrefab;
    public float fireRate = 0.15f;

    [Header("Visual Effects")]
    public ParticleSystem muzzleParticles;
    public Light2D muzzleFlashLight;
    public float flashDuration = 0.05f;
    
    private float nextFireTime = 0f;
    private float flashTimer = 0f;
    private float defaultFlashIntensity;

    private void Start()
    {
        if (muzzleFlashLight != null)
        {
            defaultFlashIntensity = muzzleFlashLight.intensity;
            muzzleFlashLight.intensity = 0f;
        }
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        if (flashTimer > 0 && muzzleFlashLight != null)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                muzzleFlashLight.intensity = 0f;
            }
            else
            {
                // Fade out
                muzzleFlashLight.intensity = Mathf.Lerp(0f, defaultFlashIntensity, flashTimer / flashDuration);
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        switch (currentWeapon)
        {
            case WeaponType.Basic:
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                break;
            case WeaponType.DoubleShot:
                Instantiate(bulletPrefab, transform.position + transform.up * 0.3f, transform.rotation);
                Instantiate(bulletPrefab, transform.position - transform.up * 0.3f, transform.rotation);
                break;
            case WeaponType.SpreadShot:
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                Instantiate(bulletPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 15f));
                Instantiate(bulletPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, -15f));
                break;
            case WeaponType.TailGun:
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                Instantiate(bulletPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 180f));
                break;
        }

        // Muzzle Flash / Particles will be played below

        // Particles
        if (muzzleParticles != null)
        {
            muzzleParticles.Play();
        }

        // Muzzle Flash
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.intensity = defaultFlashIntensity;
            flashTimer = flashDuration;
        }
    }
}
