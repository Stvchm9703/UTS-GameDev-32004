using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PacStudentController : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;

    private GameObject player = null;
    private PacStudentMovement _pacStudentMovement = null;

    private LevelGenerator levelGenerator;
    private Waypoint currentWaypoint, lastWaypoint;

    private Direction? currentInput, lastInput; // 0: up , 1: 
    // Start is called before the first frame update
    private float fixedTimeUpdate = 2f;
    private float currentTime = 2f;

    private List<List<Direction>> directionBuffer = new List<List<Direction>>();
    

    [SerializeField] private GameObject commandGameObject;
    private List<TextMeshProUGUI> commandTexts = new List<TextMeshProUGUI>();

    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        currentWaypoint = levelGenerator.Waypoints.First(wp => wp.IsWalkable());
        Debug.Log(currentWaypoint);
        // Instantiate the Player
        player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        _pacStudentMovement = player.GetComponent<PacStudentMovement>();
        // _pacStudentMovement.SetTargetWaypoint(currentWaypoint);
        InitDebugCommand();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleKeyPressed();
        HandleMovement();

    }

    void LateUpdate()
    {
        DebugCommands();
    }

    void HandleKeyPressed()
    {
        var commandList = new List<Direction>();

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0.2f)
        {
            // Up
            commandList.Add(Direction.Up);
        }

        if (vertical < -0.2f)
        {
            commandList.Add(Direction.Down);
        }

        if (horizontal < -0.2f)
        {
            commandList.Add(Direction.Left);
        }

        if (horizontal > 0.2f)
        {
            commandList.Add(Direction.Right);
        }

        if (directionBuffer.Count == 50)
        {
            directionBuffer.RemoveAt(0);
        }

        if (directionBuffer.Count == 0)
        {
            directionBuffer.Add(commandList);
        }
        else
        {
            var lastCommand = directionBuffer.Last();
            if (lastCommand != null)
            {
                string lastCmd = string.Join(",", lastCommand.ConvertAll(w => (int)w));
                string commandCmd = string.Join(",", commandList.ConvertAll(w => (int)w));
                if (String.Equals(lastCmd, commandCmd) == false)
                {
                    directionBuffer.Add(commandList);
                }
            }
        }

        UpdateInput();
    }

    void HandleMovement()
    {
        // update fixtime 
        if (currentTime >= fixedTimeUpdate)
        {
            currentTime = 0f;
            lastInput = currentInput;
            currentInput = null;
            return;
        }
        currentTime += Time.fixedDeltaTime;  
        if(_pacStudentMovement.Arrived() == false) return;
        if(currentInput == null) return;
        var wp = levelGenerator.TryGetWalkable(currentWaypoint, currentInput.Value);
        if (wp != null)
        {
            lastWaypoint = currentWaypoint;
            _pacStudentMovement.SetTargetPosition(wp.Value);
            currentWaypoint = wp.Value;
        }
    }


    void UpdateInput()
    {
        if (directionBuffer.Count >= 2)
        {
            var lastcommand = directionBuffer[^1];
            var lastCommand2 = directionBuffer[^2];
            var diff = lastcommand.Except(lastCommand2)
                .Union(lastCommand2.Except(lastcommand)).ToList();
            if (diff.Count != 0)
            {
                lastInput = currentInput;
                currentInput = diff[0];
            }
            // Debug.Log($"lastInput: {(int)lastInput.Value} currentInput: {(int)currentInput.Value}");
            return;
        }

        if (directionBuffer.Count == 1)
        {
            var tmp = directionBuffer[0];
            if (tmp.Count == 0) return;
            
            currentInput = tmp.Last();
        }
        
    }


    void InitDebugCommand()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject go = new GameObject();
            go.name = $"cmd_{i}";
            go.transform.SetParent(commandGameObject.transform);
            go.transform.localPosition = new Vector3(0, 300 - i * 10, 0);
            var textSet = go.AddComponent<TextMeshProUGUI>();
            textSet.fontSize = 10;
            commandTexts.Add(textSet);
        }
    }

    void DebugCommands()
    {
        foreach (var commandSet in directionBuffer)
        {
            var index = directionBuffer.IndexOf(commandSet);
            string command = $"[{string.Join(",", commandSet)}]";
            commandTexts[index].text = command;
        }
    }
}