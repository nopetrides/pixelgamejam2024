using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] 
    bool freezeXZAxis = true;

    [SerializeField]
    private Camera _cam;

    

    void FixedUpdate()
    {
        if (_cam == null)
            _cam = Camera.main;
        if (_cam == null) return;
        
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
