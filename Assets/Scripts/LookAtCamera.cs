using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (Camera.main == null) {
            return;
        }
        
        Vector3 lookPos = transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 up = Camera.main.transform.rotation * Vector3.up;
        transform.LookAt(lookPos, up);
    }
}