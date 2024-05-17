using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PoolSystem : MonoBehaviour
{
    [SerializeField]
    private List<PoolableObject> _poolableObjects;

    [SerializeField]
    private List<string> _pooledObjectTypes;
    
    
    private ObjectPool<PoolableObject> _pool;
    
    private Dictionary<string, ObjectPool<PoolableObject>> _pools = new();

    [SerializeField]
    private int _initialSize = 10;
    [SerializeField]
    private int _maxSize = 20;
    
    public static PoolSystem Instance;
    
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

    public void CreatePool(string objectType)
    {
        var index = _pooledObjectTypes.IndexOf(objectType);
        var obj = _poolableObjects[index];
        
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

    public GameObject Spawn(string objectType , Vector3 location, string data )
    {
        if(!_pools.ContainsKey(objectType)) CreatePool(objectType);
        if (_pools.TryGetValue(objectType, out var pool))
        {
            var pooledObject = pool.Get();
            pooledObject.transform.position = location;
            pooledObject.DataSetup(data);
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
}
