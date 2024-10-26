using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    private InGameHUIManager _inGameHuiManager;


    void Start()
    {
        _inGameHuiManager = GameObject.Find("HUI").GetComponent<InGameHUIManager>();
    }
    
}