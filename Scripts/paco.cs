using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class paco : MonoBehaviour
{
    [SerializeField]
    public uint langs;
    [SerializeField]
    public bool scanScriptables;
    [SerializeField]
    public string scriptablePath;
    [SerializeField]
    public string filePath;
    [SerializeField]
    public string confPath;
    [SerializeField]
    public string variablePath;

    private static paco _instance;
    public static paco Instance()
    { return _instance; }
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Debug.Log("Madre");
        if (_instance == null)
        {
            Debug.Log("De Jose");
            _instance = this;
            DontDestroyOnLoad(this);
            LocalInterface.Instance().Initiate(langs, scanScriptables, scriptablePath);
            LocalInterface.Instance().StartInExecution(filePath, 0, confPath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    

}
