/* 1. description: all logic related to a tower
 * 2. @author: Linden/Matthew
 * 3. @date: 28-12-24
 * 4. @version: 1.0
 */
using System.Collections.Generic;
using UnityEngine;
using System;
public class Tower : MonoBehaviour
{
    [SerializeField] protected Transform centerPoint;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected Transform rotationPoint;
    [SerializeField] protected Transform rangeIndicator;
    public readonly string targetTag = "Enemy";
    public bool faceTarget;

    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float fireRate;
    public float damage;
    public int pierce;
    public float range;


    public TargetingType tarType;
    public List<Upgrade> path1;
    public List<Upgrade> path2;

    public SpriteRenderer sr;
    protected UIStateManager uiStateManager;
    protected DateTime lastShot = DateTime.Now;
    protected Color baseColour;
    protected Color selectedColor;

    // desc: sub class to hold information relating to a tower upgrade
    [System.Serializable]
    public class Upgrade {
        public string upgradeName;
        public string upgradeDescription;

        public bool hasBeenApplied;
        public int price;

        public float fireRateMultiplier;
        public float projectileSpeedMultiplier;
        public float damageAdd;
        public float rangeMultiplier;

        public int pierceAdd;

        public Sprite towerSprite;
        public GameObject attackPrefab;
    }

    public enum TargetingType{
        closest,
        first,
        last
    }

    // desc: unity function called at start of object
    // pre: none
    // post: none
    public void Start()
    {
        uiStateManager = FindFirstObjectByType<UIStateManager>();
        sr = GetComponent<SpriteRenderer>();

        baseColour = sr.color;

        selectedColor = sr.color * .5f;
        selectedColor.a = baseColour.a;

        rangeIndicator.localScale = new Vector3(2 * range, 2 * range, 2 * range);
    }

    // desc: unity function called once per frame
    // pre: none
    // post: none
    public void Update()
    {
        HandleAttack();
    }

    // desc: gets the number of upgrades applied on a given path
    // pre: whether its the first path or not
    // post: path index as int, or -1 if path is completly upgraded
    public int GetPathProgress(bool doFirst)
    {
        List<Upgrade> path = doFirst  ? path1 : path2;
        for (int i = 0; i < path.Count; i++)
        {
            if (!path[i].hasBeenApplied) return i;
        }
        return -1;
    }

    // desc: applies upgrade to tower path
    // pre: whether its the first path or not
    // post: none
    public void UpgradePath(bool doFirst)
    {
        List<Upgrade> path = doFirst ? path1 : path2;
        foreach (Upgrade upgrade in path) {
            if (upgrade.hasBeenApplied) continue;
            ApplyUpgrade(upgrade);
            return;
        }

    }

    // desc: gets Upgrade data for next upgrade on path
    // pre: whether its the first path or not
    // post: returns upgrade data
    public Upgrade GetNextUpgrade(bool isFirst)
    {
        int pathProgress = GetPathProgress(isFirst);
        if (pathProgress == -1) return null;
        
        if (isFirst)
        {
            return path1[pathProgress];
        }
        return path2[pathProgress];
    }

    // desc: applies given upgrade data to tower
    // pre: valid upgrade
    // post: none
    public void ApplyUpgrade(Upgrade upgrade) 
    {
        upgrade.hasBeenApplied = true;
        fireRate /= upgrade.fireRateMultiplier;
        projectileSpeed *= upgrade.projectileSpeedMultiplier;
        range *= upgrade.rangeMultiplier;

        damage += upgrade.damageAdd;
        pierce += upgrade.pierceAdd;
        if (upgrade.towerSprite != null)
        {
            sr.sprite = upgrade.towerSprite;
        }

        if (upgrade.attackPrefab != null) 
        {
            projectilePrefab = upgrade.attackPrefab;
        }
        rangeIndicator.localScale = new Vector3 (2 * range, 2 * range, 2 * range);
    }

    // desc: sends out default attack if enough time has elapsed
    // pre: none
    // post: none
    protected virtual void HandleAttack()
    {
        float timeSinceShot = (float)(DateTime.Now - lastShot).TotalSeconds;

        if (timeSinceShot < fireRate)
        {
            return;
        }

        Transform target = GetTarget();
        if (target == null)
        {
            return;
        }         

        Vector2 direction = (target.position - centerPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion facingDir = Quaternion.Euler(new Vector3(0, 0, angle));
        if (faceTarget)
            rotationPoint.rotation = facingDir * Quaternion.Euler(0, 0, 90);
       

        lastShot = DateTime.Now;


        GameObject pewPew = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);


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

    // desc: gets position of target
    // pre: none
    // post: position of target as transform
    protected Transform GetTarget()
    {
        GameObject[] rawTargets = GameObject.FindGameObjectsWithTag(targetTag);
        List<GameObject> validTargets = new();

        foreach (GameObject t in rawTargets)
        {
            if (Vector2.Distance(transform.position, t.transform.position) < range)
            {
                validTargets.Add(t);
            }
        }

        if (validTargets.Count == 0) {
            return null;
        }

        switch (tarType){
            case TargetingType.closest:
                return ClosestTarget(validTargets);
            case TargetingType.first:
                return FirstTarget(validTargets);
            case TargetingType.last:
                return LastTarget(validTargets);
            default:
                Debug.LogError("Unknown targeting type");
                return null;
        }
    }

    // desc: gets the transform the first target in range
    // pre: list of valid targets
    // post: returns transform of target
    Transform FirstTarget(List<GameObject> validTargets)
    {
        Transform firstTarget = null;
        float firstDistance = Mathf.Infinity;

        foreach (GameObject target in validTargets)
        {

            float dist = target.GetComponent<PathFollow>().distanceRemaining;
            if (dist < firstDistance)
            {
                firstDistance = dist;
                firstTarget = target.transform;
            }
        }
        return firstTarget;
    }
    
    // desc: gets the transform the last target in range
    // pre: list of valid targets
    // post: returns transform of target
    Transform LastTarget(List<GameObject> validTargets)
    {
        Transform firstTarget = null;
        float firstDistance = Mathf.Infinity;

        foreach (GameObject target in validTargets)
        {
            float dist = target.GetComponent<PathFollow>().distanceTravelled;
            if (dist < firstDistance)
            {
                firstDistance = dist;
                firstTarget = target.transform;
            }
        }
        return firstTarget;
    }

    // desc: gets the transform the closest target in range
    // pre: list of valid targets
    // post: returns transform of target
    Transform ClosestTarget(List<GameObject> validTargets)
    {
        Transform closeTarget = null;
        float closeDist = Mathf.Infinity;
        Vector2 currentPosition = centerPoint.position;

        foreach (GameObject target in validTargets)
        {
            Debug.Log("???");
            float dist = Vector2.Distance(currentPosition, target.transform.position);
            if (dist < closeDist)
            {
                closeDist = dist;
                closeTarget = target.transform;
            }
        }
        return closeTarget;
    }

    // EDITOR ONLY CODE FOR VISUALIZATION BEFORE RELEASE
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(centerPoint.position, range);
    }
    // desc: code run when object is moused over
    // pre: none
    // post: none
    private void OnMouseOver()
    {
        rangeIndicator.gameObject.SetActive(true);
        sr.color = selectedColor;

        if (Input.GetMouseButtonDown(0)) 
        {
            uiStateManager.OpenUpgrade(this);
        }
    }

    // desc: code run when mouse exits object
    // pre: none
    // post: none
    private void OnMouseExit()
    {
        rangeIndicator.gameObject.SetActive(false);
        sr.color = baseColour;
    }
}

