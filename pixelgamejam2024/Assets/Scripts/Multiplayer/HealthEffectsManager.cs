using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEffectsManager : MonoBehaviour
{
    public static HealthEffectsManager Instance;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    public Dictionary<Rigidbody, int> ActiveHealthAffectors;
        
    public int CanCurrentlyAffectHealth(Rigidbody rb)
    {
        return ActiveHealthAffectors.TryGetValue(rb, out var affector) ? affector : 0;
    }
}
