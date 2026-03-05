using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Heal Amount")]
    public int healValue = 1;

    [Header("Optional Effect")]
    public AudioSource pickupSound;
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();

        // Kalau collider bukan player, jangan lanjut
        if (player == null) return;

        // Heal player
        player.Heal(healValue);

        // Efek suara
        if (pickupSound != null)
            pickupSound.Play();

        // Efek visual
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // Hilangkan item
        Destroy(gameObject);
    }
}
