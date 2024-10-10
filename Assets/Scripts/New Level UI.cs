using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class NewLevelUI : MonoBehaviour
{
    [SerializeField] TMP_InputField NameInputField;
    [SerializeField] TMP_InputField WidthInputField;
    [SerializeField] TMP_InputField HeightInputField;
    [SerializeField] TMPro.TMP_Text Error;

    private string LevelDirectory = Application.dataPath + "/Levels/";

    public void CreateNewLevel ()
    {
        string levelName = NameInputField.text;
        string levelWidth = WidthInputField.text;
        string levelHeight = HeightInputField.text;

        if (levelName == "" || levelWidth == "" || levelHeight == "")
        {
            Error.text = "All Input Fields Must Be Filled";
            return;
        }
        if (levelName.Length > 100)
        {
            Error.text = "Level Name Can Not Be Greater Than 100 Character";
            return;
        }
        if (!levelHeight.All(char.IsDigit) || !levelWidth.All(char.IsDigit))
        {
            Error.text = "Level Dimentions Should Only Contain Numbers";
            return;
        }
        if (int.Parse(levelHeight) > 100 || int.Parse(levelWidth) > 100)
        {
            Error.text = "Level Dimentions Should Be Less Than 100 In Size";
            return;
        }

        string path = LevelDirectory + levelName + ".txt";

        PlayerPrefs.SetString("levelName", levelName);
        PlayerPrefs.SetString("levelWidth", levelWidth);
        PlayerPrefs.SetString("levelHeight", levelHeight);

        PlayerPrefs.SetString("SavePath", path);

        SceneManager.LoadScene("LevelEditor");
        Debug.Log($"New Level Created: {levelName}, {levelWidth}, {levelHeight}");
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}