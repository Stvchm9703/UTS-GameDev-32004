using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class CherryController : MonoBehaviour
{
    /// Cherry-Controller
    /// for the adding the item Collider for normal or power  
    float RespwanTime = 10f;

    float RespwanTimer = 0f;
    bool isReady = false;
    // [SerializeField] bool debug = false;

    [SerializeField] int maxPowerCount = 0;


    List<Waypoint> AvailableWaypoints, WalkableWaypoints;
    List<ItemCollider> itemColliders;
    GameObject levelGenGameObj;
    LevelGenerator levelGenerator;
    MapRender mapRender;
    Queue<IEnumerator> SpawnerQueue;

    void Start()
    {
        SpawnerQueue = new Queue<IEnumerator>();
        itemColliders = new List<ItemCollider>();
        WalkableWaypoints = new List<Waypoint>();

        InitItemColliders();
        StartCoroutine(ProcessSpawner());
    }


    void InitItemColliders()
    {
        levelGenGameObj = GameObject.Find("Grid");
        levelGenerator = levelGenGameObj.GetComponent<LevelGenerator>();

        mapRender = levelGenGameObj.GetComponent<MapRender>();
        bool shouldManualMaxCount = maxPowerCount > 0;


        if (levelGenerator != null)
        {
            // AvailableWaypoints = levelGenerator.Waypoints.Where(wp => wp.IsWalkable()).ToList();
            AvailableWaypoints = new List<Waypoint>();
            WalkableWaypoints = levelGenerator.Waypoints.Where(wp => wp.IsWalkable()).ToList();
            foreach (var wp in WalkableWaypoints)
            {
                CreateItemCollider(wp);
                if (shouldManualMaxCount == false && wp.gridType == 6)
                {
                    maxPowerCount++;
                }
            }

            this.isReady = true;
        }
    }

    void CreateItemCollider(Waypoint wp)
    {
        var itemCollider = new GameObject("wp");
        // itemCollider.transform.SetParent(levelGenGameObj.transform);
        itemCollider.transform.localPosition = wp.position;


        var itmCol = itemCollider.AddComponent<ItemCollider>();
        itmCol.SetupFromCherryController(wp, this);

        itemColliders.Add(itmCol);
    }

    public void OnItemHit(ItemCollider itemCollider)
    {
        // Destory process
        this.AvailableWaypoints.Add(itemCollider.waypoint);
        this.itemColliders.Remove(itemCollider);
        if (itemCollider.itemType == 6)
        {
            this.SpawnerQueue.Enqueue(RespwanPowerCherry());
        }

        mapRender.RemoveItemTile(itemCollider.waypoint.x, itemCollider.waypoint.y);

        Destroy(itemCollider.gameObject);
    }

    IEnumerator RespwanPowerCherry()
    {
        Debug.Log("RespwanPowerCherry");
        yield return new WaitForSeconds(RespwanTime);
        // Debug.Log($"AvailableWaypoints : {AvailableWaypoints.Count}");
        int randomIndex = Random.Range(0, AvailableWaypoints.Count);
        var randomWp = AvailableWaypoints[randomIndex];
        randomWp.gridType = 6;
        CreateItemCollider(randomWp);
        mapRender.AddItemTile(randomWp.x, randomWp.y, randomWp.gridType);
        yield return null;
    }


    IEnumerator ProcessSpawner()
    {
        while (true)
        {
            while (SpawnerQueue.Count > 0 && isReady)
            {
                yield return StartCoroutine(SpawnerQueue.First());
                SpawnerQueue.Dequeue();
            }

            yield return null;
        }
    }
}