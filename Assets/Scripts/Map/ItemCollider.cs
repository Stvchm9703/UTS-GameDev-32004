using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCollider : MonoBehaviour
{
    // BoxCollider2D boxCollider;
    public Waypoint waypoint { get; private set; }

    public int itemType
    {
        get => waypoint.gridType;
    }

    // public int itemType = 0;
    public CherryController cherryController;

    void Start()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // this.gameObject.layer = LayerMask.NameToLayer("Item");
        var rigidBody = this.AddComponent<Rigidbody2D>();
        rigidBody.isKinematic = false;
        // rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rigidBody.gravityScale = 0;
        rigidBody.useAutoMass = false;
        rigidBody.bodyType = RigidbodyType2D.Static;
        
        var  boxCollider = this.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.includeLayers = LayerMask.GetMask("Default");
        boxCollider.layerOverridePriority = 1;


    }

    public void SetupFromCherryController(Waypoint wp, CherryController cherryController)
    {
        this.waypoint = wp;
        this.cherryController = cherryController;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
             this.cherryController.OnItemHit(this);
        }
    }
    

}