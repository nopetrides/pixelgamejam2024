using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PoolSystem : MonoBehaviour
{
    [SerializeField]
    private PoolableObject _poolableObject;
    private ObjectPool<PoolableObject> _pool;

    [SerializeField]
    private int _initialSize = 10;
    [SerializeField]
    private int _maxSize = 20;
    
    public static PoolSystem Instance;

    //For debugging
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) Spawn();
    }

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

    private void Start()
    {
        _pool = new ObjectPool<PoolableObject>(() =>
        {
            return Instantiate(_poolableObject);
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
    }

    public void CreatePool(PoolableObject obj)
    {
        _poolableObject = obj;
        // Debug.Log($"{_poolableObject.name}");
        // _pool = new ObjectPool<PoolableObject>(() =>
        // {
        //     return Instantiate(_poolableObject);
        // }, pooledObject =>
        // {
        //     pooledObject.gameObject.SetActive(true);
        // }, pooledObject =>
        // {
        //     pooledObject.gameObject.SetActive(false);
        // }, pooledObject =>
        // {
        //     Destroy(pooledObject.gameObject);
        // }, true, _initialSize, _maxSize);
    }

    public void Spawn()
    {
        var pooledObject = _pool.Get();
        pooledObject.transform.position = transform.position + Random.insideUnitSphere * 10;
    }

    public void DeSpawn(PoolableObject obj)
    {
        _pool.Release(obj);
    }
}
