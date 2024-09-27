using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
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
    private Sprite outsideCornerWallSprite; // the [1] tile.  default should be top left corner as outter area as the default direaction

    [SerializeField]
    private Sprite outsideFlattenWallSpriteX; // the [2]X tile.  default should be top as outter area as the default direaction

    [SerializeField]
    private Sprite outsideFlattenWallSpriteY; // the [2]Y tile.  default should be Left as outter area as the default direaction

    [SerializeField]
    private Sprite insideCornerWallSprite; // the [3] tile.  default should be top left corner as inner area as the default direaction

    [SerializeField]
    private Sprite insideFlattenWallSpriteX; // the [4]X tile.  default should be top as inner area as the default direaction

    [SerializeField]
    private Sprite insideFlattenWallSpriteY; // the [4]Y tile.  default should be Left as inner area as the default direaction

    [SerializeField]
    private Sprite insideAreaSprite; // the [5] and [6] tile.  the inner area as the default direaction

    // [5] for normal pallet, [6] for the power pallet

    // the [7] tile.  the adjoint area as the default direaction


    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private TilemapRenderer tileRenderer;

    // Sprite asset types


    void Start()
    {
        GenerateMap();
    }

    // generate the map based on the levelMap array
    private void GenerateMap()
    {
        for (int y = 0; y < levelMap.GetLength(0); y++)
        {
            for (int x = 0; x < levelMap.GetLength(1); x++)
            {
                int tileType = levelMap[y, x];

                Vector3Int tilePosition = new Vector3Int(x, -y, 0);
                Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
                
                switch (tileType)
                {
                    case 0:
                        break;
                    case 1:
                        tileInstance = GetOutsideCornerWallTile(x, y);
                        break;
                    case 2:
                        tileInstance = GetOutsideWallTile(x, y);
                        break;
                    case 3:
                        tileInstance = GetInsideCornerWallTile(x, y);
                        break;
                    case 4:
                        tileInstance = GetInsideWallTile(x, y);
                        break;
                    case 5:
                        tileInstance.sprite = insideAreaSprite;
                        break;
                    case 6:
                        tileInstance.sprite = insideAreaSprite;
                        break;
                    case 7:
                        tileInstance.sprite = insideAreaSprite;
                        break;
                }
                tilemap.SetTile(tilePosition, tileInstance);
            }
        }
    }

    // find the direction of the inner place
    // 1 for the top left corner
    // 2 for the top
    // 3 for the top right corner
    // 4 for the middle left
    // 5 for the middle  (which is self)
    // 6 for the middle right
    // 7 for the bottom left corner
    // 8 for the bottom
    // 9 for the bottom right corner



    // outside corner wall sprite
    // it only has 4 direction
    // check the top, left, right, bottom has 1 or 2
    private int GetOutsideCornerWallDirection(int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        if ((top == 1 || top == 2) && (left == 1 || left == 2))
        {
            // bottom right corner
            return 9;
        }
        else if ((top == 1 || top == 2) && (right == 1 || right == 2))
        {
            // bottom left corner
            return 7;
        }
        else if ((bottom == 1 || bottom == 2) && (left == 1 || left == 2))
        {
            // top right corner
            return 3;
        }
        else if ((bottom == 1 || bottom == 2) && (right == 1 || right == 2))
        {
            // top left corner
            return 1;
        }
        return 0;
    }

    private Tile GetOutsideCornerWallTile(int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = outsideCornerWallSprite;

        var direaction = GetOutsideCornerWallDirection(x, y);
        switch (direaction)
        {
            case 1:
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, 1, 1)
                );
                break;
            case 3:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, 1, 1)
                );
                break;
            case 7:

                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, -1, 1)
                );
                break;
            case 9:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, -1, 1)
                );
                break;
        }
        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }

    private int GetOutsideWallDirection(int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        if ((top == 1 || top == 2 || top == 7) && (bottom == 1 || bottom == 2 || bottom == 7))
        {
            if (left == 5 || left == 6)
                return 4;
            // vertical wall
            if (right == 5 || right == 6)
                return 6;
            return -1;
        }
        else if ((left == 1 || left == 2 || left == 7) && (right == 1 || right == 2 || right == 7))
        {
            // horizontal wall
            if (top == 5 || top == 6)
                return 2;
            if (bottom == 5 || bottom == 6)
                return 8;
            return -2;
        }
        return 0;
    }

    private Tile GetOutsideWallTile(int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        var direction = GetOutsideWallDirection(x, y);
        if (direction == 2)
        {
            tileInstance.sprite = outsideFlattenWallSpriteX;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 0),
                new Vector3(1, -1, 1)
            );
        }
        else if (direction == 4)
        {
            tileInstance.sprite = outsideFlattenWallSpriteY;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 0),
                new Vector3(-1, 1, 1)
            );
        }
        else if (direction == 6 || direction == -1)
        {
            tileInstance.sprite = outsideFlattenWallSpriteY;
        }
        else if (direction == 8 || direction == -2)
        {
            tileInstance.sprite = outsideFlattenWallSpriteX;
        }

        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }

    private int GetInsideCornerWallDirection(int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        if ((top == 3 || top == 4) && (left == 3 || left == 4))
        {
            // bottom right corner
            return 9;
        }
        else if ((top == 3 || top == 4) && (right == 3 || right == 4))
        {
            // bottom left corner
            return 7;
        }
        else if ((bottom == 3 || bottom == 4) && (left == 3 || left == 4))
        {
            // top right corner
            return 3;
        }
        else if ((bottom == 3 || bottom == 4) && (right == 3 || right == 4))
        {
            // top left corner
            return 1;
        }
        return 0;
    }

    private Tile GetInsideCornerWallTile(int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = insideCornerWallSprite;

        var direaction = GetInsideCornerWallDirection(x, y);
        switch (direaction)
        {
            case 1:
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, 1, 1)
                );
                break;
            case 3:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, 1, 1)
                );
                break;
            case 7:

                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, -1, 1)
                );
                break;
            case 9:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, -1, 1)
                );
                break;
        }
        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }

    private int GetInsideWallDirection(int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        if ((top == 3 || top == 4 || top == 7) && (bottom == 3 || bottom == 4 || bottom == 7))
        {
            if (left == 5 || left == 6)
                return 4;
            // vertical wall
            if (right == 5 || right == 6)
                return 6;
            return -1;
        }
        else if ((left == 3 || left == 4 || left == 7) && (right == 3 || right == 4 || right == 7))
        {
            // horizontal wall
            if (top == 5 || top == 6)
                return 2;
            if (bottom == 5 || bottom == 6)
                return 8;
            return -2;
        }
        else if (top == 3 || top == 4 || top == 7 || bottom == 3 || bottom == 4 || bottom == 7)
        {
            return -1;
        }
        else if (left == 3 || left == 4 || left == 7 || right == 3 || right == 4 || right == 7)
        {
            return -2;
        }
        Debug.Log("x: " + x + " y: " + y + " c:" + levelMap[y, x]);
        Debug.Log("top: " + top + " left: " + left + " right: " + right + " bottom: " + bottom);
        return 0;
    }

    private Tile GetInsideWallTile(int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        var direction = GetInsideWallDirection(x, y);
        if (direction == 2 || direction == -2)
        {
            tileInstance.sprite = insideFlattenWallSpriteX;
        }
        else if (direction == 4 || direction == -1)
        {
            tileInstance.sprite = insideFlattenWallSpriteY;
        }
        else if (direction == 6)
        {
            tileInstance.sprite = insideFlattenWallSpriteY;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 0),
                new Vector3(-1, 1, 1)
            );
        }
        else if (direction == 8)
        {
            tileInstance.sprite = insideFlattenWallSpriteX;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 0),
                new Vector3(1, -1, 1)
            );
        }

        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }
}
