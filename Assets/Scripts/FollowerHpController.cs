using UnityEngine;

public class FollowerHpController : MonoBehaviour
{
    public GameObject _followObj;

    private void LateUpdate()
    {
        if (_followObj != null) {
            transform.position = _followObj.transform.position;
        }
    }
}