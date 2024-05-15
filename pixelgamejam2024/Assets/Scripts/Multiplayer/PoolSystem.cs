using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PoolSystem : MonoBehaviour
{
    private ObjectPool<PoolableObject> _pool;
    
    private Dictionary<string, ObjectPool<PoolableObject>> _pools = new();
    
    private Dictionary<string,List<int>> _treasures = new();

    [SerializeField]
    private int _initialSize = 10;
    [SerializeField]
    private int _maxSize = 20;
    
    public static PoolSystem Instance;

    //For debugging
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) Spawn();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void CreatePool(string objectType, PoolableObject obj)
    {
        var pool = new ObjectPool<PoolableObject>(() =>
        {
            return Instantiate(obj);
        }, pooledObject =>
        {
            pooledObject.gameObject.SetActive(true);
        }, pooledObject =>
        {
            pooledObject.gameObject.SetActive(false);
        }, pooledObject =>
        {
            Destroy(pooledObject.gameObject);
        }, true, _initialSize, _maxSize);
        
        _pools.Add(objectType, pool);
    }

    public GameObject Spawn(string objectType , PoolableObject obj)
    {
        if(!_pools.ContainsKey(objectType)) CreatePool(objectType, obj);
        if (_pools.TryGetValue(objectType, out var pool))
        {
            var pooledObject = pool.Get();
            pooledObject.transform.position = transform.position + Random.insideUnitSphere * 10; //Modify, or perhaps add this logic to the instantiator
            return pooledObject.gameObject;
        }
        return null;
    }
    
    public void DeSpawn(string objectType, PoolableObject obj)
    {
        if (_pools.TryGetValue(objectType, out var pool))
        {
            pool.Release(obj);
        }
    }

    public void AddTreasure(string objectType, List<int> values)
    {
        _treasures.Add(objectType, values);
        foreach (var kvp in _treasures)
        {
            Debug.Log($"Key: {kvp.Key}, Values: {string.Join(", ", kvp.Value)}");
        }
    }
}
