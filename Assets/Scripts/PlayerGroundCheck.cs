using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        playerController.SetGroundedState(true);
    }
    private void OnTriggerExit(Collider other)
    {
        playerController.SetGroundedState(false);
    }
    private void OnTriggerStay(Collider other)
    {
        playerController.SetGroundedState(true);
    }
}
