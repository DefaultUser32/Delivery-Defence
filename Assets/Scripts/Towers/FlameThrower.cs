/* 1. description: script for override logic for the flamethrower tower
 * 2. @author: Linden/Matthew
 * 3. @date: 05-01-25
 * 4. @version: 1.0
 */
using System;
using UnityEngine;

public class FlameThrower : Tower
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] SpriteRenderer headRenderer;

    ParticleSystem.EmissionModule em;
    ParticleSystem.MainModule main;
    public float fireSizeMultiplier;

    // desc: base unity function
    // pre: none
    // post: none
    public void Start()
    {
        em = ps.emission;
        main = ps.main;
        base.Start();
    }

    // desc: this is re-writen in this subclass such that it is always facing the enemies, rather than just when shooting
    // pre: none
    // post: none
    protected override void HandleAttack()
    {
        main.startLifetime = range * fireSizeMultiplier;

        Transform target = GetTarget();
        if (target == null)
        {
            em.enabled = false;
            return;
        }

        em.enabled = true;

        Vector2 direction = (target.position - centerPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion facingDir = Quaternion.Euler(new Vector3(0, 0, angle));

        rotationPoint.rotation = Quaternion.Euler(0, 0, 90 + facingDir.eulerAngles.z);

        float timeSinceShot = (float)(DateTime.Now - lastShot).TotalSeconds;

        if (timeSinceShot < fireRate)
        {
            return;
        }


        lastShot = DateTime.Now;


        GameObject pewPew = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        // resizes the projectile to match range indicator
        // this is the best aproximation i could come up with (its not based on anything but vibes)
        float fireScale = 2 * (range - 2) + 1;

        pewPew.transform.localScale = new Vector3(fireScale, fireScale, fireScale);

        Rigidbody2D rb = pewPew.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        pewPew.transform.rotation = facingDir;

        Projectile projectile = pewPew.GetComponent<Projectile>();
        projectile.pierce = pierce;
        projectile.damage = damage;
    }

    // desc: override for upgrade application
    // pre: valid upgrade data
    // post: none
    public void ApplyUpgrade(Upgrade upgrade)
    {
        base.ApplyUpgrade(upgrade);
        damage -= upgrade.damageAdd;
        damage *= upgrade.damageAdd;
        main.startLifetime = range * fireSizeMultiplier;
    }

    // desc: unity function that is called when mouse is hovering tower
    // pre: none
    // post: none
    private void OnMouseOver()
    {
        sr.color = selectedColor;
        headRenderer.color = selectedColor;
        rangeIndicator.gameObject.SetActive(true);

        if (Input.GetMouseButtonDown(0))
        {
            uiStateManager.OpenUpgrade(this);
        }
    }

    // desc: unity function that is called when mouse is exiting tower
    // pre: none
    // post: none
    private void OnMouseExit()
    {
        rangeIndicator.gameObject.SetActive(false);
        sr.color = baseColour;
        headRenderer.color = baseColour;
    }
}
