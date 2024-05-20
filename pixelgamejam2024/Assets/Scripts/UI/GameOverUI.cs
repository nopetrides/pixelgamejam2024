using UnityEngine;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.MusicTypes.GameOver, 1f);
    }
}
