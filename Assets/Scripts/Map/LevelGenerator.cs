using System;
using System.Collections;
using System.Collections.Generic;
// using System.Text.Json;
using System.Linq;
using System.Text;
// using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Windows;

//
[Serializable]
public struct Waypoint
{
    public int x, y;
    public Vector3 position;

    public int gridType;
    // public bool IsIntersection;
    // public bool IsWayX, IsWayY;

    public Waypoint(int row, int col, int gridType, Vector3 position)
    {
        this.x = col;
        this.y = row;
        this.gridType = gridType;
        this.position = position;
        // this.IsIntersection = isIntersection;
        // this.IsWayX = isWayX;
        // this.IsWayY = isWayY;
    }

    public bool IsWalkable()
    {
        return gridType == 5 || gridType == 6 || gridType == 0;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}, {3}]", x, y, gridType, position.ToString());
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class LevelGenerator : MonoBehaviour
{
    private readonly int[,] levelMap =
    {
        { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 4 },
        { 2, 6, 4, 0, 0, 4, 5, 4, 0, 0, 0, 4, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 3, 5, 3, 4, 4, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 4, 4, 5, 3, 4, 4, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 4, 4, 5, 5, 5, 5, 4 },
        { 1, 2, 2, 2, 2, 1, 5, 4, 3, 4, 4, 3, 0, 4 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 3, 4, 4, 3, 0, 3 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 0 },
        { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 }
    };

    // private int[,] debugMap ;

    [SerializeField] public List<Waypoint> Waypoints { get; private set; }


    [SerializeField] private MapRender mapRender;


    // Start is called before the first frame update
    private void Awake()
    {
        Waypoints = new List<Waypoint>();
        var fullLevelMap = GenerateFullMap(levelMap);
        // PrintMap(fullLevelMap); // Optional: For Debugging

        if (mapRender != null) mapRender.RenderMap(fullLevelMap);

        // Debug.Log(waypoints);
        AdjustCamera(fullLevelMap);
        GenerateWaypoint(fullLevelMap);
    }


    // Function to generate the full map with horizontal and vertical flipping
    private int[,] GenerateFullMap(int[,] originalMap)
    {
        var rows = originalMap.GetLength(0);
        var cols = originalMap.GetLength(1);

        // Remove the bottom row for vertical mirroring
        // var trimmedRows = rows - 1;

        // Full map size will be 2x the original in both dimensions (mirroring horizontally and vertically)
        var fullMap = new int[rows * 2, cols * 2];

        // Copy original to top-left quadrant
        CopyMapSection(originalMap, fullMap, 0, 0, rows, cols);

        // Create and copy horizontally mirrored version to top-right quadrant
        var hMirrored = FlipHorizontally(originalMap);
        CopyMapSection(hMirrored, fullMap, 0, cols, rows, cols);

        // Create and copy vertically mirrored version to bottom-left quadrant
        var vMirrored = FlipVertically(originalMap);
        CopyMapSection(vMirrored, fullMap, rows, 0, rows, cols);

        // Create and copy both horizontally and vertically mirrored to bottom-right quadrant
        var hvMirrored = FlipHorizontally(vMirrored);
        CopyMapSection(hvMirrored, fullMap, rows, cols, rows, cols);


        // Create a new array with one less element
        int[,] newArray = new int[rows * 2 - 1, cols * 2];
        // Copy rows before the middle row
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols * 2; col++)
            {
                newArray[row, col] = fullMap[row, col];
            }
        }

        // Copy rows after the middle row
        for (int i = rows + 1; i < rows * 2; i++)
        {
            for (int col = 0; col < cols * 2; col++)
            {
                newArray[i - 1, col] = fullMap[i, col];
            }
        }

        newArray[rows, 0] = 9;

        return newArray;
    }

    // Function to copy a section of one map into another at a specified location
    private void CopyMapSection(
        int[,] source,
        int[,] destination,
        int destRow,
        int destCol,
        int numRows,
        int numCols
    )
    {
        for (var row = 0; row < numRows; row++)
        for (var col = 0; col < numCols; col++)
            destination[destRow + row, destCol + col] = source[row, col];
    }

    // Flip the map horizontally (mirroring across the vertical axis)
    private int[,] FlipHorizontally(int[,] map)
    {
        var rows = map.GetLength(0);
        var cols = map.GetLength(1);
        var flipped = new int[rows, cols];

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
            flipped[row, cols - 1 - col] = map[row, col];

        return flipped;
    }

    // Flip the map vertically (mirroring across the horizontal axis)
    private int[,] FlipVertically(int[,] map)
    {
        var rows = map.GetLength(0);
        var cols = map.GetLength(1);
        var flipped = new int[rows, cols]; // Removing the bottom row as specified

        for (var row = 0; row < rows; row++) // Skip bottom row
        for (var col = 0; col < cols; col++)
            flipped[rows - 1 - row, col] = map[row, col]; // Offset by -2 since we skip the bottom row

        return flipped;
    }

    // Optional: Debug function to print the full map
    void PrintMap(int[,] map)
    {
        string line = "";
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                line += map[row, col] + ",";
            }

            line += "\n";
        }

        File.WriteAllBytes(Application.streamingAssetsPath + "/map.txt", Encoding.UTF8.GetBytes(line));
    }

    void AdjustCamera(int[,] fullMap)
    {
        Camera.main.orthographicSize = (float)fullMap.GetLength(0) / 2f;
        Camera.main.transform.position = new Vector3(
            (float)fullMap.GetLength(1) / 2f,
            -(float)fullMap.GetLength(0) / 2f + 1f,
            -10f
        ); // Adjust Z as necessary
    }

    void GenerateWaypoint(int[,] fullMap)
    {
        for (int row = 0; row < fullMap.GetLength(0); row++)
        {
            // the y-axis
            for (int col = 0; col < fullMap.GetLength(1); col++)
            {
                var position = mapRender.GetTilePosition(row, col);
                Waypoint waypoint = new Waypoint(row, col, fullMap[row, col], position);
                Waypoints.Add(waypoint);
            }
        }
    }

    public Waypoint? TryGetWalkable(Waypoint currWp, Direction direction)
    {
        // Debug.Log("get command");
        // Debug.Log($"r:{currWp.x} c:{currWp.y} d:{direction.ToString()}");
        Waypoint? targetWp = null;
        switch (direction)
        {
            case Direction.Up:
                // if (waypoints.Any(w => w.x == row && w.y == col-1) == false) return false;
                targetWp = Waypoints.Find(w => w.x == currWp.x && w.y == currWp.y - 1);
                if (targetWp == null) return null;
                if (targetWp.Value.IsWalkable() == false) return null;
                return targetWp;

            case Direction.Down:
                targetWp = Waypoints.Find(w => w.x == currWp.x && w.y == currWp.y + 1);
                if (targetWp == null) return null;
                if (targetWp?.IsWalkable() == false) return null;
                return targetWp;

            case Direction.Left:
                targetWp = Waypoints.Find(w => w.x == currWp.x - 1 && w.y == currWp.y);
                if (targetWp == null) return null;
                if (targetWp?.IsWalkable() == false) return null;
                return targetWp;

            case Direction.Right:
                targetWp = Waypoints.Find(w => w.x == currWp.x + 1 && w.y == currWp.y);
                if (targetWp == null) return null;
                if (targetWp?.IsWalkable() == false) return null;
                return targetWp;
        }

        return null;
    }
}