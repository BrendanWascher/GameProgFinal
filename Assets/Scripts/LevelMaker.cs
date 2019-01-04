using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaker : MonoBehaviour
{
    #region Prefabs of the objects that will spawn
    public GameObject FourDoorRoom;
    public GameObject ThreeDoorRoomUp;
    public GameObject ThreeDoorRoomDown;
    public GameObject TwoDoorRoomVer;
    public GameObject TwoDoorRoomHor;
    public GameObject OneDoorRoom;
    public GameObject DoorCap;
    #endregion
    #region Designer changeable values
    public int numOfRooms;  //changeable in editor to decide how many rooms to spawn
    public int blockSpace = 5;  //distance between middle of each room
    #endregion
    #region A static random value
    public static System.Random rnd;  //used to both find a random location in opendoorlist and what random room to spawn
    #endregion
    #region Lists for spawning rooms
    [HideInInspector]
    public List<GameObject> RoomList;  //list of all spawned rooms
    private List<Transform> OpenDoorList;  //List of Transforms about where the open doors are at in the map
    private List<GameObject> CapsOfRooms;  //List of all the caps on the rooms that were open after all rooms had been spawned
    private List<Transform> DoorsToRemove;  //List of the doors that were connected during spawning and should be removed
    #endregion
    #region Private integers to be used throughout many functions
    private int randDoor;  //random location in the opendoorlist list to spawn a room off of
    private int roomsCreated;  //how many rooms have been created (for checking when to stop spawning rooms
    private int openDoorsRemaining;  //check how many doors can still be spawned off of so we know when to stop spawning
    private int index; //top of the roomlist list to know where to either spawn a room or destroy a room
    #endregion
    //Should spawn all the rooms and make all the lists before doing anything else
    void Awake()
    {
        CreateLevel();
    }
    //public function to create level so a game manager can call it in the future
    //so it can create a new level
    public void CreateLevel()
    {
        InitializeOnStart();
        SpawnFirstRoom();
        MainLoop();
        SpawnCaps();
    }

    //initalizes values at the beginning of the level creation
    private void InitializeOnStart()
    {
        //initialize random value and other fields and lists
        rnd = new System.Random(Random.seed = System.DateTime.Now.Millisecond);
        roomsCreated = 0;
        openDoorsRemaining = 0;
        index = 0;
        RoomList = new List<GameObject>();
        CapsOfRooms = new List<GameObject>();
        OpenDoorList = new List<Transform>();
        DoorsToRemove = new List<Transform>();
    }

    //create the first room which will always be a four door room
    //that is spawned in the middle of the world
    private void SpawnFirstRoom()
    {
        RoomList.Add((GameObject)Instantiate(FourDoorRoom,
            Vector3.zero, Quaternion.identity));
        //RoomList[index].transform.localPosition = Vector3.zero;
        roomsCreated++; //add a new room
        Transform[] allElements = new Transform[9];  //create an array for all children in gameobject
        allElements = RoomList[index].GetComponentsInChildren<Transform>(); //load array with the children of new room
        foreach (Transform door in allElements)  //loope through array
        {
            if (door.tag == "North" || door.tag == "South" ||
                door.tag == "East" || door.tag == "West")  //check for transforms with directional tags
            {
                OpenDoorList.Add(door);  //add tagged object to the list
                openDoorsRemaining++;  //add that number to how many doors can be spawned off of still
            }
        }
    }

    //main loop of the starting funciton
    private void MainLoop()
    {
        for (; roomsCreated < numOfRooms; roomsCreated++)  //loop through until we've spawned as many rooms as we're supposed to
        {
            randDoor = Mathf.FloorToInt(rnd.Next(0, OpenDoorList.Count));  //find a random location in the list to spawn the room off of
            //switch (0) //HACK
            switch (Mathf.FloorToInt(rnd.Next(0, 6)))  //picks a random value to spawn a randomized room 
            {
                case 0:  //spawn a four door room
                    RoomList.Add((GameObject)Instantiate(FourDoorRoom,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(9);
                    break;
                case 1:  //spawn a three door room (third door up)
                    RoomList.Add((GameObject)Instantiate(ThreeDoorRoomUp,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(7);
                    break;
                case 2:  //spawn a three door room (third door down
                    RoomList.Add((GameObject)Instantiate(ThreeDoorRoomDown,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(7);
                    break;
                case 3:  //spawn a two door room (doors up and down)
                    RoomList.Add((GameObject)Instantiate(TwoDoorRoomVer,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(5);
                    break;
                case 4:  //spawn a two door room (doors left and right)
                    RoomList.Add((GameObject)Instantiate(TwoDoorRoomHor,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(5);
                    break;
                case 5:  //spawn a dead end room
                    RoomList.Add((GameObject)Instantiate(OneDoorRoom,
                        OpenDoorList[randDoor].transform.position, Quaternion.identity));
                    index++;
                    SwitchBody(3);
                    break;
                default:
                    break;
            }
        }
    }

    //Takes the paramenter arrayNum which will be the number of children in the 
    //spawning rooms gameobject. It will be used in other functions to create
    //an array of that size. That way it doesn't create an unnecessarily large
    //array, but an array large enough to hold all the elements it needs to hold.
    //It will call the different functions that each element of the previous 
    //switch statement would call anyway.
    private void SwitchBody(int arrayNum)
    {
        if (FindDoorLocation(arrayNum))
        {
            if (!CheckRoomOverlap())
            {
                AddNewOpenDoors(arrayNum);
            }
        }
        else
        {
            RemoveRoomAndDoor();
        }
    }

    //this function is another check to see if the room should be spawned.
    //after the room has been created and moved to the proper area, it checks to see
    //if its location (the center of the room) matches up to any other room location
    //in the list.if it doesn't find any other matches, it returns false saying the room
    //is not overlapping any other room. If it finds a matching room, it destorys the room
    //and reutns true saying that there was an overlapping room and we should not continue
    //with this room any further
    private bool CheckRoomOverlap()
    {
        for (int i = 0; i < RoomList.Count; i++)
        {
            if ((RoomList[i].transform.position ==
                RoomList[index].transform.position) && i != index)
            {
                RemoveRoomAndDoor();
                return true;
            }
        }
        return false;
    }

    //function to find where to spawn the door at. Returns a bool to decide whether 
    //or not the spawn was successful. takes an int that will be used to initialize
    //an array of that size. Used because each prefab as a differing amount of 
    //children to consider. This way there are no left over children if the array was
    //created too big for the next iteration, and can be big enough if the previous
    //iteration had fewer children than the current one.
    private bool FindDoorLocation(int arrayNum)
    {
        //if the door we are spawning off is the north door
        if (OpenDoorList[randDoor].tag == "North")
        {
            //make the new room connect to it via its south door
            return GetConnectingDoorLoc("South", arrayNum);
        }
        //if the door we are spawning off is the east door
        else if (OpenDoorList[randDoor].tag == "East")
        {
            //make the new room connect to it via its west door
            return GetConnectingDoorLoc("West", arrayNum);
        }
        //if the door we are spawning off is the south door
        else if (OpenDoorList[randDoor].tag == "South")
        {
            //make the new room connect to it via its north door
            return GetConnectingDoorLoc("North", arrayNum);
        }
        //if the door we are spawning off is the west door
        else if (OpenDoorList[randDoor].tag == "West")
        {
            //make the new room connect to it via its east door
            return GetConnectingDoorLoc("East", arrayNum);
        }
        //if some other value is here incorrectly
        else
            return false;//dont spawn whatever it is
    }

    //continuation of the findDoorLocation function. returns whether or not the room
    //should be spawned. Takes in a directional string of what tag it shold be looking
    //for in the children. Also takes in an int that will initialize an array so 
    //it can hold the appropriate amount of children from the parent.
    //if it find the correct tag, it finds the distance between where the room should be
    //and where the room is. It then moves the parent object that much and in the correct 
    //direction so that the rooms will be properly connected.
    private bool GetConnectingDoorLoc(string direction, int arrayNum)
    {
        Vector3 tempDis;  //used to determine how much to offset a spawned room
        Transform[] doorLocs = new Transform[arrayNum];
        doorLocs = RoomList[index].GetComponentsInChildren<Transform>();
        foreach (Transform room in doorLocs)
        {
            if (room.tag == direction)
            {
                tempDis = room.transform.position - OpenDoorList[randDoor].transform.position;
                RoomList[index].transform.position -= tempDis;
                return CheckDoorsAreClear(direction, arrayNum);
                #region GetConnectingDoorLocFirstPass
                //following are some examples I tried to move the room to the correct position
                //RoomList[index].transform.position += OpenDoorList[randDoor].transform.localPosition;
                //room.transform.position = OpenDoorList[0].transform.position;
                //RoomList[index].transform.position += room.transform.position;
                //RoomList[index].transform.position = room.transform.position - room.transform.localPosition;
                #endregion
            }
        }
        return false;  //destroy the created room
    }

    //this function takes in the parameters that let it know how many children
    //the spawning instance has so it can create an array with the exact amount
    //it needs as well as a string for the direction its being spawned from.
    //This function will check all the rooms (if they exist) around it (not
    //including the one it is spawning from) to see if it will block any of
    //their doors or they will block any of its doors. If either is the case,
    //it won't spawn this room. If it doesn't block any or get blocked, then 
    //it will say it is ok to finish spawning this room.
    private bool CheckDoorsAreClear(string direction, int arrayNum)
    {
        Transform[] allElements = new Transform[arrayNum];
        allElements = RoomList[index].GetComponentsInChildren<Transform>();
        if (direction == "North")
        {
            if (CheckEastRoom(allElements) &&
                CheckSouthRoom(allElements) && CheckWestRoom(allElements))
                return true;
            else
                return false;
        }
        else if(direction == "East")
        {
            if (CheckNorthRoom(allElements) &&
                CheckSouthRoom(allElements) && CheckWestRoom(allElements))
                return true;
            else
                return false;
        }
        else if (direction == "South")
        {
            if (CheckEastRoom(allElements) &&
                CheckNorthRoom(allElements) && CheckWestRoom(allElements))
                return true;
            else
                return false;
        }
        else if (direction == "West")
        {
            if (CheckEastRoom(allElements) &&
                CheckSouthRoom(allElements) && CheckNorthRoom(allElements))
                return true;
            else
                return false;
        }
        return false;

# region CheckDoorsAreClearFirstPass
        //Check current spawned room's doors to see if they are blocked.
        //Done by checking tag of door we are looking, and depending what its
        //tag is, check either block spacing(in this case 5) above, below, to
        //the right, or to the left of it. If there is a room blocking one of 
        //the doors, check if there is a door connecting the two rooms.
        //If there is a door connecting, continue on checking the other doors.
        //If all the other doors are clear, finish spawning the room. 
        //If any of the doors are blocked. Do not spawn this room. However,
        //do not mark the doorway this room was spawned off of as closed.

        //this actually doesn't work. Because it only is checking the doors of the 
        //room being spawned, instead of checking the doors of the rooms that were
        //already spawned around it
        /*
        foreach (Transform door in allElements)
        {
            if(door.tag == "North" && direction != "North")
            {
                Vector3 tempLoc = (RoomList[index].transform.position
                    + new Vector3(0, blockSpace));
                if (!IsDoorClear(tempLoc, "South"))
                {
                    return false;
                }
            }
            else if(door.tag == "East" && direction != "East")
            {
                Vector3 tempLoc = (RoomList[index].transform.position 
                    + new Vector3(blockSpace, 0));
                if (!IsDoorClear(tempLoc, "West"))
                {
                    return false;
                }
            }
            else if(door.tag == "South" && direction != "South")
            {
                Vector3 tempLoc = (RoomList[index].transform.position
                    + new Vector3(0, -blockSpace));
                if (!IsDoorClear(tempLoc, "North"))
                {
                    return false;
                }
            }
            else if(door.tag == "West" && direction != "West")
            {
                Vector3 tempLoc = (RoomList[index].transform.position
                    + new Vector3(-blockSpace, 0));
                if(!IsDoorClear(tempLoc, "East"))
                {
                    return false;
                }
            }
        }
        return true;
        */
#endregion
    }

    //These functions check each of the surrounding rooms for three cases
    //Case 0, there is a room in the direction it is checking, but doesn't 
    //have a door in that direction. So it checks if the spawning room has a
    //door in that direction. if it doesn't, it can continue checking the 
    //next direction. if it does, don't spawn this room.
    //Case 1, there is a room in the direction it is checking, and it has
    //a door in that direction. So it checks if the spawning room has a door
    //in that direction. if it does, it can continue checking the next direction.
    //if it doesn't, don't spawn this room.
    //Case 2, there is not a room in that direction. It can continue checking
    //the next direction.
    private bool CheckNorthRoom(Transform[] allElements)
    {
        Vector3 tempLoc = (RoomList[index].transform.position
        + new Vector3(0, blockSpace));
        int switchCheck = IsDoorClear(tempLoc, "South");
        switch (switchCheck)
        {
            case 0:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "North")
                    {
                        return false;
                    }
                }
                return true;
            case 1:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "North")
                    {
                        return true;
                    }
                }
                return false;
            case 2:
                return true;
        }
        return false;
    }
    private bool CheckEastRoom(Transform[] allElements)
    {
        Vector3 tempLoc = (RoomList[index].transform.position
        + new Vector3(blockSpace, 0));
        int switchCheck = IsDoorClear(tempLoc, "West");
        switch (switchCheck)
        {
            case 0:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "East")
                    {
                        return false;
                    }
                }
                return true;
            case 1:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "East")
                    {
                        return true;
                    }
                }
                return false;
            case 2:
                return true;
        }
        return false; ;
    }
    private bool CheckSouthRoom(Transform[] allElements)
    {
        Vector3 tempLoc = (RoomList[index].transform.position
        + new Vector3(0, -blockSpace));
        int switchCheck = IsDoorClear(tempLoc, "North");
        switch (switchCheck)
        {
            case 0:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "South")
                    {
                        return false;
                    }
                }
                return true;
            case 1:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "South")
                    {
                        return true;
                    }
                }
                return false;
            case 2:
                return true;
        }
        return false;
    }
    private bool CheckWestRoom(Transform[] allElements)
    {
        Vector3 tempLoc = (RoomList[index].transform.position
        + new Vector3(-blockSpace, 0));
        int switchCheck = IsDoorClear(tempLoc, "East");
        switch (switchCheck)
        {
            case 0:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "West")
                    {
                        return false;
                    }
                }
                return true;
            case 1:
                foreach (Transform door in allElements)
                {
                    if (door.tag == "West")
                    {
                        return true;
                    }
                }
                return false;
            case 2:
                return true;
        }
        return false;
    }

    //Continuation of the checking(direction)room functions.
    //returns a value depending on the cases stated above
    private int IsDoorClear(Vector3 locToCheck, string doorDir)
    {
        for(int i = 0; i < RoomList.Count; i++)
        {
            if(RoomList[i].transform.position == locToCheck)
            {
                if (DoesRoomConnect(doorDir, i))
                {
                    return 1;
                }
                else
                    return 0;
            }
        }
        return 2;
    }

    //Continuation of the previous function. lets it know if the room
    //it is checking has a door that connects it to the spawning room
    private bool DoesRoomConnect(string doorDir, int roomIndex)
    {
        //wont know how many elements are in the parent so
        //we'll use the number of children that are in the largest parent 
        Transform[] allElements = new Transform[9];  
        allElements = RoomList[roomIndex].GetComponentsInChildren<Transform>();
        foreach(Transform door in allElements)
        {
            if(door.tag == doorDir)
            {
                return true;
            }
        }
        return false;
    }

    //this function destroys the room that was created. It also takes in a bool
    //that determins whether or not the room it was spawned off of should be removed.
    private void RemoveRoomAndDoor()
    {
        Destroy(RoomList[index]);
        RoomList.RemoveAt(index);
        index--;
        roomsCreated--;
    }

    //this function adds all the doors of the newly spawned room to the opendoor list.
    //it takes in an int to once again initialize an array that can hold the correct
    //number of children the parent has. It also does not add the door that connects
    //to the spawned door, as it is not open.
    private void AddNewOpenDoors(int arrayNum)
    {
        Transform[] allElements = new Transform[arrayNum];
        allElements = RoomList[index].GetComponentsInChildren<Transform>();
        foreach (Transform room in allElements)
        {
            if (room.tag == "North" || room.tag == "South" ||
                room.tag == "East" || room.tag == "West")
            {
                if (room.transform.position != OpenDoorList[randDoor].position)
                {
                    if (CheckDuplicates(room.transform.position))
                    {
                        OpenDoorList.Add(room.GetComponent<Transform>());
                        openDoorsRemaining++;
                    }
                }
            }
        }
        OpenDoorList.RemoveAt(randDoor);
        RemoveDoors();
        openDoorsRemaining--;
    }

    //makes sure it doesnt add open doors if the spawned room connects to other doors
    //other than the one it spawned off of. Adds the doors it shouldn't have in the list
    //to a different list to be destoryed after the loop.
    private bool CheckDuplicates(Vector3 doorPos)
    {
        foreach(Transform openDoor in OpenDoorList)
        {
            if(doorPos == openDoor.position && 
                openDoor.position != OpenDoorList[randDoor].position)
            {
                DoorsToRemove.Add(openDoor);
                return false;
            }
        }
        return true;
    }

    //removes all the doors that aren't open from the opendoorlist. removes all elements
    //from the temporary storage (doorstoremove) list
    private void RemoveDoors()
    {
        foreach(Transform door in DoorsToRemove)
        {
            OpenDoorList.Remove(door);
            openDoorsRemaining--;
        }
        DoorsToRemove.Clear();
    }

    //Spawns all the caps on the doors that were still open after all the rooms had 
    //been spawned. Creates a gameobject at those locations that has a collider that
    //will prevent the player from leaving the level through those open doors. 
    //Depending on which side of the room the door is located, the cap is rotated to fit
    //that side of the door.
    private void SpawnCaps()
    {
        foreach(Transform door in OpenDoorList)
        {
            CapsOfRooms.Add((GameObject)Instantiate(DoorCap,
                            door.position, Quaternion.identity));
            switch (door.tag)
            {
                case "East":
                    CapsOfRooms[CapsOfRooms.Count - 1].transform.Rotate(0, 0, -90f);
                    break;
                case "South":
                    CapsOfRooms[CapsOfRooms.Count - 1].transform.Rotate(0, 0, 180f);
                    break;
                case "West":
                    CapsOfRooms[CapsOfRooms.Count - 1].transform.Rotate(0, 0, 90f);
                    break;
            }
        }
    }

    //Destroys all room and cap gameobjects and clears the lists so that
    //a new level can be spawned in. Public function so a game manager can 
    //call it.
    public void ResetLevel()
    {
        foreach(GameObject room in RoomList)
        {
            Destroy(room);
        }
        foreach(GameObject cap in CapsOfRooms)
        {
            Destroy(cap);
        }
        RoomList.Clear();
        CapsOfRooms.Clear();
        OpenDoorList.Clear();
    }
}
