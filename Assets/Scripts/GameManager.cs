using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player, key, exit, level, timer;
    public Text levelText;
    public string thisSceneToLoad;
    private LevelMaker thisLevel;
    private Key thisKey;
    private Exit thisExit;
    private TimeManager thisTimer;
    private int levelCount;
    private int roomIndex;
    private int newRoomIndex;

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

    private void LoadThisScene()
    {
        SceneManager.LoadScene(thisSceneToLoad);
    }

    private void ResetIndexValues()
    {
        roomIndex = LevelMaker.rnd.Next(1, thisLevel.RoomList.Count);
        OtherIndexValue();
    }

    private void ResetLocations()
    {
        player.transform.position = thisLevel.RoomList[0].transform.position;
        player.transform.Translate(0f, 0f, -1);
        key.transform.position = thisLevel.RoomList[roomIndex].transform.position;
        key.transform.Translate(0f, 0f, -1f);
        exit.transform.position = thisLevel.RoomList[newRoomIndex].transform.position;
        exit.transform.Translate(0f, 0f, -1f);
    }

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

    private void DisplayLevel()
    {
        levelText.text = ("Level: " + levelCount).ToString();
    }

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
