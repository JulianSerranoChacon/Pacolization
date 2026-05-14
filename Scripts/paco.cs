using TMPro;
using UnityEngine;

public class paco : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string paco = "paco";
        TMP_Text texto = GetComponent<TMP_Text>();
        Convert conversion = devuelve;
        texto.text = conversion(paco);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    delegate string Convert(string f);
    private static string devuelve(string f)
    {
        return f + "salchichon";
    }
}
