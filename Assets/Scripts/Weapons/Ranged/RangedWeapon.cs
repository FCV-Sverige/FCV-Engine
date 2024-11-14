using System;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform firepoint;

    private SpriteRenderer spriteRenderer;
    
    
    protected void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    /// <summary>
    /// Spawns projectile on firepoint transform and subscribes to hitAction on projectile to control damage
    /// </summary>
    protected override void Fire()
    {
        base.Fire();

        Vector2 spawnPosition = firepoint.position;
        Quaternion spawnRotation = GetRotationToMouse();
        Projectile spawnedProjectile = Instantiate(projectile, spawnPosition, spawnRotation);
        
        spawnedProjectile.hitAction.AddListener(ProjectileHit);
    }
    /// <summary>
    /// Controls the visual and aim function for the ranged weapon
    /// </summary>
    protected override void Update()
    {
        if (!Equipped) return;
        
        base.Update();
        
        transform.rotation = GetRotationToMouse();
        
        FlipGunSprite();
    }

    private void FlipGunSprite()
    {
        spriteRenderer.flipY = !(Vector2.Dot(transform.up, Vector2.up) > 0);
    }

    private Quaternion GetRotationToMouse()
    {
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = targetPosition - (Vector2)transform.position;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(new Vector3(0,0, angle));
    }
    
    /// <summary>
    /// If projectile hits this function removes health from the Health component provided
    /// </summary>
    /// <param name="health"></param>
    private void ProjectileHit(Health health)
    {
        health.RemoveHealth(damage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if (firepoint)
            Gizmos.DrawWireSphere(firepoint.position, .08f);
    }
}
