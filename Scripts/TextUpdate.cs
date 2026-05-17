using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TextUpdate : MonoBehaviour
{
    [SerializeField]
    public uint ID;
     private TMP_Text tmpText;
    private LocalInterface localInterface;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        localInterface = LocalInterface.Instance();
        setText();
    }
    void setText()
    {
        tmpText.text = localInterface.GetLine(ID);
        tmpText.isRightToLeftText = localInterface.IsRightToLeft();
    }
}