using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject NewLevelUIPrefab;
    [SerializeField] private GameObject ConfirmDeletePrefab;

    public UnityEngine.UIElements.Image Thumbnail;

    public string Path;
    public string LevelName;

    public TMPro.TMP_Text Text;

    public bool IsNewLevel = false;

    private void Start()
    {
        if (GameObject.Find("New Level UI(Clone)") == null)
            if (PlayerPrefs.GetString("IsLoadingNewLevelUI") == "True")
                InstantiateNewLevelUI();
    }

    // Loads level or new level UI
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

    // Spawns the confirm delete UI
    public void SpawnConfirmDeleteUI()
    {
        Transform levelEditor = GameObject.Find("Level Selector").transform;
        GameObject confirmDelete = Instantiate(ConfirmDeletePrefab, levelEditor);
        confirmDelete.GetComponent<ConfirmDelete>().Path = this.Path;
        confirmDelete.GetComponent<ConfirmDelete>().level = this.gameObject;
    }
}