using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject LevelPrefab;
    [SerializeField] private int LevelCount;
    [SerializeField] private List<GameObject> LevelList;
    [SerializeField] private Transform LevelsTransform;
    void Start()
    {
        int counter = 0;
        int x = -270;
        int y = 80;
        while (counter <= this.LevelCount + 1)
        {
            if (x > 270) x = -270;
            GameObject temp = Instantiate(LevelPrefab, new Vector2(x, y), Quaternion.identity, LevelsTransform);
            temp.transform.position = new Vector3(x, y, 0);
            LevelList.Add(temp);
            counter++;
            x += 180;
            if (counter % 4 == 0) y += 80;
        }
    }

   
}
