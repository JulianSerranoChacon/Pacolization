using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextExtraction : MonoBehaviour
{
    List<TextMeshPro> tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = new List<TextMeshPro>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<TextMeshPro>(true));
            }
        }
    }

}
