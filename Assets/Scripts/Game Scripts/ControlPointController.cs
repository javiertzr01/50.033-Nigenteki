using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointController : MonoBehaviour
{

    public GameStateStore gameStateStore;

    private bool isControlPointActive = false;

    // todo: have a variable to have list of players

    // Start is called before the first frame update
    void Start()
    {
        isControlPointActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // todo: If isControlPointActive==true, Check if players are still alive/not-destroyed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //todo: Check Team of Player

            // todo:


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {


        }
    }
}
