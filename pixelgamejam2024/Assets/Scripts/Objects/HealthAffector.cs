using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HealthAffector : MonoBehaviour
{
    [SerializeField]
    private HealthAffectorSO _healthAffectorSo;
    
    private Rigidbody _rb;
    private int _healthChangeValue;
    
    [Tooltip("< 0 means infinite LifeSpan")]
    [SerializeField]
    protected float _lifeSpan = -1f;

    public int PoolID;
    protected WaitForSeconds _waitTime;
    protected Coroutine _disabler;

    private void Awake()
    {
        //_rb = GetComponent<Rigidbody>();
        //_healthChangeValue = _healthAffectorSo.HealthChangeValue;
        //HealthEffectsManager.Instance.CreatePool(_rb, _healthChangeValue, 1);
    }

    public void Setup()
    {
        _rb = GetComponent<Rigidbody>();
        _healthChangeValue = _healthAffectorSo.HealthChangeValue;
    }

    public Rigidbody GetRigidbody()
    {
        return _rb;
    }

    public int GetHealthChangeValue()
    {
        return _healthChangeValue;
    }
    
    protected void OnEnable()
    {
        if (_lifeSpan >= 0f)
        {
            _waitTime = new WaitForSeconds(_lifeSpan);
            _disabler = StartCoroutine(DisableCoroutine());
        }
    }
    
    protected void OnDisable()
    {
        HealthAffectorPool.Instance.ReAddObjectToPool(PoolID, this);
    }

    private IEnumerator DisableCoroutine()
    {
        yield return _waitTime;
        DisablePoolableObject();
    }

    private void DisablePoolableObject()
    {
        ClearCoroutine();
        gameObject.SetActive(false);
    }

    private void ClearCoroutine()
    {
        if(_disabler == null) return;
        
        StopCoroutine(_disabler);
        _disabler = null;
    }
}
