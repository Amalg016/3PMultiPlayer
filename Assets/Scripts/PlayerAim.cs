using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    Camera cam;
    [SerializeField] GameObject[] items;
    [SerializeField] GameObject[] scopes;
    [SerializeField] GameObject scope;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Aim();
        }
        else
        {
            cam.fieldOfView = 60f;
            scopes[1].SetActive(false);
            scopes[0].SetActive(false);
            scope.SetActive(true);
        }
    }
    void Aim()
    {
       
        if (items[1].gameObject.activeSelf)
        {
            cam.fieldOfView = 10f;
            scopes[1].SetActive(true);
            scope.SetActive(false);
        }
        else if (items[0].gameObject.activeSelf)
        {
            cam.fieldOfView = 40f;
            scopes[0].SetActive(true);
            scope.SetActive(false);
        }
        else
        {
            cam.fieldOfView = 60f;
            scopes[1].SetActive(false);
            scopes[0].SetActive(false);
            scope.SetActive(true);
        }
    }
}
