using UnityEngine;

public class Peluru : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Musuh"))
        {
            Musuh musuh = other.GetComponent<Musuh>();
            if (musuh != null)
            {
                // 🔥 KIRIM ARAH SERANGAN
                musuh.TakeDamage(damage, direction);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
