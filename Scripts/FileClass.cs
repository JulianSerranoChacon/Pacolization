using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Data;

public class FileClass
{
    public FileClass() {}

    //Lista de los idiomas ordenados al leer el XML de lenguajes
    private List<string> languagesOrder = new List<string>();

    public void WriteXML(string path)
    {
        //Doc XML donde vamos a guardar los datos del localCore
        XmlDocument xmlDoc = new XmlDocument();

        //Escribimos primero la cabecera del XML
        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(declaration);

        //Elemento raiz del que colgaran todos los textos
        XmlElement root = xmlDoc.CreateElement("translations");
        xmlDoc.AppendChild(root);


        //Recorremos todo el unorderedMap
        foreach(KeyValuePair<uint, Dictionary<uint, string>> pair in LocalCore.Instance().GetLines)
        {
            //id es el ID del texto el stringTable del localCore
            uint id = pair.Key;
            //Cantidad de traducciones que tiene el texto ID
            string[] texts = new string[LocalCore.Instance().getLang()];

            foreach (KeyValuePair<uint,string> s in pair.Value)
            {
                texts[pair.Key] = s.Value;
            }
            


            //Nodo del texto y seteo de su id en el XML
            XmlElement textNode = xmlDoc.CreateElement("text");
            textNode.SetAttribute("id", id.ToString());

            //Recorremos el array de los textos traducidos a los distintos idiomas
            for(int i = 0; i < texts.Length; i++)
            {
                string langName;
                //Si el indice del text[i] pertence al rango de idiomas disponibles lo ponemos dentro del
                //XML como su hijo y con la etiqueta langName correspondiente
                if(i < languagesOrder.Count)
                    langName = texts[i];
                else
                    langName = "langNotDefined_" + i;
                
                //Creamos el nodo hijo del texto
                XmlElement langNode = xmlDoc.CreateElement(langName);
                langNode.InnerText = texts[i];
                textNode.AppendChild(langNode);
            }
            root.AppendChild(textNode);
        }
        //Antes de acabar guardamos el archivo en la ruta
        xmlDoc.Save(path);
    }

    public Dictionary<uint, Dictionary<uint, string>> ReadXML(string filename, List<string> langNames) 
    {
        // Mapa que contiene los mapas de los textos de cada idioma usando el propio idioma como clave
        Dictionary<uint, Dictionary<uint, string>> ret = new Dictionary<string, Dictionary<uint, string>>();

        if(langNames == null)
            langNames = new List<string>();

        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");

        //creo los idiomas con sus tablas dentro del diccionario
        for(int i = 0; i < langNames.Count ; i++)
            ret.Add(langNames[i],new Dictionary<uint, string>());

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
                if (!ret.ContainsKey(lang.Name))
                {
                    langNames.Add(lang.Name);
                    ret.Add(lang.Name,new Dictionary<uint, string>());
                }
                    
                //introduzco el texto en el idioma correspondiente con su id
                ret[lang.Name].Add(id,lang.InnerText);
            }
        }

        foreach (var item in ret)
        {
            UnityEngine.Debug.Log("Idioma: " + item.Key);

            foreach (var values in item.Value)
            {
                UnityEngine.Debug.Log(
                    "ID: " + values.Key +
                    " -> " + values.Value);
            }
        }

        return ret;
    }


    public Dictionary<uint, XmlNode> ReadXMLLanguage(string filename, List<string> langNames)
    {
        Dictionary<uint, XmlNode> ret = new Dictionary<string, XmlNode>();

        if(langNames == null)
            langNames = new List<string>();
        
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("Lenguaje");

        UnityEngine.Debug.Log(texts.Count);

        //Limpiamos primero el orden de los idiomas leidos en el XML
        languagesOrder.Clear();

        foreach (XmlNode node in texts)
        {
            //Id del texto
            uint id = uint.Parse(node.Attributes["id"].Value);

            //Nombre del Idioma
            string langName = node.ChildNodes.Item(0).InnerText;

            //Nombre del lenguaje
            langNames.Add(langName);
            languagesOrder.Add(langName);

            //Metemos el lenguaje devuelto con los idiomas y sus parametros
            ret[id] = node;
            UnityEngine.Debug.Log(langName);
        }
        //devolvemos un mapa con los idiomas y sus parametros
        return ret;
    }
}
