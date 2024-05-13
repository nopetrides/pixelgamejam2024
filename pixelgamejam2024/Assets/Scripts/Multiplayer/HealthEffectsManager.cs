using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEffectsManager : MonoBehaviour
{
    public static HealthEffectsManager Instance;
    private const int _initialPoolSize = 10;
    private Rigidbody prefab;

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

    private Dictionary<Rigidbody, int> ActiveHealthAffectors = new();
        
    public int CanCurrentlyAffectHealth(Rigidbody rb)
    {
        return ActiveHealthAffectors.TryGetValue(rb, out var affector) ? affector : 0;
    }
    
    public void CreatePool(Rigidbody prefab, int healthChangeValue, int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            Rigidbody obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            ActiveHealthAffectors.Add(obj, healthChangeValue);
        }

        foreach (var rb in ActiveHealthAffectors)
        {
            Debug.Log($"{rb.Key} : {rb.Value}");
        }
    }
}
