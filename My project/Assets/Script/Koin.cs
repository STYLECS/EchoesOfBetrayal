using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void Start()
    {
        // Daftarkan coin ke sistem level complete
        if (GameManagerLevelComplete.instance != null)
            GameManagerLevelComplete.instance.RegisterCoin();
        else
            Debug.LogWarning("⚠️ GameManagerLevelComplete belum ditemukan (Coin)");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Tambah skor (core)
        GameManager.instance?.TambahSkor(coinValue);

        // Laporkan coin diambil
        GameManagerLevelComplete.instance?.CoinCollected();

        Destroy(gameObject);
    }
}
