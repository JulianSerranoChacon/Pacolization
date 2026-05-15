using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class LocalCore
{

#region Atributos
    //stringTable es un diccionario (en C# se implementan mediante unordered maps). 
    //Cada key alberga un array de tamano languages. 
    //En cada posicion del array se encuentra el string en un idioma concreto.
    private int languages;
    private Dictionary<uint, string[]> stringTable;
    private Dictionary<uint, TMP_Text> refTable;
    private Dictionary<uint, Tuple<ScriptableObject, FieldInfo>> refScriptObj;
    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private int currentLang;

    public IReadOnlyDictionary<uint, string[]> GetLines => stringTable;
#endregion

#region Singleton
    //La clase necesitara ser un singleton ya que solo queremos que exista una
    public LocalCore() {}

    private static LocalCore _instance;

    public static LocalCore Instance()
    {
         if (_instance == null)
        {
            _instance = new LocalCore();
        }
        return _instance;
    }
#endregion

#region Metodos
    //Inicia los atributos de la clase
    //Establece el maximo de idiomas a langAm

    public void Initiate(int langAm)
    {
        if(langAm <= 0)
            throw new ArgumentException("Ammount of languages cannot be negative or 0.");

        languages = langAm;
        stringTable = new Dictionary<uint, string[]>();
        refTable = new Dictionary<uint, TMP_Text>();
        refScriptObj = new Dictionary<uint, Tuple<ScriptableObject, FieldInfo>>();  

        currentLang = 0;
    }

    //Devuelve el string de la ID correspondiente del idioma que esta activo.
    public string GetLine(uint ID)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
            return box[currentLang];
        else
            throw new ArgumentException("No value assigned to corresponding key.");
    }

    //Escribe la linea de la ID correspondiente al idioma que esta activo. 
    //Si la ID es nueva, crea un array y lo almacena
    public void SetLine(uint ID, string value)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
            box[currentLang] = value;
        else
        {
            box = new string[languages];
            box[currentLang] = value;
            stringTable[ID] = box;
        }

        //Debug.Log(currentLang);
    }

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado.
    public void ChangeLang(int newLang)
    {
        if(newLang >= languages)
            throw new ArgumentException("New language value exceeding range of languages.");

        currentLang = newLang; 
        //SetAllStrings();
    }

    public void SceneLoaded()
    {
        SetTMPStrings();
    }

    #region Reference gaming
    public void SetTMPReference(uint ID, TMP_Text reff)
    {
        refTable[ID] = reff;
    }

    public void SetScriptableObjectReference(uint ID, ScriptableObject obj, FieldInfo info)
    {
        refScriptObj[ID] = new Tuple<ScriptableObject, FieldInfo>(obj, info);
    }

    //Se cambia el idioma y todas las respectivas referencias
    private void SetTMPStrings()
    {
        foreach(TMP_Text reff in refTable.Values)
        {
            string[] box;

            if(stringTable.TryGetValue(uint.Parse(reff.text), out box))
                reff.text = box[currentLang];
        }
        
    }

    private void SetScriptableStrings()
    {
        foreach(var item in refScriptObj)
        {
            string[] box;
            object val =item.Value.Item2.GetValue(item.Value.Item1);
            if (stringTable.TryGetValue(uint.Parse(val.ToString()),out box))
            {
                item.Value.Item2.SetValue(item.Value.Item1, box[currentLang]);
            }
        }
    }

    public void ClearReferences()
    {
        refTable.Clear();
    }
    #endregion

#endregion
}