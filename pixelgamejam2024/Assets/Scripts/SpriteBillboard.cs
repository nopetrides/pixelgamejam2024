using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] 
    bool freezeXZAxis = true;

    private Camera _cam;
    
    void FixedUpdate()
    {
        if (_cam == null)
            try
            {
                _cam = Camera.main;
            }
            catch
            {
                Debug.LogWarning("Camera not yet ready");
            }

        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, _cam.transform.rotation.eulerAngles.y, 0f);

        }
        else
        {
            transform.rotation = _cam.transform.rotation;
        }
    }
}
