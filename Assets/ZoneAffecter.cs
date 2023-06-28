using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneAffecter : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
       if(other.gameObject.CompareTag("Player"))
       {
          other.gameObject.GetComponent<PlayerController>().InZone = true;
       }
    }
    private void OnTriggerExit(Collider other)
    {
       if (other.gameObject.CompareTag("Player"))
       {
          other.gameObject.GetComponent<PlayerController>().InZone = false;
       }
    }
}
