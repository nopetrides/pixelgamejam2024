using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthAffectorPool
{
    private const int _initialPoolSize = 10;
    

    private static Dictionary<Rigidbody, int> HealthAffectors = new();
    
    private static Dictionary<int, Queue<HealthAffector>> _affectorPools = new ();
        
    public static int CanCurrentlyAffectHealth(Rigidbody rb)
    {
        return HealthAffectors.TryGetValue(rb, out var affector) ? affector : 0;
    }
    
    public static void CreatePool(HealthAffector prefab, int capacity)
    {
        int poolId = prefab.GetInstanceID();
        _affectorPools[poolId] = new Queue<HealthAffector>(capacity);
        prefab.gameObject.SetActive(false);
        Rigidbody rb = prefab.GetRigidbody();
        if (!HealthAffectors.ContainsKey(rb))
        {
            HealthAffectors.Add(rb, prefab.GetHealthChangeValue());
        }
        for (int i = 0; i < capacity; i++)
        {
            CreateObjectInPool(poolId, prefab);
        }
    }

    //this is the method that is called above to instantiate each prefab in the pool
    private static HealthAffector CreateObjectInPool(int poolId, HealthAffector prefab)
    {
        var clone = GameObject.Instantiate(prefab);
        clone.PoolID = poolId;
        _affectorPools[poolId].Enqueue(clone);
        return clone;
    }

    //this method is the one called to "recycle" the prefabs, reactivating it with the desired parameters
    public static HealthAffector GetNext(HealthAffector prefab, Vector3 pos, Quaternion rot, bool setActive = true)
    {
        var clone = GetNext(prefab);
        clone.transform.SetPositionAndRotation(pos, rot);
        clone.gameObject.SetActive(setActive);
        return clone;
    }

    //this method is called above to iterate over the pool to activate the next in the queue or create another prefab in the pool if none are available to "reuse"
    private static HealthAffector GetNext(HealthAffector prefab)
    {
        int poolId = prefab.GetInstanceID();
        if (!_affectorPools.ContainsKey(poolId))
        {
            CreatePool(prefab, _initialPoolSize);
        }

        var pool = _affectorPools[poolId];
        for (int i = 0; i < pool.Count; i++)
        {
            var item = pool.Dequeue();
            item.PoolID = poolId;
            if (item.gameObject.activeInHierarchy == false)
            {
                return item;
            }
        }

        return CreateObjectInPool(poolId, prefab);
    }

    //this method re-adds the prefab to the queue
    public static void ReAddObjectToPool(int poolId, HealthAffector clone)
    {
        _affectorPools[poolId].Enqueue(clone);
    }
}
