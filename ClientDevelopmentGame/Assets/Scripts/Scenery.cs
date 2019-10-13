using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenery : MonoBehaviour {

    public void reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadCleanup()
    {
        SceneManager.LoadScene("Cleanup");
    }
}
