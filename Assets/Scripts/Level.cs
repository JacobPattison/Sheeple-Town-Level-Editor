using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject NewLevelUIPrefab;

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

    public void DeleteLevel()
    {
        Debug.Log("Deleted Level: " + this.Path);

        if (File.Exists(this.Path))
            File.Delete(this.Path);

        if (File.Exists(this.Path + ".meta"))
            File.Delete(this.Path + ".meta");

        Destroy(this.gameObject);
    }
}
