using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{

    private string _coordinate = "";


    private void OnEnable()
    {
        _coordinate = transform.position.ToString("F2");
        TreasureManager.Instance.AskServerIfTreasureSpawnerWasAlreadyFound(_coordinate);
        // Debug.Log($"{_coordinate} : {_coordinate.GetType()}");
        // Vector3 coords = Vector3Parser.TryParse(_coordinate, out Vector3 result) ? result : Vector3.zero;
        // Debug.Log($"{coords} : {coords.GetType()}");
    }
}
