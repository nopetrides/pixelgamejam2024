using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    [SerializeField]
    float _lifeSpan = 2f;

    private WaitForSeconds _waitTime;
    
    
    protected void OnEnable()
    {
        if (_lifeSpan >= 0f)
        {
            _waitTime = new WaitForSeconds(_lifeSpan);
            StartCoroutine(DisableCoroutine());
        }
    }

    private IEnumerator DisableCoroutine()
    {
        yield return _waitTime;
        PoolSystem.Instance.DeSpawn(this);
    }
}
