using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    public Transform target;
    public float smoothSpeed = 5f;
    public float zOffset = -10f;

    private Vector3 velocity = Vector3.zero;

    private Volume globalVolume;
    private ChromaticAberration chromaticAberration;
    private float glitchTimer = 0f;
    private float glitchDuration = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameObject volumeObj = GameObject.Find("PostProcessingVolume");
        if (volumeObj != null)
        {
            globalVolume = volumeObj.GetComponent<Volume>();
            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.TryGet(out chromaticAberration);
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, zOffset);
        
        // Use SmoothDamp instead of Lerp for buttery smooth subpixel movement
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);

        // Handle Chromatic Aberration Glitch
        if (glitchTimer > 0 && chromaticAberration != null)
        {
            glitchTimer -= Time.deltaTime;
            float progress = Mathf.Clamp01(glitchTimer / glitchDuration);
            chromaticAberration.intensity.Override(progress * 1f); // Max intensity 1
        }
    }

    public void DamageGlitch()
    {
        if (chromaticAberration != null)
        {
            glitchTimer = 2f; // Increased to 2 seconds
            glitchDuration = 2f;
            chromaticAberration.intensity.Override(1f);
        }
    }
}
