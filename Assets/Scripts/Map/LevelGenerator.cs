using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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
    private MapRender mapRender;


    // Start is called before the first frame update
    void Start()
    {
        int[,] fullLevelMap = GenerateFullMap(levelMap);
        // PrintMap(fullLevelMap); // Optional: For Debugging
        if (mapRender != null)
        {
            mapRender.RenderMap(fullLevelMap);
        }

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
}
