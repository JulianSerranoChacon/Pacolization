using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pacolization : MonoBehaviour
{
    [SerializeField]
    private uint langs = 0;
    [SerializeField]
    private bool scanScriptables;
    [SerializeField]
    private string scriptablePath;
    [SerializeField]
    private string[] filePath;
    [SerializeField]
    private string confPath;
    [SerializeField]
    private uint initialLang = 0;
    /*
    [SerializeField]
    public string variablePath;*/
    [SerializeField]
    public string genderConfigurationPath;

    private static Pacolization _instance;
    public static Pacolization Instance()
    { return _instance; }
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            LocalInterface.Instance().Initiate(langs, scanScriptables, scriptablePath);
            LocalInterface.Instance().StartInExecution(filePath[initialLang], 0, confPath, /*variablePath,*/ genderConfigurationPath);
        }
        else
        {
            Destroy(this);
        }
    }
     void OnApplicationQuit()
    {
      LocalInterface.Instance().OnQuit();    
    }
}
