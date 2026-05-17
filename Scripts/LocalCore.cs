using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LocalCore
{

#region Atributos
    //stringTable es un diccionario (en C# se implementan mediante unordered maps). 
    //Cada key alberga un array de tamano languages. 
    //En cada posicion del array se encuentra el string en un idioma concreto.
    private uint languages;
    //private Dictionary<uint, string[]> stringTable;

    private Dictionary<uint, XmlNode> languageMap;
    private Dictionary<uint, Dictionary<uint, string>> stringMap;
    private Dictionary<uint, Pair<ScriptableObject, FieldInfo>> refScriptObj;

    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private uint currentLang;

    //public IReadOnlyDictionary<uint, string[]> GetLines => stringTable;
    public IReadOnlyDictionary<uint, Dictionary<uint, string>> GetLines => stringMap;
    public IReadOnlyDictionary<uint, XmlNode> GetLanguageMap => languageMap;
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

    public bool IsRightToLeft()
    {
        string direccion = languageMap[currentLang]
        .SelectSingleNode("Texto")
        .SelectSingleNode("Direccion")
        .InnerText;
        return direccion!= "Iz_Der";
    }
    public void Initiate(uint langAm)
    {
        if(langAm <= 0)
            throw new ArgumentException("Ammount of languages cannot be negative or 0.");

        languages = langAm;

        stringMap = new Dictionary<uint, Dictionary<uint, string>>();
        languageMap = new Dictionary<uint, XmlNode>();

        refScriptObj = new Dictionary<uint, Pair<ScriptableObject, FieldInfo>>();

        currentLang = 0;
    }
    public void  SetLanguageConfig(Dictionary<uint, XmlNode> conf)
    {
        languageMap = conf;
    }


    //Devuelve el string de la ID correspondiente del idioma que esta activo.
    public string GetLine(uint ID)
    {
        if(!stringMap.ContainsKey(currentLang) || !stringMap[currentLang].ContainsKey(ID))
            throw new ArgumentException("No value assigned to corresponding key.");

        return stringMap[currentLang][ID];
    }

    //Escribe la linea de la ID correspondiente al idioma que esta activo. 
    //Si la ID es nueva, crea un array y lo almacena
    public void SetLine(uint ID, string value)
    {
        if (!stringMap.ContainsKey(currentLang))
        {
            throw new ArgumentException("No value assigned to corresponding Lang.");
        }
        else
        {
            if(!stringMap[currentLang].ContainsKey(ID))
                stringMap[currentLang].Add(ID, value);
            else
                stringMap[currentLang][ID] = value;
        }
    }
    public void SetLineLangs(uint ID,string value)
    {
        for(uint i=0; i<languages; i++)
        {
            stringMap[i].Add(ID, value);
        }
    }
    public uint GetNumLangs()
    {
        return languages;
    }

    public void SetLine(uint ID, uint lang, string value)
    {
         if (!stringMap.ContainsKey(lang))
        {
            throw new ArgumentException("No value assigned to corresponding Lang.");
        }
        else
        {
            if(!stringMap[lang].ContainsKey(ID))
                stringMap[lang].Add(ID, value);
            else
                stringMap[lang][ID] = value;
        }
    }

    public void AddNewLanguage(uint id)
    {
        if (!stringMap.ContainsKey(id))
                stringMap.Add(id, new Dictionary<uint, string>());
    }

    public void AddRemainingLanguages()
    {
        for(uint i = 0; i < languages; i++)
        {
            if (!stringMap.ContainsKey(i))
                stringMap.Add(i, new Dictionary<uint, string>());
        }
    }

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado.
    public void ChangeLang(uint newLang)
    {
        if(newLang >= languages)
            throw new ArgumentException("New language value exceeding range of languages.");

        currentLang = newLang; 
    }

    #region Reference gaming


    public void SetScriptableObjectReference(uint ID, ScriptableObject obj, FieldInfo info)
    {
        refScriptObj[ID] = new Pair<ScriptableObject, FieldInfo>(obj, info);
    }

    public void SetScriptableStrings()
    {
        FlushScriptableReferences();
        foreach(var item in refScriptObj)
        {
            object val = item.Value.second.GetValue(item.Value.first);

            if (!stringMap.ContainsKey(currentLang) || !stringMap[currentLang].ContainsKey(uint.Parse(val.ToString())))
            {
                throw new ArgumentException("No value found for ScriptableObject.");
            }
            else
            {
                item.Value.second.SetValue(item.Value.first, stringMap[currentLang][uint.Parse(val.ToString())]);
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