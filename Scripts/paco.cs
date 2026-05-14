using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class paco : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LocalInterface.Instance().Initiate(1,false,"s");
        LocalInterface.Instance().StartInExecution("Assets/Scripts/UAJ-FinalProject/Scripts/example.xml");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
