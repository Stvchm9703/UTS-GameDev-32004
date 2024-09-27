using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRender : MonoBehaviour
{
 

    [SerializeField]
    private Sprite outsideCornerWallSprite; // the [1] tile.  default should be top left corner as outter area as the default direaction

    [SerializeField]
    private Sprite outsideFlattenWallSprite; // the [2] tile.  default should be Left as outter area as the default direaction

    [SerializeField]
    private Sprite insideCornerWallSprite; // the [3] tile.  default should be top left corner as inner area as the default direaction

    [SerializeField]
    private Sprite insideFlattenWallSprite; // the [4] tile.  default should be top as inner area as the default direaction

    [SerializeField]
    private Sprite normalPalletSprite; // the [5] tile.  the inner area as the default direaction

    [SerializeField]
    private Sprite powerPalletSprite; // the [6] tile.  the inner area as the default direaction

    [SerializeField]
    private Sprite AdaptiveCornerWallSprite;

    // the [7] tile.  the adjoint area as the default direaction


    [SerializeField]
    private Tilemap wallTilemap,
        itemTilemap;

    // Sprite asset types



    // generate the map based on the levelMap array
    public void RenderMap(int[,] levelMap)
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
                        tileInstance = GetOutsideCornerWallTile(levelMap, x, y);
                        wallTilemap.SetTile(tilePosition, tileInstance);

                        break;
                    case 2:
                        tileInstance = GetOutsideWallTile(levelMap, x, y);
                        wallTilemap.SetTile(tilePosition, tileInstance);

                        break;
                    case 3:
                        tileInstance = GetInsideCornerWallTile(levelMap, x, y);
                        wallTilemap.SetTile(tilePosition, tileInstance);

                        break;
                    case 4:
                        tileInstance = GetInsideWallTile(levelMap, x, y);
                        wallTilemap.SetTile(tilePosition, tileInstance);

                        break;
                    case 5:
                        tileInstance.sprite = normalPalletSprite;
                        itemTilemap.SetTile(tilePosition, tileInstance);
                        break;
                    case 6:
                        tileInstance.sprite = powerPalletSprite;
                        itemTilemap.SetTile(tilePosition, tileInstance);
                        break;
                    case 7:
                        tileInstance = GetAdaptiveCornerWallTile(levelMap, x, y);
                        wallTilemap.SetTile(tilePosition, tileInstance);
                        break;
                }
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
    private int GetOutsideCornerWallDirection(int[,] levelMap, int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        int topL = x - 1 >= 0 && y - 1 >= 0 ? levelMap[y - 1, x - 1] : 0;
        int topR = x + 1 < levelMap.GetLength(1) && y - 1 >= 0 ? levelMap[y - 1, x + 1] : 0;
        int bottomL = x - 1 >= 0 && y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x - 1] : 0;
        int bottomR =
            x + 1 < levelMap.GetLength(1) && y + 1 < levelMap.GetLength(0)
                ? levelMap[y + 1, x + 1]
                : 0;

        
       
        if ((top == 1 || top == 2) && (left == 1 || left == 2))
        {
            if (top == 1) return 3;
            return 9;
        }
        else if ((top == 1 || top == 2) && (right == 1 || right == 2))
        {
            if (top == 1) return 1;
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

    private Tile GetOutsideCornerWallTile(int[,] levelMap, int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = outsideCornerWallSprite;

        var direaction = GetOutsideCornerWallDirection(levelMap, x, y);
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

    private int GetOutsideWallDirection(int[,] levelMap, int x, int y)
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

    private Tile GetOutsideWallTile(int[,] levelMap, int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        var direction = GetOutsideWallDirection(levelMap, x, y);
        tileInstance.sprite = outsideFlattenWallSprite;
        // if (direction == 2 || direction == -2 || direction == 8)
        // {
        //     // tileInstance.transform = Matrix4x4.TRS(
        //     //     Vector3.zero,
        //     //     Quaternion.Euler(0, 0, 0),
        //     //     new Vector3(1, 1, 1)
        //     // );
        // }
        if (direction == 4 || direction == 6 || direction == -1)
        {
            tileInstance.sprite = outsideFlattenWallSprite;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 90),
                new Vector3(1, 1, 1)
            );
        }

        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }

    private int GetInsideCornerWallDirection(int[,] levelMap, int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        if ((top == 3 || top == 4) && (left == 3 || left == 4))
        {
            // bottom right corner
            // if (top == 3) return 3;
            return 9;
        }
        else if ((top == 3 || top == 4) && (right == 3 || right == 4))
        {
            // bottom left corner
            // if (top == 3) return 1;
            return 7;
        }
        else if ((bottom == 3 || bottom == 4) && (left == 3 || left == 4))
        {
            // top right corner
            // if (top == 3) return 1;
            return 3;
        }
        else if ((bottom == 3 || bottom == 4) && (right == 3 || right == 4))
        {
            // top left corner
            // if (top == 3) return 3;
            return 1;
        }
        return 0;
    }

    private Tile GetInsideCornerWallTile(int[,] levelMap, int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = insideCornerWallSprite;

        var direaction = GetInsideCornerWallDirection(levelMap, x, y);
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

    private int GetInsideWallDirection(int[,] levelMap, int x, int y)
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

        return 0;
    }

    private Tile GetInsideWallTile(int[,] levelMap, int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = insideFlattenWallSprite;

        var direction = GetInsideWallDirection(levelMap, x, y);
        // if (direction == 2 || direction == -2 || direction == 8) { }
        if (direction == 4 || direction == -1 || direction == 6)
        {
            // tileInstance.sprite = insideFlattenWallSpriteY;
            tileInstance.transform = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(0, 0, 90),
                new Vector3(1, 1, 1)
            );
        }

        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }

    private int GetAdaptiveCornerWallDirection(int[,] levelMap, int x, int y)
    {
        int top = y - 1 >= 0 ? levelMap[y - 1, x] : 0;
        int left = x - 1 >= 0 ? levelMap[y, x - 1] : 0;
        int right = x + 1 < levelMap.GetLength(1) ? levelMap[y, x + 1] : 0;
        int bottom = y + 1 < levelMap.GetLength(0) ? levelMap[y + 1, x] : 0;

        // single wall as up , double wall as left

        if ((top == 3 || top == 4) && (left == 1 || left == 2))
        {
            return 1;
        }
        else if ((top == 3 || top == 4) && (right == 1 || right == 2))
        {
            return 3;
        }
        else if ((bottom == 3 || bottom == 4) && (left == 1 || left == 2))
        {
            return 7;
        }
        else if ((bottom == 3 || bottom == 4) && (right == 1 || right == 2))
        {
            return 9;
        }
        return 0;
    }

    private Tile GetAdaptiveCornerWallTile(int[,] levelMap, int x, int y)
    {
        Tile tileInstance = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        tileInstance.flags = TileFlags.None;
        tileInstance.sprite = AdaptiveCornerWallSprite;

        var direaction = GetAdaptiveCornerWallDirection(levelMap, x, y);
        switch (direaction)
        {
            case 1:
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, -1, 1)
                );
                break;
            case 3:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, -1, 1)
                );
                break;
            case 7:

                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(1, 1, 1)
                );
                break;
            case 9:
                /// fixed
                tileInstance.transform = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 0, 0),
                    new Vector3(-1, 1, 1)
                );
                break;
        }
        tileInstance.flags = TileFlags.LockAll;
        return tileInstance;
    }
}
