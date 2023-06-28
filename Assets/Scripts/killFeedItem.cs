using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class killFeedItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }
    public void SetUp(string killer,string victim)
    {
        text.text = "<b>" + killer + "</b>" + "<i>" + "  killed  " + "</i>" + "<b>" + victim + "</b>";
    }
}
