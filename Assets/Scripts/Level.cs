using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject NewLevelUIPrefab;

    public string Path;
    public string LevelName;
    public TMPro.TMP_Text Text;
    public bool IsNewLevel = false;

    private void OnMouseDown()
    {
        if (IsNewLevel)
        {
            InstantiateNewLevelUI();
        }
        else
        {
            PlayerPrefs.SetString("SavePath", this.Path);
            SceneManager.LoadScene("LevelEditor");
        }
    }

    private void InstantiateNewLevelUI()
    {
        Instantiate(NewLevelUIPrefab, GameObject.Find("Level Selector").transform);
    }
}
