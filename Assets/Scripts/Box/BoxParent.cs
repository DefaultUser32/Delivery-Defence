/* 1. description: handles logic for the health/damage of enemies
 * 2. @author: Linden/Matthew
 * 3. @date: 22-12-24
 * 4. @version: 1.0
 */

using UnityEngine;

public class BoxParent : MonoBehaviour
{
    public GameObject boxLevelDown;
    public float health;
    public int damageValue; // amount of damage that the box does to the player if it gets through
    protected int moneyDeath; // amount of money earned on death

    // desc: start is run before the first frame when the object is created (UNITY FUNCTION)
    // pre: none
    // post: none
    void Start()
    {
        moneyDeath = (int)health * 3;
        damageValue = Mathf.CeilToInt(health);
    }

    // desc: callback function for when 2 triggers enter each other in Unity, with a reference to the collider of the other object
    // pre: Collider2D other object
    // post: none
    protected virtual void OnTriggerEnter2D(Collider2D collide)
    {
        // if its a projectile, deal damage to self
        if (collide.TryGetComponent<Projectile>(out Projectile p))
        {
            health -= p.damage;
        }

        damageValue = Mathf.CeilToInt(health);

        // if the box dies, 
        if (health <= 0)
        {
            GameManager.money += moneyDeath;
            BoxDowngrade();
            Destroy(gameObject);
            return;
        }
    }

    // desc: when a box is killed, spawns the next box down in the chain
    // pre: must have boxLevelDown not null, otherwise does nothing
    // post: creates an instance of the next box
    protected void BoxDowngrade()
    {
        if (boxLevelDown == null) return;

        GameObject downBox = Instantiate(boxLevelDown, transform.position, transform.rotation);

        // set new bos position
        downBox.transform.parent = transform.parent;
        downBox.transform.position = transform.position;

        // get path component of this and new instance
        var pathFollow = GetComponent<PathFollow>();
        var newPathFollow = downBox.GetComponent<PathFollow>();

        // edge case for invalid path initialization
        if (newPathFollow == null || newPathFollow == null)
        {
            Debug.Log("downgrade failed due to invalid path");
        }

        // copy values
        newPathFollow.path = pathFollow.path;
        newPathFollow.targetNode = pathFollow.targetNode;
        newPathFollow.ofset = pathFollow.ofset;
        newPathFollow.speed = pathFollow.speed;
    }

    // desc: callback function for when the box reaches the end
    // pre: none
    // post: deals damage to player, and deletes itself
    public void HandleWhenEndAtTrackToDealDamageToTheStaticGameManagerWhichWeDidntDoBeforeAllInThisVeryLongNamedFunctionItIsSchockinglyLong()
    {
        GameManager.health -= damageValue;
        Destroy(gameObject);
    }
}
