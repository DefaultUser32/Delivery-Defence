/* 1. description: UI logic for following a path
 * 2. @author: Linden/Matthew
 * 3. @date: 20-12-24
 * 4. @version: 1.0
 */
using System;
using UnityEngine;

// script to allow any object to follow a path
// starts at first given node and stops at last
public class PathFollow : MonoBehaviour
{
    [SerializeField] float tolerance; // distance from node for it to count
    [SerializeField] float maxOfset; // max random distance ofset
    
    public PathParent path;
    public float speed;

    public float distanceTravelled;
    public float distanceRemaining;

    public Vector3 ofset;
    public int targetNode = 1;

    // desc: awake is a unity function that is called at initialization
    // pre: none
    // post: none
    void Awake()
    {
        ofset = new Vector3 (UnityEngine.Random.Range(0.0f, 10.0f), UnityEngine.Random.Range(0.0f, 10.0f), 0);
        ofset = ofset.normalized * maxOfset;

    }

    // desc: sets position to start of track
    // pre: none
    // post: none
    public void ResetPosition()
    {
        transform.position = path.nodePositions[0] + ofset;

        targetNode = 1;
    }

    // desc: unity function called a set number of times per second
    // pre: none
    // post: none
    private void FixedUpdate()
    {
        // update global variables for targeting
        distanceRemaining = GetDistanceToEnd();
        distanceTravelled = GetDistanceTravelled();

        // code for if at end of track
        if (targetNode >= path.nodePositions.Length)
        {
            if (TryGetComponent<BoxParent>(out BoxParent b))
            {
                b.HandleWhenEndAtTrackToDealDamageToTheStaticGameManagerWhichWeDidntDoBeforeAllInThisVeryLongNamedFunctionItIsSchockinglyLong();
                return;
            }
            Destroy(gameObject);
        }

        // finding direction to next node
        Vector3 nextNode = path.nodePositions[targetNode] + ofset;
        Vector3 dir = nextNode - transform.position;

        if (dir.magnitude < tolerance)
        {
            targetNode++;
            return;
        }
        
        transform.position += dir.normalized * speed;

        // handle rotation
        float dirDiff = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - transform.rotation.z;
        transform.rotation = Quaternion.Euler(0, 0, dirDiff + 90);
    }

    // desc: sets position to that of the closest node
    // pre: none
    // post: none
    public void GoToClosestNode()
    {
        int closestIndex = 0;
        float minDistance = Vector3.Distance(transform.position, path.nodePositions[0]);
        for (int i = 0; i < path.nodePositions.Length; i++) {
            float distance = Vector3.Distance(transform.position, path.nodePositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        closestIndex = Math.Clamp(closestIndex, 0, path.nodePositions.Length - 1);

        targetNode = closestIndex;
    }

    // desc: gets distance left to reach the end
    // pre: none
    // post: returns distance left as float
    private float GetDistanceToEnd()
    {
        if (targetNode >= path.nodePositions.Length) return -1f;

        float travelled = 0;
        for (int i = path.nodePositions.Length - 1; i > targetNode; i--)
        {
            travelled += Vector3.Distance(path.nodePositions[i], path.nodePositions[i - 1]);
        }

        travelled += Vector3.Distance(transform.position, path.nodePositions[targetNode]);
        return travelled;
    }

    // desc: gets distance travelled from start
    // pre: none
    // post: returns distance travelled as float
    private float GetDistanceTravelled()
    {
        if (targetNode >= path.nodePositions.Length - 1) return -1f;

        float travelled = 0;
        for (int i = 0; i < targetNode; i++)
        {
            travelled += Vector3.Distance(path.nodePositions[i], path.nodePositions[i + 1]);
        }

        travelled += Vector3.Distance(transform.position, path.nodePositions[targetNode]);
        return travelled;
    }


}
