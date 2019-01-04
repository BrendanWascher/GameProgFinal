using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;  //designer changeable player speed
    private Vector3 direction = new Vector2();  //the direction the player is going
    private Vector3 keyDirection = new Vector2();  //the direction the keys say the player should go in
    private bool isKeyPressed;  //whether or not a key has been pressed
	
    //Get keyboard input and set the values of how much the player should move
    //or if they should move at all
	void Update ()
    {
        ResetKeyAndDirection();
        GetKeyInput();
        SetDirection();
	}

    //reset the values of key direction and whether or not a key has been pressed
    private void ResetKeyAndDirection()
    {
        keyDirection.x = 0;
        keyDirection.y = 0;
        isKeyPressed = false;
    }

    //Get input from the keyboard and add values depending on what key was pressed
    private void GetKeyInput()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyDirection.x += 1;
            isKeyPressed = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyDirection.x -= 1;
            isKeyPressed = true;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            keyDirection.y += 1;
            isKeyPressed = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            keyDirection.y -= 1;
            isKeyPressed = true;
        }
    }

    //set how much the player should move based on key input
    private void SetDirection()
    {
        if (!isKeyPressed)
        {
            direction = Vector3.zero;
        }
        direction += keyDirection;
        direction.Normalize();
    }

    //move at a fixed speed
    private void FixedUpdate()
    {
        transform.Translate(direction *speed * Time.fixedDeltaTime);
    }
}
