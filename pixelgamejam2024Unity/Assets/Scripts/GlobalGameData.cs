using UnityEngine;

public class GlobalGameData : MonoBehaviour
{
    [SerializeField] private PlayerCharactersData _playerDataOrigin;

    private PlayerCharactersData _characterDataInstance;
    
    private void Awake()
    {
        _characterDataInstance = Instantiate(_playerDataOrigin);
        DontDestroyOnLoad(gameObject);
    }
}
