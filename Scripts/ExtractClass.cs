using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class ExtractClass 
{
    private bool scanScriptables = false;
    private string scriptablePath = "Assets";
    private uint ID = 0;

    private Dictionary<uint,Pair<ScriptableObject,FieldInfo>> scriptObjRef;

    public ExtractClass(bool scan, string path) 
    {
        scanScriptables= scan;
        scriptablePath = path;
        scriptObjRef = new Dictionary<uint, Pair<ScriptableObject, FieldInfo>>();
    }

    //Metodo que se usa para encontrar objetos de ciertos tipos en unity
    public void ScanScriptables()
    {
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(ScriptableObject)),new String[]{ scriptablePath});
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]); 
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath); 
            if (asset != null) 
            {
                ExtractValues(asset);
            }
         }
    }

    public void ExtractValues(ScriptableObject obj)
    {
        Type objectType = obj.GetType();
        foreach (FieldInfo m in objectType.GetFields())
        {
            if (m.Attributes ==FieldAttributes.Public)
            {
                object val = m.GetValue((obj));
                if (val is string)
                {
                    scriptObjRef[ID] = new Pair<ScriptableObject, FieldInfo>(obj, m);
                    LocalCore.Instance().SetLine(ID, (string)val);
                    LocalCore.Instance().SetScriptableObjectReference(ID, obj, m);

                    ID++;
                }
            }
        }    
    }
    //Este metodo es el que se encarga de extraer todos los strings de los objetos TMP_Text del proyecto y les 
    //anade un componente TextUpdate para que estas puedan actualizarse al idioma necesario en runtime
    public void ExtractStrings()
    {
        //Se crea una nueva lista al principio para evitar que se llene con infomacion repetida
        List<TMP_Text> tmp = new List<TMP_Text>();

        //Cogemos primero la direccion de las escena en la que estamos
        string activeScenePath = SceneManager.GetActiveScene().path;

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
       
            string scenePath = EditorBuildSettings.scenes[i].path;
            //En caso de que ya estemos en la escena, no la cargamos
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            Scene currentScene = SceneManager.GetSceneByBuildIndex(i);
            foreach (var root in currentScene.GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<TMP_Text>(true));
                foreach (TMP_Text text in tmp)
                {
                    LocalCore.Instance().SetLine(ID, text.text);
                    TextUpdate temp= text.GetComponent<TextUpdate>();
                    if (temp==null)
                    {
                        temp = text.AddComponent<TextUpdate>();
                    }
                    temp.ID = ID;
                    ID++;
                }
                tmp.Clear();
            }
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            //Cerramos la escena antes de irnos a la siguiente escena
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.CloseScene(currentScene, true);
                
            }
        }
        if(scanScriptables)
        {
            ScanScriptables();
        }
    }

    public void AutoUIClampSetup()
    {
        //Se crea una nueva lista al principio para evitar que se llene con infomacion repetida
        List<TMP_Text> tmp = new List<TMP_Text>();

        //Cogemos primero la direccion de las escena en la que estamos
        string activeScenePath = SceneManager.GetActiveScene().path;

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            //Control de seguridad por si la escena esta deshabilitada en el Build Settings
            if (!EditorBuildSettings.scenes[i].enabled) continue;

            string scenePath = EditorBuildSettings.scenes[i].path;
            //En caso de que ya estemos en la escena, no la cargamos
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            //Se obtiene la escena de forma segura mediante su ruta para el bucle en el Editor
            Scene currentScene = EditorSceneManager.GetSceneByPath(scenePath);
            if (!currentScene.IsValid() || !currentScene.isLoaded) continue;

            foreach (var root in currentScene.GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<TMP_Text>(true));
                foreach (TMP_Text text in tmp)
                {
                    GUISet(text);
                }
                tmp.Clear();
            }
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            //Cerramos la escena antes de irnos a la siguiente escena
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.CloseScene(currentScene, true);
            }
        }
    }

    private void GUISet(TMP_Text text)
    {
        GameObject textGo = text.gameObject;
        RectTransform textRect = textGo.GetComponent<RectTransform>();

        //Se guarda el SIZE del texto antes de meter componentes
        Vector2 originalTextSize = textRect != null ? textRect.sizeDelta : Vector2.zero;

        //Se incluye el LayoutElement en el HIJO (objeto con texto) si no lo tiene
        LayoutElement layoutElement = textGo.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = Undo.AddComponent<LayoutElement>(textGo);
        }

        //Se le asigna el SIZE original como el SIZE preferido 
        //para que el VerticalLayoutGroup sepa cuanto mide y no colapse el texto en vertical.
        if (textRect != null && originalTextSize != Vector2.zero)
        {
            layoutElement.preferredWidth = originalTextSize.x;
            layoutElement.preferredHeight = originalTextSize.y;
        }

        //Se buca el contenedor PADRE
        Transform parentTransform = textGo.transform.parent;
        if (parentTransform != null)
        {
            GameObject parentGo = parentTransform.gameObject;
            RectTransform parentRect = parentGo.GetComponent<RectTransform>();

            //Guardamos el SIZE original del PADRE para congelar su posicion
            Vector2 originalParentSize = parentRect != null ? parentRect.sizeDelta : Vector2.zero;

            //Guardamos los datos de posicionamiento espacial del PADRE para evitar que salte de posicion
            Vector2 originalAnchorMin = parentRect != null ? parentRect.anchorMin : Vector2.zero;
            Vector2 originalAnchorMax = parentRect != null ? parentRect.anchorMax : Vector2.zero;
            Vector2 originalPivot = parentRect != null ? parentRect.pivot : Vector2.zero;
            Vector2 originalAnchoredPos = parentRect != null ? parentRect.anchoredPosition : Vector2.zero;

            //Se incluye el Content Size Fitter en el PADRE
            if (parentGo.GetComponent<ContentSizeFitter>() == null)
            {
                ContentSizeFitter fitter = Undo.AddComponent<ContentSizeFitter>(parentGo);
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            //Se incluye el Vertical Layout Group en el PADRE
            if (parentGo.GetComponent<VerticalLayoutGroup>() == null)
            {
                VerticalLayoutGroup layoutGroup = Undo.AddComponent<VerticalLayoutGroup>(parentGo);
                layoutGroup.padding = new RectOffset(5, 5, 5, 5); // Valores por defecto que se pueden cambiar
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;

                //Ahora si se activa el control para que el PADRE use los SIZE preferidos que se le dan al LayoutElement
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
            }

            //Se incluye el script UIClamper en el PADRE
            if (parentGo.GetComponent<UIClamper>() == null)
            {
                Undo.AddComponent<UIClamper>(parentGo);
            }

            //Se fuerza un refresco inmediato del layout en el editor para asentar las posiciones
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            //Se restauran las dimensiones exactas que el PADRE tenia en la interfaz original
            if (parentRect != null && originalParentSize != Vector2.zero)
            {
                parentRect.anchorMin = originalAnchorMin;
                parentRect.anchorMax = originalAnchorMax;
                parentRect.pivot = originalPivot;
                parentRect.anchoredPosition = originalAnchoredPos;
                parentRect.sizeDelta = originalParentSize;
            }
        }
    }

    //Este metodo cambia todos strings publicos de los scriptable por su ID
    public void ReplaceStrings()
    {
        foreach(var item in scriptObjRef)
        {
           item.Value.second.SetValue(item.Value.first, item.Key.ToString());
        }
    }
    
    //Este metodo extrae todas las referencias a los scriptable objects del proyecto  para mandarlo al local core
    public void ExtractRefs(ScriptableObject obj)
    {
        Type objectType = obj.GetType();
        foreach (FieldInfo m in objectType.GetFields())
        {
            if (m.Attributes == FieldAttributes.Public)
            {
                object val = m.GetValue((obj));
                if (val is string)
                {
                    LocalCore.Instance().SetScriptableObjectReference(uint.Parse((string)val), obj, m);
                }
            }
        }
    }

    //Este metodo recorre el proyecto entero en runtime para obtener las referencias a los scriptabes
    public void setScriptableRefereces()
    {
        scriptObjRef.Clear();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(ScriptableObject)), new String[] { scriptablePath });
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (asset != null)
            {
                ExtractRefs(asset);
            }
        }
    }

}