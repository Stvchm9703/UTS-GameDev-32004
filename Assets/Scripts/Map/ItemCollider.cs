using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemCollider : MonoBehaviour
{
    // BoxCollider2D boxCollider;
    public Waypoint waypoint;
    public UnityEvent<ItemCollider, GameObject> emmitItemHitEvent = new UnityEvent<ItemCollider, GameObject>();

    public int itemType
    {
        get => waypoint.gridType;
    }

    void Start()
    {
        this.gameObject.tag = "item";
        var rigidBody = this.AddComponent<Rigidbody2D>();
        rigidBody.isKinematic = false;
        rigidBody.gravityScale = 0;
        rigidBody.useAutoMass = false;
        rigidBody.bodyType = RigidbodyType2D.Static;

        var boxCollider = this.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.includeLayers = LayerMask.GetMask("Default");
        boxCollider.layerOverridePriority = 1;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            emmitItemHitEvent.Invoke(this, other.gameObject);
        }
    }
}