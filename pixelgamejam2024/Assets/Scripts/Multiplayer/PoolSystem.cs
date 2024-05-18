using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolSystem : MonoBehaviour
{
    [SerializeField]
    private List<PoolableObject> _poolableObjects;

    [SerializeField]
    private List<string> _pooledObjectTypes;
    
    
    private ObjectPool<PoolableObject> _pool;
    
    private ConcurrentDictionary<string, ObjectPool<PoolableObject>> _pools = new();

    private ConcurrentBag<SpawnData> _objectsToAddToPool = new();

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
        // Ensure pools are ready
        foreach (var objType in _pooledObjectTypes)
        {
            if(!_pools.ContainsKey(objType)) CreatePool(objType);
        }
    }

    private void Update()
    {
        // only modify the pool on the main thread
        while (!_objectsToAddToPool.IsEmpty)
        {
            if (_objectsToAddToPool.TryTake(out var spawnData))
            {
                if (_pools.TryGetValue(spawnData.ObjectType, out var pool))
                {
                    var pooledObject = pool.Get();
                    pooledObject.transform.position = spawnData.Location;
                    pooledObject.DataSetup(spawnData.Location);
                }
            }
        }
    }

    public void CreatePool(string objectType)
    {
        Debug.Log($"Creating start");
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
        
        _pools.TryAdd(objectType, pool);
        Debug.Log($"Creating end");
    }

    public void Spawn(string objectType, Vector3 location)
    {
        Debug.Log($"Add to spawn queue");
        _objectsToAddToPool.Add(new SpawnData{ObjectType = objectType, Location = location});
    }
    
    public void DeSpawn(string objectType, PoolableObject obj)
    {
        if (_pools.TryGetValue(objectType, out var pool))
        {
            pool.Release(obj);
        }
    }

    private class SpawnData
    {
        public string ObjectType;
        public Vector3 Location;
    }
}
