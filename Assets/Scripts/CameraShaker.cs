using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public bool shake = false;
    public float power = 0.15f;
    public float duration = 0.2f;
    public float slowDownAmount = 2f;
    Vector3 StartPos;
    float initialDuration;
  
    private void Update()
    {
        if (shake)
        {
            if (duration > 0)
            {
                this.transform.localPosition = StartPos + Random.insideUnitSphere * power;
                duration -= Time.deltaTime * slowDownAmount;
            }
            else
            {
                shake = false;
                duration = initialDuration;
                this.transform.localPosition = StartPos;
            }
        }
    }
    public void Shake()
    {
        StartPos = this.transform.localPosition;
        initialDuration = duration;
        shake = true;
        
    }
 //   public IEnumerator Shake(float duration, float magnitude)
 //   {
 //       Vector3 orignalPosition = transform.position;
 //       float elapsed = 0f;
 //
 //       while (elapsed < duration)
 //       {
 //           float x = Random.Range(-1f, 1f) * magnitude;
 //           float y = Random.Range(-1f, 1f) * magnitude;
 //
 //           transform.position = new Vector3(x, y,0);
 //           elapsed += Time.deltaTime;
 //           yield return 0;
 //       }
 //       transform.position = orignalPosition;
 //   }//s
}
