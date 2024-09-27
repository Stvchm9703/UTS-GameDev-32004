using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public int[] coor;
    public List<int> directions;
    public Vector3 position;
    public Waypoint Up,
        Down,
        Left,
        Right;

    public Waypoint(int row, int col, Vector3 position)
    {
        this.coor = new int[2] { row, col };
        this.position = position;
    }
}

public class LevelGenerator : MonoBehaviour
{
    private int[,] levelMap =
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
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 },
    };

    [SerializeField]
    private List<Waypoint> waypoints = new List<Waypoint>();

    public List<Waypoint> Waypoints
    {
        get => waypoints;
    }

    [SerializeField]
    private MapRender mapRender;

    // Start is called before the first frame update
    void Awake()
    {
        int[,] fullLevelMap = GenerateFullMap(levelMap);
        // PrintMap(fullLevelMap); // Optional: For Debugging
        if (mapRender != null)
        {
            mapRender.RenderMap(fullLevelMap);
        }
        GenerateWaypoint(fullLevelMap);

        AdjustCamera(fullLevelMap);
    }

    // Function to generate the full map with horizontal and vertical flipping
    int[,] GenerateFullMap(int[,] originalMap)
    {
        int rows = originalMap.GetLength(0);
        int cols = originalMap.GetLength(1);

        // Remove the bottom row for vertical mirroring
        int trimmedRows = rows - 1;

        // Full map size will be 2x the original in both dimensions (mirroring horizontally and vertically)
        int[,] fullMap = new int[trimmedRows * 2, cols * 2];

        // Copy original to top-left quadrant
        CopyMapSection(originalMap, fullMap, 0, 0, rows, cols);

        // Create and copy horizontally mirrored version to top-right quadrant
        int[,] hMirrored = FlipHorizontally(originalMap);
        CopyMapSection(hMirrored, fullMap, 0, cols, rows, cols);

        // Create and copy vertically mirrored version to bottom-left quadrant
        int[,] vMirrored = FlipVertically(originalMap);
        CopyMapSection(vMirrored, fullMap, trimmedRows, 0, trimmedRows, cols);

        // Create and copy both horizontally and vertically mirrored to bottom-right quadrant
        int[,] hvMirrored = FlipHorizontally(vMirrored);
        CopyMapSection(hvMirrored, fullMap, trimmedRows, cols, trimmedRows, cols);

        return fullMap;
    }

    // Function to copy a section of one map into another at a specified location
    void CopyMapSection(
        int[,] source,
        int[,] destination,
        int destRow,
        int destCol,
        int numRows,
        int numCols
    )
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                destination[destRow + row, destCol + col] = source[row, col];
            }
        }
    }

    // Flip the map horizontally (mirroring across the vertical axis)
    int[,] FlipHorizontally(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        int[,] flipped = new int[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                flipped[row, cols - 1 - col] = map[row, col];
            }
        }
        return flipped;
    }

    // Flip the map vertically (mirroring across the horizontal axis)
    int[,] FlipVertically(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        int[,] flipped = new int[rows - 1, cols]; // Removing the bottom row as specified

        for (int row = 0; row < rows - 1; row++) // Skip bottom row
        {
            for (int col = 0; col < cols; col++)
            {
                flipped[rows - 2 - row, col] = map[row, col]; // Offset by -2 since we skip the bottom row
            }
        }
        return flipped;
    }

    // Optional: Debug function to print the full map
    void PrintMap(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            string line = "";
            for (int col = 0; col < cols; col++)
            {
                line += map[row, col] + " ";
            }
            Debug.Log(line);
        }
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
            for (int col = 0; col < fullMap.GetLength(1); col++)
            {
                if (fullMap[row, col] == 5 || fullMap[row, col] == 6 || fullMap[row, col] == 0)
                {
                    List<int> directions = IsWaypoint(col, row, fullMap);
                    if (directions == null)
                    {
                        continue;
                    }
                    var position = mapRender.GetTilePosition(row, col);
                    Waypoint waypoint = new Waypoint(row, col, position);
                    waypoint.directions = directions;
                    waypoints.Add(waypoint);
                }
            }
        }

        foreach (Waypoint waypoint in waypoints)
        {
            foreach (int direction in waypoint.directions)
            {
                switch (direction)
                {
                    case 2:
                        waypoint.Up = GetWaypoint(waypoint.coor[0], waypoint.coor[1], 8);
                        break;
                    case 4:
                        waypoint.Left = GetWaypoint(waypoint.coor[0], waypoint.coor[1], 6);
                        break;
                    case 6:
                        waypoint.Right = GetWaypoint(waypoint.coor[0], waypoint.coor[1], 4);
                        break;
                    case 8:
                        waypoint.Down = GetWaypoint(waypoint.coor[0], waypoint.coor[1], 2);
                        break;
                }
            }
        }
    }

    List<int> IsWaypoint(int x, int y, int[,] levelMap)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : -1;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : -1;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : -1;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : -1;

        // check self
        if (!IsWalkable(levelMap[y, x]))
        {
            return null;
        }

        // check is corner
        if (
            IsWalkable(top) && IsWalkable(left)
            || IsWalkable(top) && IsWalkable(right)
            || IsWalkable(bottom) && IsWalkable(left)
            || IsWalkable(bottom) && IsWalkable(right)
        )
        {
            List<int> wp = new List<int>();
            if (IsWalkable(top))
                wp.Add(2);
            if (IsWalkable(left))
                wp.Add(4);
            if (IsWalkable(right))
                wp.Add(6);
            if (IsWalkable(bottom))
                wp.Add(8);
            return wp;
        }

        return null;
    }

    bool IsWalkable(int val)
    {
        return val == 5 || val == 6;
    }

    Waypoint GetWaypoint(int row, int col, int direction)
    {
        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint.directions.Contains(direction))
            {
                if (waypoint.coor[0] == row && (direction == 8 || direction == 2))
                {
                    return waypoint;
                }
                if (waypoint.coor[1] == col && (direction == 4 || direction == 6))
                {
                    return waypoint;
                }
            }
        }

        return null;
    }
}
