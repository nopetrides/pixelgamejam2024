using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{

    private string _coordinate = "";


    private async void OnEnable()
    {
        StartCoroutine(AsyncLoad()); ;
        // _coordinate = transform.position.ToString("F2");
        // TreasureManager.Instance.AskServerIfTreasureSpawnerWasAlreadyFound(_coordinate);
    }

    private IEnumerator AsyncLoad()
    {
        yield return new WaitForSeconds(0.01f);
        _coordinate = transform.position.ToString("F2");
        TreasureManager.Instance.AskServerIfTreasureSpawnerWasAlreadyFound(_coordinate);
    }
}
