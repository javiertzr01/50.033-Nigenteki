using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummaryManager : NetworkBehaviour
{
    public UnityEvent mainMenu;

    public List<PlayerStats> red = new List<PlayerStats>();
    public List<PlayerStats> blue = new List<PlayerStats>();

    public void GameEnd(int gameWinLoss)
    {
        Debug.Log("Display and Update Match Summary");
        UpdateSummary(gameWinLoss);
    }

    public void ProcessPlayer(ulong id, int team, int killCount, int deathCount)
    {
        PlayerStats cur = new PlayerStats(id, team, killCount, deathCount);
        // cur.sprite = playerSpawner.charSpriteMap[id];
        if (team == 0)
        {
            red.Add(cur);
        }
        if (team == 1)
        {
            blue.Add(cur);
        }
    }

    public void GetTeams()
    {
        foreach (var id in red)
        {
            Debug.Log($"Got red team name: \n {id.name}");
        }
        foreach (var id in blue)
        {
            Debug.Log($"Got blue team name: \n {id.name}");
        }
    }

    public void Start()
    {

    }

    public void PopulateList(List<PlayerStats> playerList, int teamId)
    {
        while (playerList.Count < 3)
        {
            PlayerStats stats = new PlayerStats();
            stats.teamId = teamId;
            stats.name = "Null";
            playerList.Add(stats);
        }
    }

    private void UpdateSummary(int gameWinLoss)
    {
        PopulateList(blue, 1);
        PopulateList(red, 0);

        Text[] texts = GameObject.FindObjectsOfType<Text>();
        Image[] images = GameObject.FindObjectsOfType<Image>();

        foreach(Text text in texts)
        {
            if (text.name == "Winner")
            {
                switch (gameWinLoss)
                {
                    case 0:
                        text.text = "Game Tied";
                        break;
                    case 1:
                        text.text = "Red Wins!";
                        break;
                    case 2:
                        text.text = "Blue Wins!";
                        break;
                    default:
                        text.text = $"Value Invoked by GameWinLoss = {gameWinLoss}";
                        break;
                }
            }
            if (text.name == "Kills")
            {
                string team = text.transform.parent.parent.name;
                string player = text.transform.parent.name;
                if (team == "Team A")
                {
                    AssignText(text, player, blue, "kills");
                }
                if (team == "Team B")
                {
                    AssignText(text, player, red, "kills");
                }
            }     
            if (text.name == "Deaths")
            {
                string team = text.transform.parent.parent.name;
                string player = text.transform.parent.name;
                if (team == "Team A")
                {
                    AssignText(text, player, blue, "deaths");
                }
                if (team == "Team B")
                {
                    AssignText(text, player, red, "deaths");
                }
            }    
            if (text.name == "Name")       
            {
                string team = text.transform.parent.parent.parent.name;     // parent.parent.parent.name = team   
                string player = text.transform.parent.parent.name;          // parent.parent.name = player 
                if (team == "Team A")    
                {
                    AssignText(text, player, blue, "names");  
                }
                if (team == "Team B")       
                {
                    AssignText(text, player, red, "names");
                }
            }      
        }

        // foreach (Image image in images)
        // {
        //     if (image.name == "Image" && image.transform.parent.name == "PlayerContainer") // have to check which player
        //     {
        //         // TODO: image.sprite = defenderBlue;
        //     }    
        // }
    }

    private void AssignText(Text text, string player, string player1, string player2, string player3)
    {
        switch (player)
        {
            case "Player1":
                text.text = player1;
                break;
            case "Player2":
                text.text = player2;
                break;
            case "Player3":
                text.text = player3;
                break;
            default:
            Debug.Log("No such UI Element");
            break;
        }
    }
    
    private void AssignText(Text text, string player, int player1, int player2, int player3)
    {
        AssignText(text, player, player1.ToString(), player2.ToString(), player3.ToString());
    }

    private void AssignText(Text text, string player, List<PlayerStats> playerStats, string type)
    {
        if (type == "kills")
        {
            AssignText(text, player, playerStats[0].kills, playerStats[1].kills, playerStats[2].kills);
        }
        else if (type == "deaths")
        {
            AssignText(text, player, playerStats[0].deaths, playerStats[1].deaths, playerStats[2].deaths);
        }
        else if (type == "names")
        {
            AssignText(text, player, playerStats[0].name, playerStats[1].name, playerStats[2].name);
        }
    }

    public void MainMenu()
    {
        mainMenu.Invoke();
    }
}

public enum KD
{
    kills = 0,
    deaths = 1
}

public struct PlayerStats
{
    public ulong clientId;
    public int teamId;  // 0 - Red, 1 - Blue
    public int kills;
    public int deaths;
    public string name;
    public Sprite sprite;

    public PlayerStats(ulong id, int team, int killCount, int deathCount)
    {
        this.clientId = id;
        this.teamId = team;
        this.name = "Player " + clientId.ToString();
        this.kills = killCount;
        this.deaths = deathCount;
        this.sprite = null;
    }
}