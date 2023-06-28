using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] Transform KillFeed;
    [SerializeField] GameObject killFeedItem;
    [SerializeField] GameObject leaveRoom;
    [SerializeField] GameObject scoreboardItemPrefab;
    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();
    GameObject[] scoreboard;
    public TMP_Text _Text;
    int current;
    [SerializeField] Transform ShipPoint;
    GameObject Ship;

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
            player.SetDeaths(0);
            player.SetKills(0);
        }
      
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        if (PhotonNetwork.IsMasterClient == true)
        {
            PhotonNetwork.InstantiateRoomObject(Path.Combine ("PhotonPrefabs", "Ship1"), ShipPoint.position, ShipPoint.rotation, 0);
        }
    }
    private void Awake()
    {
        Application.targetFrameRate=60;
        ShipPoint = GameObject.FindGameObjectWithTag("ShipPoint").transform;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }
    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public void Pause()
    {
      
        container.gameObject.SetActive(!container.gameObject.activeSelf);
        leaveRoom.SetActive(!leaveRoom.activeSelf);
        if (container.gameObject.activeSelf)
        {
          // scoreboard= container.GetComponentsInChildren<ScoreboardItem>();
            scoreboard = GameObject.FindGameObjectsWithTag("scoreboard");
            //int MaxValue;
            foreach (GameObject score in scoreboard)
            {
                score.GetComponent<ScoreboardItem>().AddKills();
                score.GetComponent<ScoreboardItem>().AddDeaths();
               // string kills = score.GetComponent<ScoreboardItem>().killsText.text;
                //string deaths = score.GetComponent<ScoreboardItem>().deathsText.text;               
                //MaxValue=System.Convert.ToInt32(kills);
            }                       
        }
    }
    public void InstantiateKillFeed(Player killer,Player victim)
    {
        string Killer = killer.NickName.ToString();
        string Victim = victim.NickName.ToString();
        killFeedItem killfeeds = Instantiate(killFeedItem, KillFeed).GetComponent<killFeedItem>();
        killfeeds.SetUp(Killer,Victim);
    }
   public void LeaveMatch()
    {
        StartCoroutine(LeaveAndLoad());
    }
    IEnumerator LeaveAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
          yield return null;      
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(3);
    }
    private void Update()
    {
        
          current = (int)(1f / Time.unscaledDeltaTime);
        _Text.text = current.ToString() + " FPS";
        if (container.gameObject.activeSelf == false)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
            }
        }
        else if (container.gameObject.activeSelf == true)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
