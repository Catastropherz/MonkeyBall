using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manage background music across scenes
public class BGM : MonoBehaviour
{
    // Singleton instance
    public static BGM instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
             //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            // destroy this
            Destroy(gameObject);
        }


        //Set to not destroy on load
        DontDestroyOnLoad(this.gameObject);
    }
}
