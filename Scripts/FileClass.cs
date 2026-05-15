using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEditor;
using TMPro;

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
        foreach(KeyValuePair<uint, string[]> pair in LocalCore.Instance().GetLines)
        {
            //id es el ID del texto el stringTable del localCore
            uint id = pair.Key;
            //Cantidad de traducciones que tiene el texto ID
            string[]texts = pair.Value;

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

    public void ReadXML(string filename) 
    {
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");

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
                //Cambiamos el idioma del localCore y anadimos traduccion al Diccionario
                LocalCore.Instance().ChangeLang(j);
                LocalCore.Instance().SetLine(id, lang.InnerText);
            }

            //Reseteamos el lenguaje del LocalCore (incio de los array de las claves del Diccionario)
            LocalCore.Instance().ChangeLang(0);
        }
    }


    public Dictionary<string, XmlNode> ReadXMLLanguage(string filename, List<string> lagNames)
    {
        Dictionary<string, XmlNode> ret = new Dictionary<string, XmlNode>();
        List<string> langNames = new List<string>();
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("Lenguaje");

        Debug.Log(texts.Count);

        //ret = new string[texts.Count];

        //Limpiamos primero el orden de los idiomas leidos en el XML
        languagesOrder.Clear();

        foreach (XmlNode node in texts)
        {
            //Id del texto (sera la clave del Diccionario de LocalCore)
            uint id = uint.Parse(node.Attributes["id"].Value);

            string langName = node.ChildNodes.Item(0).InnerText;
            langNames.Add(langName);
            languagesOrder.Add(langName);

            ret[langName] = node;
            //ret = node.Attributes["Nombre"].InnerText;

            Debug.Log(node.ChildNodes.Item(0).InnerText);
        }
        return ret;

    }
}
