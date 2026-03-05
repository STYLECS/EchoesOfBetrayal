using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    public static bool isHiding = false;
    private bool canHide = false;
    private Collider2D currentHideSpot;
    private SpriteRenderer spriteRenderer;
    public MonoBehaviour shootingScript;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (canHide && Input.GetKeyDown(KeyCode.F))
        {
            isHiding = !isHiding;

            if (isHiding)
            {
                Debug.Log("Player bersembunyi!");
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (shootingScript != null) shootingScript.enabled = false; // matikan script tembak
            }
            else
            {
                Debug.Log("Player keluar dari persembunyian!");
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                if (shootingScript != null) shootingScript.enabled = true; // aktifkan lagi
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot"))
        {
            canHide = true;
            currentHideSpot = other;
            Debug.Log("Dekat semak, tekan F untuk bersembunyi");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot") && other == currentHideSpot)
        {
            canHide = false;
            isHiding = false;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (shootingScript != null) shootingScript.enabled = true;
            Debug.Log("Keluar dari area semak");
        }
    }
}
    