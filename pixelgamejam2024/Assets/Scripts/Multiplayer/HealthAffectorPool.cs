using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAffectorPool : MonoBehaviour
{
    private const int _initialPoolSize = 5;

    private static Dictionary<Rigidbody, int> HealthAffectors = new();
    
    private static Dictionary<int, Queue<HealthAffector>> _affectorPools = new ();
    public static HealthAffectorPool Instance;
    
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
        
    public int CanCurrentlyAffectHealth(Rigidbody rb)
    {
        return HealthAffectors.TryGetValue(rb, out var affector) ? affector : 0;
    }
    
    public void CreatePool(HealthAffector prefab, int capacity)
    {
        int poolId = prefab.GetInstanceID();
        _affectorPools[poolId] = new Queue<HealthAffector>(capacity);
        
        prefab.gameObject.SetActive(false);
        for (int i = 0; i < capacity; i++)
        {
            CreateObjectInPool(poolId, prefab);
        }
        
        
        foreach (var obj in HealthAffectors)
        {
            Debug.Log($"{obj.Key} : {obj.Value}");
        }
    }

    //this is the method that is called above to instantiate each prefab in the pool
    private HealthAffector CreateObjectInPool(int poolId, HealthAffector prefab)
    {
        var clone = Instantiate(prefab);
        clone.Setup();
        Rigidbody rb = clone.GetRigidbody();
        Debug.Log($"{clone.name}");
        if (!HealthAffectors.ContainsKey(rb))
        {
            Debug.Log($"add to dic");
            HealthAffectors.Add(rb, clone.GetHealthChangeValue());
        }
        clone.PoolID = poolId;
        _affectorPools[poolId].Enqueue(clone);
        return clone;
    }

    //this method is the one called to "recycle" the prefabs, reactivating it with the desired parameters
    public HealthAffector GetNext(HealthAffector prefab, Vector3 pos, Quaternion rot, bool setActive = true)
    {
        var clone = GetNext(prefab);
        clone.transform.SetPositionAndRotation(pos, rot);
        clone.gameObject.SetActive(setActive);
        Rigidbody rb = prefab.GetRigidbody();
        Debug.Log($"{rb}");
        return clone;
    }

    //this method is called above to iterate over the pool to activate the next in the queue or create another prefab in the pool if none are available to "reuse"
    private HealthAffector GetNext(HealthAffector prefab)
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
    public void ReAddObjectToPool(int poolId, HealthAffector clone)
    {
        _affectorPools[poolId].Enqueue(clone);
    }
}
