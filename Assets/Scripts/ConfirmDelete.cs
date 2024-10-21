using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConfirmDelete : MonoBehaviour
{
    public GameObject level;
    public string Path;

    // Deletes the level with is meta data
    public void ConfirmDeleteLevel()
    {
        Debug.Log("Deleted Level: " + this.Path);

        if (File.Exists(this.Path))
            File.Delete(this.Path);

        if (File.Exists(this.Path + ".meta"))
            File.Delete(this.Path + ".meta");

        Destroy(this.gameObject);
        Destroy(this.level);
    }

    // Doesnt delete level, closes connfirm UI
    public void DeclineDeleteLevel()
    {
        Destroy(this.gameObject);
    }
}