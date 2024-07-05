using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public string Path;
    public string LevelName;
    public TMPro.TMP_Text Text;

    private void OnMouseDown()
    {
        PlayerPrefs.SetString("SavePath", this.Path);
        SceneManager.LoadScene("LevelEditor");
    }
}
