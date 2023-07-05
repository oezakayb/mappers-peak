using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;

public class HologramPath : MonoBehaviour
{
    [Header("Marker Configuration")] 
    private List<Vector3> potentialMarkerPositions;
    [SerializeField] private GameObject[] markers;
    [SerializeField] private Vector3 inactivePosition;
    [SerializeField] private float markerDistance = 5.0f;
    [SerializeField] private int skipAFewMarkers = 2;
    [Header("Agents Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    private NavMeshHit playerMesh = new NavMeshHit();
    private NavMeshHit targetMesh = new NavMeshHit();

    private NavMeshPath _currentPath;
    private NavMeshAgent _navAgent;

    // Start is called before the first frame update
    private void Start()
    {
        _currentPath = new NavMeshPath();
    }

    // Update is called once per frame
    private void Update()
    {
        NavMesh.SamplePosition(player.position, out playerMesh, 10.0f, NavMesh.AllAreas);
        NavMesh.SamplePosition(target.position, out targetMesh, 10.0f, NavMesh.AllAreas);
        
        NavMesh.CalculatePath(targetMesh.position, playerMesh.position, NavMesh.AllAreas, _currentPath);
        
        UpdateHologramMarkers(_currentPath.corners);

        for (int i = 0; i < markers.Length; i++)
        {
            if (potentialMarkerPositions.Count > i + skipAFewMarkers)
            {
                markers[i].transform.position = potentialMarkerPositions[potentialMarkerPositions.Count - 1 - skipAFewMarkers - i];
            }
            else
            {
                markers[i].transform.position = inactivePosition;
            }
        }
    }

    private void UpdateHologramMarkers(Vector3[] path)
    {
        if (path.Length < 2)
        {
            return;
        }

        potentialMarkerPositions = new List<Vector3>();
        var rest = 0.0f;
        var from = Vector3.zero;
        var to = Vector3.zero;

        for (int i = 0; i < path.Length - 1; i++)
        {
            from = path[i];
            to = path[i + 1];

            var pathSegmentLength = Vector3.Distance(to, from);
            var remainingDistance = pathSegmentLength + rest;

            while (remainingDistance > markerDistance)
            {
                potentialMarkerPositions.Add(from + (to - from).normalized * (pathSegmentLength - remainingDistance));
                remainingDistance -= markerDistance;
            }
            
            rest = remainingDistance;
        }
    }
}