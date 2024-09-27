using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMovementController : MonoBehaviour
{
    [SerializeField]
    private GameObject GhostRedPrefab,
        GhostYellowPrefab,
        GhostGreenPrefab,
        GhostBluePrefab;

    [SerializeField]
    private GameObject PlayerPrefab;

    [SerializeField]
    private GameObject waypoints;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the Player
        var player = Instantiate(PlayerPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        GhostInit();
    }

    void GhostInit()
    {
        // Instantiate the Ghosts
        var gRed = Instantiate(GhostRedPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        var gYellow = Instantiate(GhostYellowPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        var gGreen = Instantiate(GhostGreenPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        var gBlue = Instantiate(GhostBluePrefab, new Vector3(0, 0, -1), Quaternion.identity);
        // Set the waypoints for the Ghosts
        Transform[] waypointsList = waypoints.GetComponentsInChildren<Transform>();


        foreach (var waypoint in waypointsList)
        {
            if (waypoint.tag == "waypoint-red")
            {
                gRed.GetComponent<EnemyMovement>().waypoints.Add(waypoint);
            }
            if (waypoint.tag == "waypoint-yellow")
            {
                gYellow.GetComponent<EnemyMovement>().waypoints.Add(waypoint);
            }
            if (waypoint.tag == "waypoint-green")
            {
                gGreen.GetComponent<EnemyMovement>().waypoints.Add(waypoint);
            }
            if (waypoint.tag == "waypoint-blue")
            {
                gBlue.GetComponent<EnemyMovement>().waypoints.Add(waypoint);
            }
        }

    }
}
