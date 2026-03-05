using UnityEngine;

[RequireComponent(typeof(Musuh))]
public class MusuhConnector : MonoBehaviour
{
    private Musuh musuh;

    void Awake()
    {
        musuh = GetComponent<Musuh>();

        // Pastikan GameManager ada
        if (GameManager.instance == null)
        {
            GameObject gmObj = GameObject.Find("GameManager");
            if (gmObj != null)
            {
                GameManager.instance = gmObj.GetComponent<GameManager>();
            }
        }

        // Debug jika belum ditemukan
        if (GameManager.instance == null)
        {
            Debug.LogWarning($"⚠️ GameManager belum ditemukan oleh {gameObject.name}. Pastikan objek GameManager aktif di scene!");
        }
        else
        {
            Debug.Log($"✅ {gameObject.name} berhasil terhubung ke GameManager!");
        }
    }
}
