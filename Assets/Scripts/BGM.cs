using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BGM : MonoBehaviour
{
    static BGM instance;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 1) 
        {
            instance = null;
            Destroy(this.gameObject);
        }
    }
}
