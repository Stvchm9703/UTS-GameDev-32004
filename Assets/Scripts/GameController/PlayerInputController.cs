using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerPrefab;

    private GameObject player = null;

    private LevelGenerator levelGenerator;
    private Waypoint currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        currentWaypoint = levelGenerator.Waypoints[0];
        // Instantiate the Player
        player = Instantiate(PlayerPrefab, currentWaypoint.position, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        onKeyPressed();
    }

    void onKeyPressed()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (vertical > 0)
        {
            Debug.Log("Move Up");
            MoveUp();
        }
        else if (vertical < 0)
        {
            Debug.Log("Move Down");
            MoveDown();
        }
        if (horizontal > 0)
        {
            Debug.Log("Move Right");
            MoveRight();
        }
        else if (horizontal < 0)
        {
            Debug.Log("Move Left");
            MoveLeft();
        }
    }

    void MoveUp()
    {
        if (currentWaypoint.Up != null)
        {
            // Debug.Log("currentWaypoint.Up : " + currentWaypoint.Up.position);
            // currentWaypoint = currentWaypoint.Up;
            // player.transform.position = currentWaypoint.position;

            var targetPosition = player.transform.position;
            targetPosition.y -= 5;

            player.GetComponent<PlayerMovement>().SetTargetWaypoint(targetPosition);
        }
    }

    void MoveDown()
    {
        if (currentWaypoint.Down != null)
        {
            // Debug.Log("currentWaypoint.Down : " + currentWaypoint.Down.position);
            // currentWaypoint = currentWaypoint.Down;
            // player.transform.position = currentWaypoint.position;
            var targetPosition = player.transform.position;
            targetPosition.y += 5;

            player.GetComponent<PlayerMovement>().SetTargetWaypoint(targetPosition);
        }
    }

    void MoveRight()
    {
        if (currentWaypoint.Right != null)
        {
            // Debug.Log("currentWaypoint.Right : " + currentWaypoint.Right.position);
            // currentWaypoint = currentWaypoint.Right;
            // player.transform.position = currentWaypoint.position;
            var targetPosition = new Vector3(
                player.transform.position.x + 5,
                player.transform.position.y,
                player.transform.position.z
            );

            player.GetComponent<PlayerMovement>().SetTargetWaypoint(targetPosition);
        }
    }

    void MoveLeft()
    {
        if (currentWaypoint.Left != null)
        {
            // Debug.Log("currentWaypoint.Left : " + currentWaypoint.Left.position);
            // currentWaypoint = currentWaypoint.Left;
            // player.transform.position = currentWaypoint.position;
            var targetPosition = player.transform.position;
            targetPosition.x -= 5;
            player.GetComponent<PlayerMovement>().SetTargetWaypoint(currentWaypoint.position);
        }
    }
}
