using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BlueFramework.Common
{
    public static class XmlUtils
    {

        public static string GetInnerText(string filePath, string elementId)
        {
            XmlNode node = GetNode(filePath, elementId);
            return node.InnerText;
        }

        public static XmlNode GetNode(string filePath, string elementId)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            XmlNode root = document.DocumentElement;

            XmlNode node = GetNode(root, elementId);
            return node;
        }

        public static XmlNode GetNode(XmlNode parent, string elementId)
        {
            if (parent.NodeType != XmlNodeType.Element)
                return null;
            if (parent.Attributes["id"]!=null && parent.Attributes["id"].Value == elementId)
                return parent;
            foreach(XmlNode child in parent.ChildNodes)
            {
                XmlNode node = GetNode(child, elementId);
                if (node != null)
                    return node;
            }
            return null;
        }
    }
}
