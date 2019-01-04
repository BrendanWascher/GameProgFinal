using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [HideInInspector]
    public bool isAtExit = false;  //needed to let the gamemanager know when player is at the exit

    //When the player is at the exit, say so
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isAtExit = true;
        }
    }

    //when the player is no longer at the exit, say so
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isAtExit = false;
        }
    }
}
