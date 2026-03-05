using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShoot : MonoBehaviour
{
    public GameObject peluruSinglePrefab;
    public GameObject peluruShotgunPrefab;
    public Transform firePoint;

    private Movement movement;
    private Animator anim;

    public int maxSingleAmmo = 15;
    public int maxShotgunAmmo = 8;

    private int currentSingleAmmo;
    private int currentShotgunAmmo;

    public float reloadSingleTime = 1.5f;
    public float reloadShotgunTime = 2.5f;

    private bool isReloadingSingle;
    private bool isReloadingShotgun;

    public AudioClip shootSingleSound;
    public AudioClip shootShotgunSound;
    private AudioSource audioSource;

    public int shotgunPelletCount = 5;
    public float minSpreadAngle = 2f;
    public float maxSpreadAngle = 25f;
    public float maxSpreadDistance = 10f;

    public float autoAimRadius = 7f;
    public LayerMask enemyLayer;

    private bool canShoot = true;

    void Start()
    {
        movement = GetComponent<Movement>();
        anim = GetComponent<Animator>();

        currentSingleAmmo = maxSingleAmmo;
        currentShotgunAmmo = maxShotgunAmmo;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (PlayerHide.isHiding) return;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            ShootSingle();

        if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
            ShootShotgun();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentSingleAmmo <= 0 && !isReloadingSingle)
                StartCoroutine(ReloadSingle());

            if (currentShotgunAmmo <= 0 && !isReloadingShotgun)
                StartCoroutine(ReloadShotgun());
        }
    }

    // ======================
    // METHOD UNTUK UI BUTTON
    // ======================

    public void UIButton_ShootSingle()
    {
        if (PlayerHide.isHiding) return;
        ShootSingle();
    }

    public void UIButton_ShootShotgun()
    {
        if (PlayerHide.isHiding) return;
        ShootShotgun();
    }

    // ======================

    void ShootSingle()
    {
        if (!canShoot || isReloadingSingle) return;

        if (currentSingleAmmo <= 0)
        {
            StartCoroutine(ReloadSingle());
            return;
        }

        StartCoroutine(ShootCooldown());

        anim.SetBool("isShooting", true);

        Transform target = GetClosestEnemy();
        Vector2 direction = GetDirection(target);

        SpawnBullet(peluruSinglePrefab, direction);
        currentSingleAmmo--;

        if (shootSingleSound)
            audioSource.PlayOneShot(shootSingleSound);

        if (currentSingleAmmo <= 0)
            StartCoroutine(ReloadSingle());

        StartCoroutine(ResetShootAnim());
    }

    void ShootShotgun()
    {
        if (!canShoot || isReloadingShotgun) return;

        if (currentShotgunAmmo <= 0)
        {
            StartCoroutine(ReloadShotgun());
            return;
        }

        StartCoroutine(ShootCooldown());

        anim.SetBool("isShooting", true);

        Transform target = GetClosestEnemy();
        Vector2 baseDirection = GetDirection(target);

        float distance = target
            ? Vector2.Distance(transform.position, target.position)
            : maxSpreadDistance;

        float t = Mathf.Clamp01(distance / maxSpreadDistance);
        float spread = Mathf.Lerp(minSpreadAngle, maxSpreadAngle, t);

        for (int i = 0; i < shotgunPelletCount; i++)
        {
            float angle = Random.Range(-spread, spread);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDirection;
            SpawnBullet(peluruShotgunPrefab, dir);
        }

        currentShotgunAmmo--;

        if (shootShotgunSound)
            audioSource.PlayOneShot(shootShotgunSound);

        if (currentShotgunAmmo <= 0)
            StartCoroutine(ReloadShotgun());

        StartCoroutine(ResetShootAnim());
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.15f);
        canShoot = true;
    }

    void SpawnBullet(GameObject prefab, Vector2 direction)
    {
        GameObject p = Instantiate(prefab, firePoint.position, Quaternion.identity);
        Peluru peluru = p.GetComponent<Peluru>();
        if (peluru != null)
            peluru.SetDirection(direction);
    }

    Vector2 GetDirection(Transform target)
    {
        if (target != null)
        {
            Vector2 dir = (target.position - firePoint.position).normalized;

            bool left = target.position.x < transform.position.x;
            if (left && movement.IsFacingRight) movement.Flip();
            if (!left && !movement.IsFacingRight) movement.Flip();

            return dir;
        }

        return movement.IsFacingRight ? Vector2.right : Vector2.left;
    }

    IEnumerator ReloadSingle()
    {
        isReloadingSingle = true;
        yield return new WaitForSeconds(reloadSingleTime);
        currentSingleAmmo = maxSingleAmmo;
        isReloadingSingle = false;
    }

    IEnumerator ReloadShotgun()
    {
        isReloadingShotgun = true;
        yield return new WaitForSeconds(reloadShotgunTime);
        currentShotgunAmmo = maxShotgunAmmo;
        isReloadingShotgun = false;
    }

    IEnumerator ResetShootAnim()
    {
        yield return new WaitForSeconds(0.15f);
        anim.SetBool("isShooting", false);
    }

    Transform GetClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, autoAimRadius, enemyLayer);
        if (hits.Length == 0) return null;

        Transform closest = null;
        float min = Mathf.Infinity;

        foreach (Collider2D h in hits)
        {
            float d = Vector2.Distance(transform.position, h.transform.position);
            if (d < min)
            {
                min = d;
                closest = h.transform;
            }
        }

        return closest;
    }

    bool IsPointerOverUI()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        return EventSystem.current.IsPointerOverGameObject();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }

    // ======================
    // PROPERTY UNTUK UI
    // ======================

    public int CurrentSingleAmmo => currentSingleAmmo;
    public int CurrentShotgunAmmo => currentShotgunAmmo;

    public int MaxSingleAmmo => maxSingleAmmo;
    public int MaxShotgunAmmo => maxShotgunAmmo;

    public bool IsReloadingSingle => isReloadingSingle;
    public bool IsReloadingShotgun => isReloadingShotgun;
}
