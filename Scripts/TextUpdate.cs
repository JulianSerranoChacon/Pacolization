using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextUpdate : MonoBehaviour
{
    [SerializeField]
    public uint ID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }
     void Start()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text =  LocalInterface.Instance().GetLine(ID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}