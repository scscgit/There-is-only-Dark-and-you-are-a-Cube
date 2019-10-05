using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
       _offset = new Vector3(0, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x + _offset.x, _offset.y, playerTransform.position.z + _offset.z);
    }
}
