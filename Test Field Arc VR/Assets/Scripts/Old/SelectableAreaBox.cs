using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableAreaBox : MonoBehaviour
{
    public float width = 5f;
    public float length = 5f;

    public Vector3 GetRandomPoint()
    {
        Vector3 pointA = transform.position + new Vector3(-width / 2, 0, -length / 2);
        Vector3 pointB = transform.position + new Vector3(width / 2, 0, length / 2);

        return new Vector3(Random.Range(pointA.x, pointB.x), transform.position.y, Random.Range(pointA.z, pointB.z));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 pointA = transform.position + new Vector3(width / 2, 0, length / 2);
        Vector3 pointB = transform.position + new Vector3(-width / 2, 0, length / 2);
        Vector3 pointC = transform.position + new Vector3(-width / 2, 0, -length / 2);
        Vector3 pointD = transform.position + new Vector3(width / 2, 0, -length / 2);

        Gizmos.DrawLine(pointA, pointB);
        Gizmos.DrawLine(pointB, pointC);
        Gizmos.DrawLine(pointC, pointD);
        Gizmos.DrawLine(pointD, pointA);
    }
}
