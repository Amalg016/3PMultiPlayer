using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminZone : MonoBehaviour
{
    public float[] RandomConstant;

    // Start is called before the first frame update

    void Start()
    {
        RandomConstant = new float[10];
        for (int i = 0; i < RandomConstant.Length; i++)
        {
            RandomConstant[i] = Random.Range(-1f, 1f);
        }

    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
