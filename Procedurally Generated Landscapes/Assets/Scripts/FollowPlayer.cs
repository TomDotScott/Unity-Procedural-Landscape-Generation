using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    [SerializeField] private float height;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, height, player.position.z);
    }
}
