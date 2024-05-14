using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Slider[] sliderTests;
    
    [SerializeField] private DragonNetworkController dragon; // temp;

    private void Start()
    {
        // temp
        dragon.OnDragonDataRefresh += DragonStatusRefresh;
    }

    private void DragonStatusRefresh(Dictionary<string,int> dragonData)
    {
        int i = 0;
        foreach (var stat in dragonData.Values)
        {
            sliderTests[i].value = stat / 60000f;
            i++;
        }
    }
}
