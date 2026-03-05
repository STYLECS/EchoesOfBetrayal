using UnityEngine;

public class SignInteract : MonoBehaviour
{
    public GameObject popupUI;   // drag panel popup di sini
    private bool isPlayerNear = false;

    void Update()
    {
        // Player harus dekat dan menekan F
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            // Toggle popup
            bool isActive = popupUI.activeSelf;
            popupUI.SetActive(!isActive);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("Dekati sign, tekan F untuk melihat info");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            popupUI.SetActive(false); // otomatis hilang saat menjauh
        }
    }
}
