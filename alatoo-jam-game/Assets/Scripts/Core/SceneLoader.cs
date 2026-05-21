using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    void OnEnable()
    {
        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}