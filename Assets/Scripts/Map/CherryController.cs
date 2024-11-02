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

    bool isReady = false;
    // [SerializeField] bool debug = false;

    [SerializeField] int maxPowerCount = 0;
    [SerializeField] GameObject cherryPrefab;

    List<Waypoint> AvailableWaypoints;
    GameObject levelGenGameObj;

    LevelGenerator levelGenerator;

    // MapRender mapRender;
    Queue<IEnumerator> SpawnerQueue;

    private List<ItemCollider> itemColliders => levelGenerator.itemColliders;

    void Start()
    {
        SpawnerQueue = new Queue<IEnumerator>();

        InitItemColliders();
        StartCoroutine(ProcessSpawner());
    }


    void InitItemColliders()
    {
        levelGenGameObj = GameObject.Find("Grid");
        levelGenerator = levelGenGameObj.GetComponent<LevelGenerator>();

        if (levelGenerator != null && maxPowerCount <= 0)
        {
            maxPowerCount = levelGenerator.Waypoints.Count(wp => wp.gridType == 6);
        }

        this.isReady = true;
    }

    GameObject CreateCherry(Waypoint wp)
    {
        var itemCollider = new GameObject("wp");
        itemCollider.transform.SetParent(this.transform);
        itemCollider.transform.localPosition = wp.position;
        itemCollider.transform.localScale = new Vector3(3, 3, 3);
        
        var itmCol = itemCollider.AddComponent<ItemCollider>();
        // itmCol.SetupFromCherryController(wp, this);
        itmCol.emmitItemHitEvent.AddListener(OnItemHitCherry);
        itemColliders.Add(itmCol);
        return itemCollider;
    }

    public void OnItemHitCherry(ItemCollider itemCollider, GameObject hitted)
    {
        // Destory process
        this.AvailableWaypoints.Add(itemCollider.waypoint);
        this.itemColliders.Remove(itemCollider);
        if (itemCollider.itemType == 6)
        {
            this.SpawnerQueue.Enqueue(RespwanCherry());
        }

        levelGenerator.RemoveItemTile(itemCollider.waypoint);
        Destroy(itemCollider.gameObject);
    }

    IEnumerator RespwanCherry()
    {
        yield return new WaitForSeconds(RespwanTime);
        // Debug.Log($"AvailableWaypoints : {AvailableWaypoints.Count}");
        int randomIndex = Random.Range(0, AvailableWaypoints.Count);
        var randomWp = AvailableWaypoints[randomIndex];
        randomWp.gridType = 8;
        CreateCherry(randomWp);
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