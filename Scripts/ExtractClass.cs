using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtractClass 
{
    List<TMP_Text> tmp; 
    // Start is called before the first frame update
    public void ExtractStrings()
    {
        //Se crea una nueva lista al principio para evitar que se llene con infomacion repetida
        tmp = new List<TMP_Text>();
        //cogemos primero la direccion de las escena en la que estamos
        string activeScenePath = SceneManager.GetActiveScene().path;
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string scenePath = EditorBuildSettings.scenes[i].path;
            //En caso de que ya estemos en la escena, no la cargamos
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            foreach (var root in SceneManager.GetSceneByBuildIndex(i).GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<TMP_Text>(true));
            }
            //cerramos la escena antes de irnos a la siguiente escena
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByBuildIndex(i), true);
            }
        }
        Debug.Log(tmp.Count);
    }

}
