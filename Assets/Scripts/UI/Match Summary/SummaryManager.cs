using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummaryManager : MonoBehaviour
{
    ulong[] blueID;   // Blue Team IDs
    ulong[] redID;    // Red Team IDs
    string[] blueNames = {"shadowlord", "mysticguardian", "pimpmaster"};
    string[] redNames = {"thelegend27", "missmagus", "ladyNagant"};
    int[] bp1kd = {1,6};    // bp1kd = Blue Player1 KD
    int[] bp2kd = {2,5};
    int[] bp3kd = {3,4};
    int[] rp1kd = {4,3};    // rp1kd = Red Player1 KD
    int[] rp2kd = {5,2};
    int[] rp3kd = {6,1};
    
    

    // Start is called before the first frame update
    void Start()
    {
        // Getting all the sprites
        Sprite defenderBlue = Resources.Load<Sprite>("defender_blue");
        Sprite defenderRed = Resources.Load<Sprite>("defender_red");
        Sprite guardianBlue = Resources.Load<Sprite>("guardian_blue");
        Sprite guardianRed = Resources.Load<Sprite>("guardian_red");

        // Some code to get the information from network here

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
                    AssignText(text, player, bp1kd[0], bp2kd[0], bp3kd[0]);
                }
                if (team == "Team B")
                {
                    AssignText(text, player, rp1kd[0], rp2kd[0], rp3kd[0]);
                }
            }     
            if (text.name == "Deaths")
            {
                string team = text.transform.parent.parent.name;
                string player = text.transform.parent.name;
                if (team == "Team A")
                {
                    AssignText(text, player, bp1kd[1], bp2kd[1], bp3kd[1]);
                }
                if (team == "Team B")
                {
                    AssignText(text, player, rp1kd[1], rp2kd[1], rp3kd[1]);
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
                image.sprite = defenderBlue;
            }    
        }
    }

    private void AssignText(Text text, string team, string player1, string player2, string player3)
    {
        switch (team)
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
    
    private void AssignText(Text text, string team, int player1, int player2, int player3)
    {
        AssignText(text, team, player1.ToString(), player2.ToString(), player3.ToString());
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
