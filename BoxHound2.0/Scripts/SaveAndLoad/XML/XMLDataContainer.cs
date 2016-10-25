using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoxHound
{
    [XmlRoot("DataCollection")]
    public class XMLDataContainer<T>
    {
        [XmlArray("DataList"), XmlArrayItem("Data")]
        public List<T> DataList = new List<T>();
    }
}
