using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string thisSceneToLoad;
    public Key keyNeeded;

    public void LoadThisScene()
    {
        SceneManager.LoadScene(thisSceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (keyNeeded.hasPickedUp)
            {
                LoadThisScene();
            }
        }
    }
}
