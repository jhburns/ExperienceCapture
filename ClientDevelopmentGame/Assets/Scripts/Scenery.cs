using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Scenery : MonoBehaviour {
    public string nextSceneName;

    public void reload()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void done()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
