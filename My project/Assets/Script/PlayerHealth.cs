using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI Health")]
    [SerializeField] private Slider healthSlider;

    [Header("Damage System")]
    [SerializeField] private float damageInterval = 1f;
    private float lastDamageTime;

    [Header("Visual Feedback")]
    [SerializeField] private float damageFlashDuration = 0.2f;
    [SerializeField] private Color damageFlashColor = Color.red;
    private SpriteRenderer spriteRenderer;

    private List<Musuh> musuhYangNempel = new List<Musuh>();

    private Animator anim;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // 🔥 RESET SAAT RESPAWN (INI KUNCI)
        isDead = false;
        currentHealth = maxHealth;
        lastDamageTime = 0f;
        musuhYangNempel.Clear();

        if (anim)
            anim.SetBool("isDead", false);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Health Slider tidak diatur di Inspector!");
        }
    }

    void Update()
    {
        if (isDead || musuhYangNempel.Count == 0) return;

        if (Time.time >= lastDamageTime + damageInterval)
        {
            int totalDamage = 0;

            musuhYangNempel.RemoveAll(musuh => musuh == null);

            foreach (Musuh musuh in musuhYangNempel)
            {
                totalDamage += musuh.DamageToPlayer;
            }

            if (totalDamage > 0)
            {
                TakeDamage(totalDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = original;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (anim)
            anim.SetBool("isDead", true);

        // 🔥 AMAN: GameManager BOLEH BELUM ADA
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager tidak ditemukan! Player mati tanpa GameOver.");
        }

        StartCoroutine(DisableAfterAnimation());
    }

    private IEnumerator DisableAfterAnimation()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        Musuh musuh = collision.GetComponent<Musuh>();

        if (musuh == null)
            musuh = collision.GetComponentInParent<Musuh>();

        if (musuh != null)
        {
            if (!musuhYangNempel.Contains(musuh))
            {
                musuhYangNempel.Add(musuh);
                Debug.Log("Musuh memasuki trigger: " + musuh.name);
            }
        }

        if (collision.CompareTag("Water"))
            TakeDamage(maxHealth);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Musuh musuh = collision.GetComponent<Musuh>();

        if (musuh == null)
            musuh = collision.GetComponentInParent<Musuh>();

        if (musuh != null && musuhYangNempel.Contains(musuh))
        {
            musuhYangNempel.Remove(musuh);
            Debug.Log("Musuh keluar dari trigger: " + musuh.name);
        }
    }

    // ======================================================
    //               FUNGSI HEAL
    // ======================================================
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
}
