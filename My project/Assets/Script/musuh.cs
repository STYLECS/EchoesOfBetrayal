using System.Collections.Generic;
using UnityEngine;

public class Musuh : MonoBehaviour
{
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitParticlePrefab;

    [Header("Gerakan")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform target;

    [Header("Darah Musuh")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Serangan")]
    [SerializeField] private int damageToPlayer = 1;
    public int DamageToPlayer => damageToPlayer;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float stopDistance = 1f;
    private bool isAggro = false;

    [Header("Aggro Sistem")]
    [SerializeField] private float aggroRadius = 5f;

    [Header("Reward Skor")]
    [SerializeField] private int skorReward = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private Vector3 spawnPoint;

    // ============================
    // DAMAGE SYSTEM
    // ============================
    [Header("Player Damage System")]
    [SerializeField] private float damageInterval = 1f;
    private float lastDamageTime = 0f;

    private List<PlayerHealth> playersInRange = new List<PlayerHealth>();
    // ============================

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        spawnPoint = transform.position;

        // Daftarkan musuh ke level complete
        if (GameManagerLevelComplete.instance != null)
            GameManagerLevelComplete.instance.RegisterEnemy();
        else
            Debug.LogWarning("⚠️ GameManagerLevelComplete belum ditemukan (Musuh)");

        // Cari player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else
        {
            Debug.LogWarning($"Player tidak ditemukan oleh {name}");
            enabled = false;
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        float jarak = Vector2.Distance(transform.position, target.position);
        bool melihatPlayer = !PlayerHide.isHiding;

        UpdateAnimation(jarak, melihatPlayer);

        if (melihatPlayer && (jarak <= detectionRange || isAggro) && jarak > stopDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
        else if (PlayerHide.isHiding)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                spawnPoint,
                speed * Time.deltaTime
            );
            isAggro = false;
        }

        if (spriteRenderer != null)
            spriteRenderer.flipX = target.position.x < transform.position.x;

        HandleDamageToPlayer();
    }

    // ============================
    // ANIMASI
    // ============================
    void UpdateAnimation(float jarak, bool melihatPlayer)
    {
        if (anim == null || isDead) return;

        if (melihatPlayer && jarak <= detectionRange)
        {
            anim.SetFloat("IdleToSout", 1f);
            anim.SetFloat("IdleToSin", 0f);
        }
        else
        {
            anim.SetFloat("IdleToSout", 0f);
            anim.SetFloat("IdleToSin", 1f);
        }
    }

    // ============================
    // DAMAGE DARI PLAYER
    // ============================
    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (hitParticlePrefab != null)
        {
            Vector3 offset = new Vector3(-Mathf.Sign(attackDirection.x), 0f, 0f) * 0.5f;

            GameObject hitEffect = Instantiate(
                hitParticlePrefab,
                transform.position + offset,
                Quaternion.identity
            );

            Destroy(hitEffect, 2f);
        }

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        isAggro = true;
        CallNearbyEnemies();

        if (currentHealth <= 0)
            Die();
    }


    void CallNearbyEnemies()
    {
        Collider2D[] sekitar = Physics2D.OverlapCircleAll(transform.position, aggroRadius);

        foreach (Collider2D col in sekitar)
        {
            Musuh m = col.GetComponent<Musuh>();
            if (m != null && !m.isDead)
                m.BecomeAggro();
        }
    }

    public void BecomeAggro()
    {
        if (!isDead)
            isAggro = true;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Tambah skor & lapor ke level complete
        GameManager.instance?.TambahSkor(skorReward);
        GameManagerLevelComplete.instance?.EnemyDefeated();

        Destroy(gameObject);
    }

    // ============================
    // DAMAGE KE PLAYER
    // ============================
    private void HandleDamageToPlayer()
    {
        if (playersInRange.Count == 0 || isDead) return;

        if (Time.time >= lastDamageTime + damageInterval)
        {
            foreach (PlayerHealth p in playersInRange)
            {
                if (p != null && !p.IsDead)
                    p.TakeDamage(damageToPlayer);
            }

            lastDamageTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth p = collision.gameObject.GetComponent<PlayerHealth>();
        if (p != null && !playersInRange.Contains(p))
            playersInRange.Add(p);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerHealth p = collision.gameObject.GetComponent<PlayerHealth>();
        if (p != null && playersInRange.Contains(p))
            playersInRange.Remove(p);
    }

    // ============================
    // GIZMOS
    // ============================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
