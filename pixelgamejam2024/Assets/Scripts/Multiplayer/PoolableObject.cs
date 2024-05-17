using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    [SerializeField]
    private float _lifeSpan = -1f;

    [SerializeField]
    protected string _name;

    private WaitForSeconds _waitTime;
    
    
    protected virtual void OnEnable()
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
        PoolSystem.Instance.DeSpawn(_name, this);
        //gameObject.SetActive(false);
    }

    public string GetName()
    {
        return _name;
    }

    public virtual void DataSetup(string data)
    {
        
    }
}
