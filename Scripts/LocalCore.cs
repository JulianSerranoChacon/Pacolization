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
    //private Dictionary<uint, string[]> stringTable;

    private Dictionary<uint, Dictionary<uint, string>> stringMap;
    private Dictionary<uint, Pair<ScriptableObject, FieldInfo>> refScriptObj;

    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private uint currentLang;

    //public IReadOnlyDictionary<uint, string[]> GetLines => stringTable;
    public IReadOnlyDictionary<uint, Dictionary<uint, string>> GetLines => stringMap;
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

        //stringTable = new Dictionary<uint, string[]>();
        stringMap = new Dictionary<uint, Dictionary<uint, string>>();
        refScriptObj = new Dictionary<uint, Pair<ScriptableObject, FieldInfo>>();  

        currentLang = 0;
    }

    //Devuelve el string de la ID correspondiente del idioma que esta activo.
    public string GetLine(uint ID)
    {
        //string[] box;

        /*if(stringTable.TryGetValue(ID, out box))
            return box[currentLang];
        else
            throw new ArgumentException("No value assigned to corresponding key.");*/

        if(!stringMap.ContainsKey(currentLang) || !stringMap[currentLang].ContainsKey(ID))
            throw new ArgumentException("No value assigned to corresponding key.");

        return stringMap[currentLang][ID];
    }

    //Escribe la linea de la ID correspondiente al idioma que esta activo. 
    //Si la ID es nueva, crea un array y lo almacena
    public void SetLine(uint ID, string value)
    {
        /*string[] box;

        if(stringTable.TryGetValue(ID, out box))
            box[currentLang] = value;
        else
        {
            box = new string[languages];
            box[currentLang] = value;
            stringTable[ID] = box;
        }*/

        if (!stringMap.ContainsKey(currentLang)){
            stringMap.Add(currentLang, new Dictionary<uint, string>());
        }
        stringMap[currentLang].Add(ID, value);
        //Debug.Log(currentLang);

        
    }

    public int getLang()
    {
        return languages;
    }

    public void SetLine(uint ID, int lang, string value)
    {
        string[] box;
        Debug.Log(languages);
        Debug.Log(lang);
        if (stringTable.TryGetValue(ID, out box))
            box[lang] = value;
        else
        {
            box = new string[languages];
            box[lang] = value;
            stringTable[ID] = box;
        }

        //Debug.Log(currentLang);
    }

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado.
    public void ChangeLang(uint newLang)
    {
        if(newLang >= languages)
            throw new ArgumentException("New language value exceeding range of languages.");

        currentLang = newLang; 
    }

    public void SceneLoaded()
    {
        //SetTMPStrings();
    }

    #region Reference gaming


    public void SetScriptableObjectReference(uint ID, ScriptableObject obj, FieldInfo info)
    {
        refScriptObj[ID] = new Pair<ScriptableObject, FieldInfo>(obj, info);
    }

    private void SetScriptableStrings()
    {
        foreach(var item in refScriptObj)
        {
            string[] box;
            object val = item.Value.second.GetValue(item.Value.first);
            if (stringTable.TryGetValue(uint.Parse(val.ToString()), out box))
            {
                item.Value.second.SetValue(item.Value.first, box[currentLang]);
            }
        }
    }
  
    public void FlushScriptableReferences()
    {
        foreach (var item in refScriptObj)
        {
                item.Value.second.SetValue(item.Value.first, item.Key.ToString());       
        }
    }
    #endregion

#endregion
}