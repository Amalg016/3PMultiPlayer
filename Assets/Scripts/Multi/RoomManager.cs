using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
            DontDestroyOnLoad(gameObject);
            Instance = this;        
    }

     void OnEnable()
    { 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
     void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1|| scene.buildIndex == 2)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
    
}
