/* 1. description: Logic/data for a projectile
 * 2. @author: Linden/Matthew
 * 3. @date: 06-01-25
 * 4. @version: 1.0
 */
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public int pierce;
    public float lifeSpan;

    DateTime spawnTime;

    // desc: base unity function called on first frame
    // pre: none
    // post: none
    private void Start()
    {
        spawnTime = DateTime.Now;
    }

    // desc: unity function called each frame
    // pre: none
    // post: none
    private void Update()
    {
        // deletes projectile if lifespan exceded
        if ((DateTime.Now - spawnTime).TotalSeconds >= lifeSpan)
        {
            Destroy(gameObject);
        }
    }

    // desc: logic for when collisions happen
    // pre: reference to collided object
    // post: none
    void OnTriggerEnter2D(Collider2D collision)
    {
        pierce--;
        if (pierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
