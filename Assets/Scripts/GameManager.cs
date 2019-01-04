using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Designer changeable/addable items
    //Gives the gamemanager all the gameobjects that hold the different items needed for the level
    public GameObject player, key, exit, level, timer;
    public Text levelText;  //text to display what level the player is on
    public string thisSceneToLoad;  //string for the scene name to go to when time is up
    #endregion
    #region Different classes that the gamemanager needs to know about
    private LevelMaker thisLevel;  //the actual level that's been created
    private Key thisKey;  //the key that allows the player to advance to next room
    private Exit thisExit;  //the exit that the player goes into after obtaining the key to go to the next room
    private TimeManager thisTimer;  //the countdown timer that shows how much time the player has left to complete the room
    #endregion
    #region Other private variables for the gamemanager to use
    private int levelCount;  //used to display to the player what level they are on
    private int roomIndex;  //used to find a room to place the key in (which will need to be different than the exit)
    private int newRoomIndex;  //used to find a room to place the exit in (which will need to be different than the key)
    #endregion

    //At the beginning of the program, get all necessary components for the level,
    //reset the values of the indexs and locations, reset level count to 1, display the 
    //level number, and reset the timer.
    void Start ()
    {
        thisKey = key.GetComponent<Key>();
        thisExit = exit.GetComponent<Exit>();
        thisLevel = level.GetComponent<LevelMaker>();
        thisTimer = timer.GetComponent<TimeManager>();
        ResetIndexValues();
        ResetLocations();
        levelCount = 1;
        DisplayLevel();
        thisTimer.ResetTimer();
	}
	
    //Check if the key has been picked up and the player is at the exit. If so, create
    //a new level by calling reset level. If the timer is up, call loadthisscene 
    //which loads a scene dependent on what the designer says should be loaded
	void Update ()
    {
		if(thisKey.hasPickedUp && thisExit.isAtExit)
        {
            ResetLevel();
        }
        if (thisTimer.isTimeUp)
        {
            LoadThisScene();
        }
	}

    //loads a scene dependent on what the designer says should be loaded (declared in editor)
    private void LoadThisScene()
    {
        SceneManager.LoadScene(thisSceneToLoad);
    }

    //reset the roomindex value by choosing a random number from the list of rooms
    //then call other index value to reset the other index value
    private void ResetIndexValues()
    {
        roomIndex = LevelMaker.rnd.Next(1, thisLevel.RoomList.Count);
        OtherIndexValue();
    }

    //move the plyer to the beginning of the dungeon and move the key and exit
    //to where they should be according to random index they were given
    private void ResetLocations()
    {
        player.transform.position = thisLevel.RoomList[0].transform.position;
        player.transform.Translate(0f, 0f, -1);
        key.transform.position = thisLevel.RoomList[roomIndex].transform.position;
        key.transform.Translate(0f, 0f, -1f);
        exit.transform.position = thisLevel.RoomList[newRoomIndex].transform.position;
        exit.transform.Translate(0f, 0f, -1f);
    }

    //creates another random index for the room location. Checks if that number is the same
    //as the index the key is in, if so, it calls this function again and again until it 
    //isn't the same value, then returns true all the way up
    private bool OtherIndexValue()
    {
        newRoomIndex = LevelMaker.rnd.Next(1, thisLevel.RoomList.Count);
        if(roomIndex == newRoomIndex)
        {
            OtherIndexValue();
        }
        else
        {
            return true;
        }
        return true;
    }

    //Gives the leveltext object the information to display what level the player is on
    private void DisplayLevel()
    {
        levelText.text = ("Level: " + levelCount).ToString();
    }

    //after a level has been completed, it calls the different functions that each
    //component needs to reset itself. then it calls the other functions that belong
    //to the game manager to reset themselves.
    private void ResetLevel()
    {
        thisKey.ResetKey();
        thisLevel.ResetLevel();
        thisLevel.CreateLevel();
        thisTimer.ResetTimer();
        ResetIndexValues();
        ResetLocations();
        levelCount++;
        DisplayLevel();
    }
}
