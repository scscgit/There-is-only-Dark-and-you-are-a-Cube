using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - playerTransform.position;
    }

    void Update()
    {
        transform.position = new Vector3(
            playerTransform.position.x + _offset.x, _offset.y, playerTransform.position.z + _offset.z);
    }
}
