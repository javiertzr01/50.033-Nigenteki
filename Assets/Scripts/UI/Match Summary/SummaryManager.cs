using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummaryManager : MonoBehaviour
{
    ulong[] blueId;   // Blue Team IDs
    ulong[] redId;    // Red Team IDs
    string[] blueNames = {"shadowlord", "mysticguardian", "pimpmaster"};
    string[] redNames = {"thelegend27", "missmagus", "ladyNagant"};
    int[,] blueKD;
    int[,] redKD;
    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < blueId.Length; i++)
        {
            // Setting Name
            blueNames[i] = "Player " + blueId[i].ToString();

            // Setting KD
            int[] kd = NetworkManager.Singleton.ConnectedClients[blueId[i]].PlayerObject.GetComponent<PlayerController>().KDStats.Value;
            for (int j = 0; j < kd.Length; j++)
            {
                blueKD[i,j] = kd[j];
            }
        }
        for (int i = 0; i < redId.Length; i++)
        {
            redNames[i] = "Player " + redId[i].ToString();

            int[] kd = NetworkManager.Singleton.ConnectedClients[redId[i]].PlayerObject.GetComponent<PlayerController>().KDStats.Value;
            for (int j = 0; j < kd.Length; j++)
            {
                redKD[i,j] = kd[j];
            }
        }

        Text[] texts = GameObject.FindObjectsOfType<Text>();
        Image[] images = GameObject.FindObjectsOfType<Image>();

        foreach(Text text in texts)
        {
            if (text.name == "Winner")
            {
                string winner = "Red";
                // TODO: Add this
                text.text = winner + " Wins!";
            }
            if (text.name == "Kills")
            {
                string team = text.transform.parent.parent.name;
                string player = text.transform.parent.name;
                if (team == "Team A")
                {
                    AssignText(text, player, blueKD, KD.kills);
                }
                if (team == "Team B")
                {
                    AssignText(text, player, redKD, KD.kills);
                }
            }     
            if (text.name == "Deaths")
            {
                string team = text.transform.parent.parent.name;
                string player = text.transform.parent.name;
                if (team == "Team A")
                {
                    AssignText(text, player, blueKD, KD.deaths);
                }
                if (team == "Team B")
                {
                    AssignText(text, player, redKD, KD.deaths);
                }
            }    
            if (text.name == "Name")       
            {
                string team = text.transform.parent.parent.parent.name;     // parent.parent.parent.name = team   
                string player = text.transform.parent.parent.name;          // parent.parent.name = player 
                if (team == "Team A")    
                {
                    AssignText(text, player, blueNames[0], blueNames[1], blueNames[2]);    
                }
                if (team == "Team B")       
                {
                    AssignText(text, player, redNames[0], redNames[1], redNames[2]);
                }
            }      
        }

        foreach (Image image in images)
        {
            if (image.name == "Image" && image.transform.parent.name == "PlayerContainer") // have to check which player
            {
                // TODO: image.sprite = defenderBlue;
            }    
        }
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

    private void AssignText(Text text, string player, int[,] kd, KD type)
    {
        AssignText(text, player, kd[0, (int) type], kd[1, (int) type], kd[2, (int) type]);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

public enum KD
{
    kills = 0,
    deaths = 1
}