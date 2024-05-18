using System.Collections;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{

    private string _coordinate = "";


    private async void OnEnable()
    {
        StartCoroutine(AsyncLoad());
        // _coordinate = transform.position.ToString("F2");
        // TreasureManager.Instance.AskServerIfTreasureSpawnerWasAlreadyFound(_coordinate);
    }

    private IEnumerator AsyncLoad()
    {
        yield return null;
        yield return null;
        Debug.Log("Treasure spawner ask server if ready");
        
        TreasureManager.Instance.AskServerIfTreasureSpawnerWasAlreadyFound(transform.position);
    }
}
