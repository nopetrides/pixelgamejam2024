using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectorInstantiate : MonoBehaviour
{
    [SerializeField]
    private HealthAffector _healthAffector;

    public void CreateInstance()
    {
        HealthAffectorPool.Instance.GetNext(_healthAffector, new Vector3(0, 0, 0), Quaternion.identity);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CreateInstance();
    }

    
}
