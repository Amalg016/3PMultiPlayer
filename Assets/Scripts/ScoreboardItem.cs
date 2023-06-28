using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;
    Player Player1;
   // PlayerManager playerManager;
   // PhotonView PV;
    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
        Player1 = player;
    }  
    public void AddDeaths()
    {
        deathsText.text = Player1.GetDeaths().ToString("00");     
    }
    public void AddKills()
    {
        killsText.text = Player1.GetKills().ToString("00");
    }
             
}
