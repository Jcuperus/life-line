using System;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform followTransform;

    private void Awake()
    {
        EventBroker.LevelReadyEvent += FindPlayer;
    }
    private void FindPlayer()
    {
        followTransform = FindObjectOfType<PlayerMovement>().transform;
    }
    private void LateUpdate()
    {
        transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);
    }
}