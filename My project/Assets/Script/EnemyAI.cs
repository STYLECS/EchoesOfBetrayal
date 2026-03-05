using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Gerakan")]
    public float speed = 2f;
    public float aggroRange = 5f;

    private Transform player;
    private Vector3 spawnPoint;
    private bool isAggro = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPoint = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= aggroRange && !PlayerHide.isHiding)
        {
            isAggro = true;
        }
        else
        {
            isAggro = false;
        }

        if (isAggro)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                spawnPoint,
                speed * Time.deltaTime
            );
        }
    }
}
