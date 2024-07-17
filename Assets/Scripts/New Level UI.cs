using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NewLevelUI : MonoBehaviour
{
    [SerializeField] TMP_InputField NameInputField;
    [SerializeField] TMP_InputField WidthInputField;
    [SerializeField] TMP_InputField HeightInputField;

    private string LevelDirectory = Application.dataPath + "/Levels/";

    public void CreateNewLevel ()
    {
        string levelName = NameInputField.text;
        string levelWidth = WidthInputField.text;
        string levelHeight = HeightInputField.text;

        

        string path = LevelDirectory + levelName + ".txt";

        PlayerPrefs.SetString("levelName", levelName);
        PlayerPrefs.SetString("levelWidth", levelWidth);
        PlayerPrefs.SetString("levelHeight", levelHeight);

        PlayerPrefs.SetString("SavePath", path);

        SceneManager.LoadScene("LevelEditor");
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}