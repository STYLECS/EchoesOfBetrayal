using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.TambahSkor(1);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameManager.instance.TambahSkor(1);
            Destroy(gameObject);

            Destroy(collision.gameObject);
        }
    }
}
