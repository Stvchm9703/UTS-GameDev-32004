using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommonSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var labelCanvas = GetComponentInChildren<Canvas>();
        labelCanvas.worldCamera = Camera.main;
    }

    
}
