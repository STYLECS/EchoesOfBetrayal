using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public PlayerShoot playerShoot;
    public TMP_Text ammoText;
    public TMP_Text reloadText;

    public string label = "Peluru";
    public bool isShotgun;

    void Update()
    {
        if (playerShoot == null) return;

        UpdateAmmo();
        UpdateReload();
    }

    void UpdateAmmo()
    {
        if (ammoText == null) return;

        if (isShotgun)
        {
            ammoText.text = label + ": " +
                playerShoot.CurrentShotgunAmmo + "/" +
                playerShoot.MaxShotgunAmmo;
        }
        else
        {
            ammoText.text = label + ": " +
                playerShoot.CurrentSingleAmmo + "/" +
                playerShoot.MaxSingleAmmo;
        }
    }

    void UpdateReload()
    {
        if (reloadText == null) return;

        bool reloading = isShotgun
            ? playerShoot.IsReloadingShotgun
            : playerShoot.IsReloadingSingle;

        reloadText.gameObject.SetActive(reloading);

        if (reloading)
        {
            reloadText.text = "Sedang Reload...";
        }
    }
}
