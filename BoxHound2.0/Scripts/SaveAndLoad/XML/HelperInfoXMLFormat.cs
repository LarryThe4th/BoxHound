using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using BoxHound.Manager;

namespace BoxHound
{
    public class HelperInfoXMLFormat
    {
        public HelperInformationManger.Category InformationCategory
        {
            get { return m_Category; }
            set { m_Category = value; }
        }
        [XmlAttribute("Category")]
        private HelperInformationManger.Category m_Category;

        public string InformationHeaderText
        {
            get
            {
                return m_HeaderText;
            }

            set
            {
                m_HeaderText = value;
            }
        }
        [XmlElement("Header")]
        private string m_HeaderText;

        public string InformationText
        {
            get
            {
                return m_Information;
            }

            set
            {
                m_Information = value;
            }
        }
        [XmlElement("Information")]
        private string m_Information;
    }
}
