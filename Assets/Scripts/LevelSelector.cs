using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject LevelPrefab;
    [SerializeField] private GameObject NewLevelPrefab;
    [SerializeField] private List<GameObject> LevelList;
    [SerializeField] private RectTransform LevelsTransform;

    private string LevelDirectory = Application.dataPath + "/Levels";

    void Start()
    {
        LoadLevels();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    private void LoadLevels()
    {
        string[] allPaths = Directory.GetFiles(LevelDirectory); // Gets all files, including meta files
        string[] levelPaths = allPaths.Where(s => s.EndsWith(".txt")).ToArray(); // Takes only the .txt files from all files

        int levelIndex = 0;
        foreach (string levelPath in levelPaths)
        {
            string filePath = Path.GetFileName(levelPath); // Gets current file path, with .txt at the end
            string levelName = filePath.Substring(0, filePath.Length - 4); // Gets only the name of the level from path

            LevelList.Add(Instantiate(LevelPrefab, LevelsTransform));
            LevelList[levelIndex].GetComponent<Level>().Path = levelPath;

            LevelList[levelIndex].GetComponent<Level>().LevelName = levelName;
            LevelList[levelIndex].GetComponent<Level>().Text.text = levelName;
            LevelList[levelIndex].GetComponent<Level>().IsNewLevel = false;
            
            foreach (Transform childTransform in LevelList[levelIndex].transform)
            {
                if (childTransform.name == "Index")
                {
                    childTransform.gameObject.GetComponent<UnityEngine.UI.Text>().text = levelIndex.ToString();
                }
            }

            levelIndex++;
        }

        LevelList.Add(Instantiate(NewLevelPrefab, LevelsTransform));
        LevelList[levelIndex].GetComponent<Level>().IsNewLevel = true;
    }

    public void DeleteLevel (Text indexText)
    {

    }
}
