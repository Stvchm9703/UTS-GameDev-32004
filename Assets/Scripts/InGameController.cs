using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InGameController : MonoBehaviour
{
    private InGameHUIManager _inGameHuiManager;

    [SerializeField] private GameObject GhostBluePrefab, GhostGreenPrefab, GhostRedPrefab, GhostYellowPrefab;

    private GameObject GhostBlueInst, GhostGreenInst, GhostRedInst, GhostYellowInst;
    // [SerializeField] private GameObject GhostBlueLabel, GhostGreenLable, GhostRedLable, GhostYellowLable;

    [SerializeField] private Transform mapTransform;

    void Start()
    {
        _inGameHuiManager = GameObject.Find("HUI").GetComponent<InGameHUIManager>();
        if (GhostBluePrefab != null)
        {
            GhostBlueInst = Instantiate(GhostBluePrefab, Vector3.zero, Quaternion.identity, mapTransform);
            GhostBlueInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            var labelCanvas = GhostBlueInst.GetComponentInChildren<Canvas>();
            labelCanvas.worldCamera = Camera.main;
            
        }

        if (GhostGreenPrefab != null)
        {
            GhostGreenInst = Instantiate(GhostGreenPrefab, mapTransform);
            GhostGreenInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            var labelCanvas = GhostGreenInst.GetComponentInChildren<Canvas>();
            labelCanvas.worldCamera = Camera.main;
        }

        if (GhostRedPrefab != null)
        {
            GhostRedInst = Instantiate(GhostRedPrefab, mapTransform);
            GhostRedInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            var labelCanvas = GhostRedInst.GetComponentInChildren<Canvas>();
            labelCanvas.worldCamera = Camera.main;
        }
        if (GhostYellowPrefab != null)
        {
            GhostYellowInst = Instantiate(GhostYellowPrefab, mapTransform);
            GhostYellowInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            var labelCanvas = GhostYellowInst.GetComponentInChildren<Canvas>();
            labelCanvas.worldCamera = Camera.main;
        }
    }
    
    
    
}