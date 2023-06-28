using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] Transform ZoneBound;
    [SerializeField] Transform player;
    [SerializeField] float InitialRadius, RadiusNow, Dist2;
    double InitialTime;
    [SerializeField] float ShrinkPower = 0.1f;
    [SerializeField] float MoveSpeed = 3f;
    private Vector3 NextCentre;
    public float[] TheRandomConstant;

    private bool FirstRoundStarted, SecondRoundStarted, ThirdRoundStarted,FourthRoundStarted;
    // Start is called before the first frame update
    void Start()
    {
        InitialRadius = HorizontalDistance(ZoneBound, transform);
        NextCentre = transform.position;
       // Dist2 = Vector3.Distance(player.position, transform.position);
        InitialTime = Time.time;
       
       // TheRandomConstant = new float[10];
        //ivde ee random constant il ni array konf idanam;
        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
        
    }

    // Update is called once per frame
    void Update()
    {
       
        RadiusNow = HorizontalDistance(ZoneBound, transform);
       // Dist2 = Vector3.Distance(player.position, transform.position);
    
        if (Time.time-InitialTime>5  & transform.localScale.x>0.75)
        {
            Shrink();
            if (!FirstRoundStarted) { MoveZone(0.75f); FirstRoundStarted = true; }
        }

        if (Time.time - InitialTime >15 & transform.localScale.x > 0.5)
        {
            Shrink();
            if (!SecondRoundStarted) { MoveZone(0.5f); SecondRoundStarted = true; }
        }
        if (Time.time - InitialTime > 25 & transform.localScale.x > 0.25)
        {
            Shrink();
            if (!ThirdRoundStarted) { MoveZone(0.25f); ThirdRoundStarted = true; }
        }
        if (Time.time - InitialTime > 35 & transform.localScale.x > 0.10)
        {
            Shrink();
            if (!FourthRoundStarted) { MoveZone(0.25f); FourthRoundStarted = true; }
        }

        transform.position = Vector3.MoveTowards(transform.position,NextCentre,Time.deltaTime*MoveSpeed);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(NextCentre, 1f);
    }
    private void MoveZone(float ShrinkRatio)
    {
        float DeltaRadius = RadiusNow - InitialRadius * ShrinkRatio;
        float xPos = transform.position.x + TheRandomConstant[0];
        float zPos = transform.position.z + TheRandomConstant[1];
        //  float xPos = Random.Range(transform.position.x - DeltaRadius, transform.position.x + DeltaRadius);
        //float zPos = Random.Range(transform.position.z - DeltaRadius, transform.position.z + DeltaRadius);
        NextCentre = new Vector3(xPos, NextCentre.y, zPos);
    }

    private void Shrink()
    {
        float ShrinkConstant = Time.deltaTime * ShrinkPower;
        transform.localScale = new Vector3(transform.localScale.x - ShrinkConstant, transform.localScale.y, transform.localScale.z - ShrinkConstant); 
    }

    private float HorizontalDistance(Transform trans1,Transform trans2) 
    {
        return Vector3.Distance(new Vector3(trans1.position.x,0, trans1.position.z), new Vector3(trans2.position.x, 0, trans2.position.z));
    }
}
