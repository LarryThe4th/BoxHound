using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace BoxHound
{

    /// <summary>
    /// Handles saving and loading XML data file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SaveAndLoad<T>
    {
        private XMLDataContainer<T> m_DataContainer = new XMLDataContainer<T>();

        public delegate void OnSerializationAction();
        public static event OnSerializationAction BeforSaveDataEvent;
        public static void OnBeforSaveGame()
        {
            if (BeforSaveDataEvent != null) BeforSaveDataEvent();
        }

        public void Save(string path)
        {
            ClearDataContainer();

            if (!File.Exists((Application.temporaryCachePath + path)))
            {
                File.WriteAllText(Application.temporaryCachePath + path, "");
            }

            // Befor saving the game, call all the CharacterModel OnSaveData funcion.
            OnBeforSaveGame();
            SaveDataIntoXML(Application.temporaryCachePath + path, m_DataContainer);
        }

        public void Save(string path, XMLDataContainer<T> data)
        {
            ClearDataContainer();

            if (!File.Exists((Application.temporaryCachePath + path)))
            {
                File.WriteAllText(Application.temporaryCachePath + path, "");
            }

            // Befor saving the game, call all the CharacterModel OnSaveData funcion.
            OnBeforSaveGame();
            SaveDataIntoXML(Application.temporaryCachePath + path, data);
        }

        public void SaveDataIntoXML(string path, XMLDataContainer<T> data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDataContainer<T>));

            FileStream stream = new FileStream(path, FileMode.Truncate);

            StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);

            serializer.Serialize(streamWriter, data);

            stream.Close();
        }

        public XMLDataContainer<T> LoadSaveData(string path)
        {
            ClearDataContainer();
            FileStream stream = new FileStream(path, FileMode.Truncate);
            m_DataContainer = LoadXMLData(stream.ToString());
            return m_DataContainer;
        }

        /// <summary>
        /// Load XML file form resouce folder.
        /// </summary>
        /// <param name="path">The path to the XML file</param>
        /// <returns>Returns a data container contains all the data in XML</returns>
        public XMLDataContainer<T> LoadInitData(string path)
        {
            ClearDataContainer();
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            m_DataContainer = LoadXMLData(textAsset.text);
            return m_DataContainer;
        }

        public void AddData(T data)
        {
            m_DataContainer.DataList.Add(data);
        }

        public void ClearDataContainer()
        {
            m_DataContainer.DataList.Clear();
        }

        private XMLDataContainer<T> LoadXMLData(string xmlContents)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDataContainer<T>));

            StringReader stringReader = new StringReader(xmlContents);

            XmlReader reader = XmlReader.Create(stringReader);

            XMLDataContainer<T> data = serializer.Deserialize(reader) as XMLDataContainer<T>;

            return data;
        }
    }
}
