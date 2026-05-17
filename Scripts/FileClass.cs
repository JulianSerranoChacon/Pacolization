using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FileClass
{
    //Lista de los idiomas ordenados al leer el XML de lenguajes
    private List<string> languagesOrder = new List<string>();
    private Dictionary<string, uint> transLang = new Dictionary<string, uint>();
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    private LocalCore _core;

    public FileClass() 
    {
        _core = LocalCore.Instance();
    }

    public void WriteXML(string path, uint lenguages)
    {
        //Doc XML donde vamos a guardar los datos del localCore
        XmlDocument xmlDoc = new XmlDocument();

        //Escribimos primero la cabecera del XML
        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(declaration);

        //Elemento raiz del que colgaran todos los textos
        XmlElement root = xmlDoc.CreateElement("translations");
        xmlDoc.AppendChild(root);

        //
        Dictionary<uint, XmlElement> textnodes = new Dictionary<uint, XmlElement>();

        //Recorremos todo el unorderedMap de textos del idioma ID
        foreach (KeyValuePair<uint, Dictionary<uint, string>> pair in _core.GetLines)
        {
            //Es el ID del idioma en el unordermap que almacena todos los idiomas
            uint langId = pair.Key;

            //Comprobamos el ID del idioma la que pertenecen los textos
            string langName;
            if (langId < languagesOrder.Count)
                langName = languagesOrder[(int)langId];
            else
                langName = "langNotDefined_" + (int)langId;

            foreach (KeyValuePair<uint, string> item in pair.Value) 
            {
                //Id del texto en el unordered_map
                uint textId = item.Key;
                //Si no esta en la tabla auxiliar de nodos xml creamos el nodo <text>
                if (!textnodes.ContainsKey(textId)) 
                {
                    XmlElement textElement = xmlDoc.CreateElement("text");
                    textElement.SetAttribute("id", textId.ToString());
                    //Añadimos al root el elemento <text>
                    root.AppendChild(textElement);

                    textnodes.Add(textId, textElement);
                }

                //Creamos el nodo hijo del <text> -> <es> <en>....
                XmlElement langNode = xmlDoc.CreateElement(langName);
                langNode.InnerText = item.Value;

                //Almacenamos el nuevo idioma en la lista de nodos del ID
                textnodes[textId].AppendChild(langNode);
            }
        }
        //Antes de acabar guardamos el archivo en la ruta
        xmlDoc.Save(path);
    }

    public void ReadXML(string filename) 
    {
        // Mapa que contiene los mapas de los textos de cada idioma usando el propio idioma como clave
        //Dictionary<uint, Dictionary<uint, string>> ret = new Dictionary<uint, Dictionary<uint, string>>();
        //(Dictionary<uint, Dictionary<uint, string>> ret = _core.GetStringMap();

        //if(langNames == null)
        //    langNames = new List<string>();

        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");

        //creo los idiomas con sus tablas dentro del diccionario
        /*for(int i = 0; i < langNames.Count ; i++)
        {
            if(!transLang.ContainsKey(langNames[i]))
                throw new ArgumentException("Inavlid Language.");

            ret.Add(transLang[langNames[i]],new Dictionary<uint, string>());
        }*/

        //Recorremos la lista de textos del XML

        for (int i = 0; i < texts.Count; i++) 
        {
            //Id del texto (sera la clave del Diccionario de LocalCore)
            uint id = uint.Parse(texts[i].Attributes["id"].Value);
            //Numero de lenguajes que tiene el texto
            int numLang = texts[i].ChildNodes.Count;

            //Debug.Log(numLang);

            //Recorremos los hijos
            for (int j = 0; j < numLang; j++)
            {
                //Nodo hijo
                XmlNode lang = texts[i].ChildNodes[j];
                //Cambiamos el idioma del localCore y anadimos traduccion al Diccionarioç

                //Si el idoma no existe en la configuracion lo creo en el mapa y la lista de nombres sin configuracion (default)
                /*if (!ret.ContainsKey(transLang[lang.Name]))
                {
                    throw new ArgumentException("Inavlid Language.");
                }*/
                    
                //introduzco el texto en el idioma correspondiente con su id
                //ret[transLang[lang.Name]].Add(id,lang.InnerText);

                _core.SetLine(id, transLang[lang.Name], lang.InnerText);
            }
        }

        //foreach (var item in ret)
        //{
        //    UnityEngine.Debug.Log("Idioma: " + item.Key);

        //    foreach (var values in item.Value)
        //    {
        //        UnityEngine.Debug.Log(
        //            "ID: " + values.Key +
        //            " -> " + values.Value);
        //    }
        //}
    }


    //public Dictionary<uint, XmlNode> ReadXMLLanguage(string filename, List<string> langNames)
    public void ReadXMLLanguage(string filename)
    {
        Dictionary<uint, XmlNode> ret = new Dictionary<uint, XmlNode>();

        //if(langNames == null)
        //    langNames = new List<string>();
        
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con lenguaje 
        XmlNodeList texts = xmlDoc.GetElementsByTagName("Lenguaje");

        //Debug.Log(texts.Count);
        //Limpiamos primero el orden de los idiomas leidos en el XML
        languagesOrder.Clear();

        //Dictionary<uint, Dictionary<uint, string>> sM = _core.GetStringMap();

        foreach (XmlNode node in texts)
        {
            //Id del lenguaje
            uint id = uint.Parse(node.Attributes["id"].Value);

            //añadimos a mapas como lenguajes haya
            //sM.Add(id, new Dictionary<uint, string>());
            _core.AddNewLanguage(id);

            //Nombre del Idioma (etiqueta Lenguaje)
            string langName = node.ChildNodes.Item(0).InnerText;

            //Map con clave el nombre del idioma y el id correspondiente
            transLang.Add(langName,id);

            //Nombre del lenguaje
            //langNames.Add(langName);
            languagesOrder.Add(langName);

            //Metemos el lenguaje devuelto con los idiomas y sus parametros
            ret[id] = node;
            //Debug.Log(langName);
        }
        //devolvemos un mapa con los idiomas y sus parametros

        _core.SetLanguageConfig(ret);
        //return ret;
    }




    
    public void WriteVariablesToXML(string path, string key, string value)
    {
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();

        if (File.Exists(path))
        {
            // El fichero ya existe → cargarlo
            xmlDoc.Load(path);
        }
        else
        {
            // No existe → crear estructura nueva
            XmlDeclaration declaration =
                xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlElement root = xmlDoc.CreateElement("Variables");

            xmlDoc.AppendChild(declaration);

            xmlDoc.AppendChild(root);









            xmlDoc.Save(path);
        }


    }


}
