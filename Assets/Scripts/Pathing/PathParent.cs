/* 1. description: UI logic to hold a path
 * 2. @author: Linden/Matthew
 * 3. @date: 21-12-24
 * 4. @version: 1.0
 */
using UnityEngine;

// PARENT OBJECT TO A BOX PATH
// must place 1 or more gameobjects as children of the object this is attached to
// path is drawn in order of hierarchy
// must call UpdateNodes(); for objects following to update their paths
public class PathParent : MonoBehaviour
{
    // GIZMOS (FOR EDITOR ONLY)
    [SerializeField] float pointSize;

    public Vector3[] nodePositions;

    private void Awake()
    {
        UpdateNodes();
    }

    // desc: updates list of node positions
    // pre: none
    // post: updates global nodePositions list
    public void UpdateNodes()
    {
        nodePositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            nodePositions[i] = transform.GetChild(i).position;
        }
    }

    // desc: used in editor to visualize path, DOES NOTHING ON RELEASE
    private void OnDrawGizmos()
    {
        if (transform.childCount == 0) return;
        Vector3[] childrenVectors = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childrenVectors[i] = transform.GetChild(i).position;
            Gizmos.DrawSphere(childrenVectors[i], pointSize);
        }

        Gizmos.DrawLineStrip(childrenVectors, false);
    }
}
