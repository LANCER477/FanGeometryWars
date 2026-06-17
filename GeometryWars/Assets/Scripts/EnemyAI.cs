using UnityEngine;

public enum EnemyType { Wanderer, Tank, Shooter }

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class EnemyAI : MonoBehaviour
{
    public EnemyType type = EnemyType.Wanderer;
    public float moveSpeed = 3f;
    public int maxHealth = 1;
    public int scoreValue = 100;

    [Header("Effects")]
    public GameObject deathParticlesPrefab;

    [Header("Shooter Settings")]
    public GameObject bulletPrefab;
    public float fireRate = 2f;
    public float stopDistance = 8f;

    private int currentHealth;
    private Rigidbody2D rb;
    private Transform player;
    private float nextFireTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = GetComponent<PolygonCollider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        nextFireTime = Time.time + Random.Range(1f, 2f);
    }

    private void FixedUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (type == EnemyType.Shooter && distance < stopDistance)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving
        }
        else
        {
            rb.linearVelocity = direction * moveSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Shooting logic
        if (type == EnemyType.Shooter && Time.time >= nextFireTime && bulletPrefab != null)
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        SpawnDeathParticles();

        Destroy(gameObject);
    }

    private void SpawnDeathParticles()
    {
        GameObject particlesObj = new GameObject("DeathParticles");
        particlesObj.transform.position = transform.position;

        ParticleSystem ps = particlesObj.AddComponent<ParticleSystem>();
        
        // Fix for "Setting the duration while system is still playing is not supported"
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 15f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            main.startColor = sr.color;
        }

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

        ParticleSystemRenderer renderer = particlesObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 22;

        ps.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
            Die(); // Kill enemy too when it crashes into player
        }
    }
}
