using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.IO;
public class ScoreManager:MonoBehaviour
{
    public const string PlayerKills = "kills";
    public const string PlayerDeaths = "deaths";
}
public static class ScoreExtensions 
{
    public static void SetKills(this Player player,int newscore)
    {
        Hashtable Kills = new Hashtable();
        Kills[ScoreManager.PlayerKills] = newscore;
       player.SetCustomProperties(Kills);
    }
    public static void AddKills(this Player player)
    {
        int current = player.GetKills();
        current ++;
        Hashtable Kills = new Hashtable();
        Kills[ScoreManager.PlayerKills] = current;
        player.SetCustomProperties(Kills);
    }
    public static int GetKills(this Player player )
    {
        object Kills;
        if(player.CustomProperties.TryGetValue(ScoreManager.PlayerKills , out Kills))
        {
            return (int)Kills;
        }
        else
        {
        return 0;
        }

    }
    public static void SetDeaths(this Player player, int newscore)
    {
        Hashtable Deaths = new Hashtable();
        Deaths[ScoreManager.PlayerDeaths] = newscore;
        player.SetCustomProperties(Deaths);
    }

    public static void AddDeaths(this Player player )
    {
        int current = player.GetDeaths();
        current++;
        Hashtable Deaths = new Hashtable();
       Deaths[ScoreManager.PlayerDeaths] = current;
        player.SetCustomProperties(Deaths);
    }
    public static int GetDeaths(this Player player)
    {
        object Deaths;
       
        if(player.CustomProperties.TryGetValue(ScoreManager.PlayerDeaths,out Deaths))
        {
            Debug.Log("getting Deaths");
            Debug.Log((int)Deaths);

            return (int)Deaths;         
        }
        else
        {
        return 0;
        }

    }   
}
