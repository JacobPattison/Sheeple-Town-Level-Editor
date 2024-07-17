using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

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
        UpdateScrollTransform();
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

            string imageSavePath = Path.GetDirectoryName(levelPath) + "/Thumbnails/" + levelName + ".png";

            byte[] fileData = System.IO.File.ReadAllBytes(imageSavePath);
            Texture2D texture = new Texture2D(1920, 1080);
            texture.LoadImage(fileData);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            Transform thumbnailTransform = LevelList[levelIndex].transform.Find("Thumbnail");
            Transform imageTransform = thumbnailTransform.transform.Find("Image");
            imageTransform.GetComponent<Image>().sprite = sprite;

            levelIndex++;
        }

        LevelList.Add(Instantiate(NewLevelPrefab, LevelsTransform));
        LevelList[levelIndex].GetComponent<Level>().IsNewLevel = true;
    }

    private void UpdateScrollTransform()
    {
        GameObject newLevelUI = GameObject.Find("New Level(Clone)");
        RectTransform newLevelTransform = newLevelUI.GetComponent<RectTransform>();


        float sourceY = newLevelTransform.anchoredPosition.y;
        float newBottom = sourceY - LevelsTransform.rect.height;


        Vector2 offsetMin = LevelsTransform.offsetMin;
        offsetMin.y = newBottom;
        LevelsTransform.offsetMin = offsetMin;
    }

    public void Exit()
    {
        Debug.Log("Application Quit");
        UnityEditor.EditorApplication.ExitPlaymode();
        Application.Quit();
    }
}