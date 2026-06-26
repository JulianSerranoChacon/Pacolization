using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    //stringMap es un diccionario (en C# se implementan mediante unordered maps). 
    private XmlNode languageNode;
    private Dictionary<uint, string> stringMap;
    private Dictionary<uint, Pair<ScriptableObject, FieldInfo>> refScriptObj;


    //Diccionario en la que guardamos los datos de las variables
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    //Diccionario en la que guardamos los datos de los modificadores de genero
    private Dictionary<string, int> generos = new Dictionary<string, int>();
    //Diccionario en el que guardamos las cantidades de los plurales
    private Dictionary<string, int> cantidades = new Dictionary<string, int>();
    //Informacion del formato actual para escribir los números y la moneda
    NumberFormatInfo numberFormatInfo;

    private List<TextUpdate> textUpdateRefs;

    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private uint currentLang;

    public IReadOnlyDictionary<uint, string> GetLines => stringMap;
    public IReadOnlyDictionary<string, string> GetVariables => variables;
    public IReadOnlyDictionary<string, int> GetGeneros => generos;
    public IReadOnlyDictionary<string, int> Getcantidades => cantidades;

    public XmlNode getLanguageNode()
    {
        return languageNode;
    }
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

    public void Initiate()
    {
        stringMap = new Dictionary<uint, string>();

        refScriptObj = new Dictionary<uint, Pair<ScriptableObject, FieldInfo>>();
        textUpdateRefs = new List<TextUpdate>();    

        currentLang = 0;
    }

    #region Registers
        public void RegisterTextUpdate(TextUpdate textUp)
    {
        textUpdateRefs.Add(textUp);
    }
    public void DeregisterTextUpdate(TextUpdate textUp)
    {
        textUpdateRefs.Remove(textUp);
    }
    public void ClearTextUpdate()
    { 
        textUpdateRefs.Clear(); 
    }
        
    #endregion

    #region  Getters and Setters
    public void  SetLanguageConfig(XmlNode conf)
    {
        languageNode = conf;
    }

    //Devuelve el string de la ID correspondiente del idioma que esta activo.
    public string GetLine(uint ID)
    {
        if(!stringMap.ContainsKey(ID))
            throw new ArgumentException("No value assigned to corresponding key.");

        return stringMap[ID];
    }

    //Escribe la linea de la ID correspondiente al idioma que esta activo. 
    //Si la ID es nueva, crea un array y lo almacena
    public void SetLine(uint ID, string value)
    {
        
        if(!stringMap.ContainsKey(ID))
                stringMap.Add(ID, value);
        else
            stringMap[ID] = value;
    }
    public bool IsRightToLeft()
    {
        string direccion = languageNode
        .SelectSingleNode("Texto")
        .SelectSingleNode("Direccion")
        .InnerText;
        return direccion != "Iz_Der";
    }

    public void WriteCantidades(string key, int value)
    {
        if (cantidades.ContainsKey(key))
            cantidades[key] = value;
        else
            cantidades.Add(key, value);
    }

    //Metodo que permite añadir una modificación de genero al diccionario de modificaciones de genero, pasandole como parametro el nombre key, y su valor value
    public void WriteGenderConfToXML(string key, int value)
    {
        if (generos.ContainsKey(key)) generos[key] = value;   //Si ya existe, la actualizamos
        else generos.Add(key, value);   //Si no existe, la creamos
    }
    public void WriteVariables(string key, string value)
    {
        if (variables.ContainsKey(key))
            variables[key] = value;
        else
            variables.Add(key, value);
    }

    public NumberFormatInfo getNumberFormatInfo()
    {
        return numberFormatInfo;
    }
    public void setNumberFormatInfo(NumberFormatInfo nI)
    {
        numberFormatInfo = nI;
    }

    #endregion

    #region Language Configuration

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado.
    public void ChangeLang(uint newLang)
    {

        currentLang = newLang; 
        refreshTexts();
    }

    public void refreshTexts()
    {
        foreach (TextUpdate refs in textUpdateRefs)
        {
            refs.SetText();
        }
    }
    #endregion

    #region Scriptable References
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

            if (!stringMap.ContainsKey(uint.Parse(val.ToString())))
            {
                throw new ArgumentException("No value found for ScriptableObject.");
            }
            else
            {
                item.Value.second.SetValue(item.Value.first, stringMap[uint.Parse(val.ToString())]);
            }

        }
    }

    public void clearMap()
    {
        stringMap.Clear();
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