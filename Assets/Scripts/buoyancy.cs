using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buoyancy : MonoBehaviour
{
    public float UpwardForce = 12.72f; // 9.81 is the opposite of the default gravity, which is 9.81. If we want the boat not to behave like a submarine the upward force has to be higher than the gravity in order to push the boat to the surface
    private bool isInWater = false;
    public  Rigidbody rb;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider collidier)
    {
        if (collidier.gameObject.CompareTag("Water"))
        {
            isInWater = true;
            rb.drag = 5f;
        }
    }

    void OnTriggerExit(Collider collidier)
    {
        if (collidier.gameObject.CompareTag("Water"))
        {
            isInWater = false;
            rb.drag = 0.05f;

        }
    }

    void FixedUpdate()
    {
        if (isInWater)
        {
            // apply upward force
            Vector3 force = transform.up * UpwardForce;
            this.rb.AddRelativeForce(force, ForceMode.Acceleration);
            //Debug.Log("Upward force: " + force + " @" + Time.time);
        }
    }
}
