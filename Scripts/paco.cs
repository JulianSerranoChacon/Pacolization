using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class paco : MonoBehaviour
{
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
            LocalInterface.Instance().Initiate(1, false, "s");
            LocalInterface.Instance().StartInExecution("Assets/Scripts/UAJ-FinalProject/Scripts/example.xml", 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
     void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    

}
