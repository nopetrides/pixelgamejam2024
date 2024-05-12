using UnityEngine;

public class CharacterTester : MonoBehaviour
{
    [SerializeField] private PlayerCharacterSO _characterDataToTest;

    [SerializeField] private PlayerController _characterController;

    private void Awake()
    {
        _characterController.SetAsCharacter(_characterDataToTest);
    }
}
