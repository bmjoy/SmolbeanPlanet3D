using System;
using UnityEngine;

public enum BuildingWearPattern { Circle, Rectangle }

public abstract class SmolbeanBuilding : MonoBehaviour
{
    public int PrefabIndex {get; set;}
    public BuildingWearPattern wearPattern = BuildingWearPattern.Rectangle;
    public Vector2 wearScale = Vector2.one;
    public GameObject building;
    public float itemDropRadius = 2f;
    public float itemDropHeight = 1f;

    public Inventory Inventory { get; private set; }

    public abstract Vector3 GetSpawnPoint();
    public abstract Vector3 GetDropPoint();

    protected virtual void Awake()
    {
        Inventory = new Inventory();
    }

    protected virtual void Start()
    {
        InvokeRepeating("RegisterWear", 0.0f, 0.5f);
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void OnDestroy()
    {
        CancelInvoke("RegisterWear");

        while(!Inventory.IsEmpty())
        {
            var item = Inventory.DropLast();

            Vector3 upPos = Vector3.up * itemDropHeight;
            Vector3 outPos = Vector3.left * UnityEngine.Random.Range(0f, itemDropRadius);
            outPos = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * outPos;

            DropController.Instance.Drop(item.dropSpec, transform.position + upPos + outPos, item.quantity);
        }

        DeliveryManager.Instance.BuildingDestroyed(this);
    }

    private void RegisterWear()
    {
        GroundWearManager.Instance.BuildingOn(building.GetComponent<Renderer>().bounds, wearPattern, wearScale);
    }

}
