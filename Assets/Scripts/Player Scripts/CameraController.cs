using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
public class CameraController : MonoBehaviour
{

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private GameObject player;
    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {

    }

    private void Update()
    {


    }

    public void FollowPlayer()
    {

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<PlayerController>().IsOwner && p.GetComponent<PlayerController>().IsClient)
            {
                player = p;   
            }
        }

        cinemachineVirtualCamera.Follow = player.transform;
    }

}
