using UnityEngine;

public class TreasurePickupObject : PoolableObject
{
    //setup the pulled treasure with the data from the SO
    [SerializeField] 
    private TreasureTypesSO _treasureData;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private int _weight;

    // used as unique key
    private Vector3 _originalCoordinates;

    public override void DataSetup(Vector3 coordinates)
    {
        _originalCoordinates = coordinates;
        _treasureData = TreasureManager.Instance.GetTreasureDataFromCoordinates(coordinates);
        _weight = _treasureData.Weight;
        _name = _treasureData.Type;
        _spriteRenderer.sprite = _treasureData.Sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        // This was caused by the players trigger (at least when it is first picked up)
        // todo inform the treasure manager, who then informs the player.
        // let the treasure manager handle returning the treasure to the pool - will also tell network players this treasure was picked up.
        // let the player handle adding weight to their inventory (if they can pick it up)
        // TODO handle dropping in the pool

        if (TreasureManager.Instance.AddTreasureToPlayer(_weight, transform.position))
        {
            PoolSystem.Instance.DeSpawn("Treasure", this);
            TreasureManager.Instance.OnTreasurePickedUp(_originalCoordinates);
        }
    }
}
