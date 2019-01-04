using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Renderer thisKey;  //needed to make the key invisible
    [HideInInspector]  //doesn't need to be seen in the inspector
    public bool hasPickedUp = false;  //needed to let the gamemanager know when the key has been obtained

    //public function to reset the key back to base values when called by
    //the game manager
    public void ResetKey()
    {
        thisKey.enabled = true;
        hasPickedUp = false;
    }

    //says the key has been picked up when hit by the player
    //makes the key invisible so the player assumes they have collected it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            hasPickedUp = true;
            thisKey.enabled = false;
        }
    }
}
