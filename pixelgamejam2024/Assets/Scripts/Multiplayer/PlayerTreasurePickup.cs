using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTreasurePickup : MonoBehaviour
{
    private Rigidbody _rb;
    private int _weight;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        TreasureManager.Instance.SetLocalPlayerRigidbody(_rb);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if it is a treasure in the manager dictionary, despawn for all, add it to own inventory
        //int weightChange = TreasureManager.Instance.IsTreasureInDictionary(other.)
    }
}
