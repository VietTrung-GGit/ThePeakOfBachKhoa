using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    [Tooltip("Cooldown every time this character shoots projectiles.")]
    public float cooldownTimer;
    [Range(0, 10)]
    [Tooltip("The size of the projectile pool")]
    public int projectilePoolSize;

    [SerializeField] private Projectile projectilePrefab;
    
    private readonly List<Projectile> projectilePool = new List<Projectile>();
    private bool isReadyToShoot = true;
    private float currentCooldownTimer;
    private void Awake()
    {
        currentCooldownTimer = cooldownTimer;

        for (int i = 0; i < projectilePoolSize; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab, transform, false);
            projectile.gameObject.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    private void Update()
    {
        if (!isReadyToShoot)
        {
            currentCooldownTimer -= Time.deltaTime;
            if (currentCooldownTimer <= 0.0f)
            {
                currentCooldownTimer = cooldownTimer;
                isReadyToShoot = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Object entered trigger area!");
        if (collision.CompareTag("Player"))
        {
            if (isReadyToShoot)
            {
                ShootProjectile(collision.gameObject);
                isReadyToShoot = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isReadyToShoot)
            {
                ShootProjectile(collision.gameObject);
            }
        }
    }

    private void ShootProjectile(GameObject target)
    {
        foreach (Projectile projectile in projectilePool)
        {
            if (!projectile.gameObject.activeSelf)
            {
                projectile.player = target;
                projectile.gameObject.SetActive(true);
                break;
            }
        }
    }

}