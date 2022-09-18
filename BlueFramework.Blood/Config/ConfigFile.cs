using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace BlueFramework.Blood.Config
{
    /// <summary>
    /// entity config setting
    /// </summary>
    public class ConfigFile
    {
        private string directoryPath;

        /// <summary>
        /// init sqlMappers's config files
        /// </summary>
        public ConfigFile()
        {
            directoryPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/sqlMappers");
        }

        /// <summary>
        /// init sqlMappers's config files
        /// </summary>
        /// <param name="sqlMapperPath">sql config xml's path</param>
        public ConfigFile(string sqlMapperPath)
        {
            directoryPath = sqlMapperPath;
        }

        /// <summary>
        /// load sqlMappers's config files
        /// </summary>
        /// <returns></returns>
        public List<EntityConfig> LoadConfigs()
        {
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            FileInfo[] files = dir.GetFiles("*.xml");
            List<EntityConfig> dataSource = new List<EntityConfig>();
            for (int i = 0; i < files.Length; i++)
            {
                List<EntityConfig> items = LoadConfigs(files[i].FullName);
                dataSource.AddRange(items);
            }
            return dataSource;
        }

        /// <summary>
        /// load configs from xmlfile
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public List<EntityConfig> LoadConfigs(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlNode root = doc.SelectSingleNode("mapper");
            string nameSpace = root.Attributes["namespace"].Value;
            List<EntityConfig> configs = new List<EntityConfig>();
            // load select,insert,update configs
            loadSelectConfigs(root, configs);
            loadInsertConfigs(root, configs);
            loadUpdateConfigs(root, configs);
            loadDeleteConfigs(root, configs);
            // update full id
            foreach (EntityConfig config in configs)
            {
                config.Id = string.Format("{0}.{1}", nameSpace,config.Id);
            }
            return configs;
        }

        private void loadSelectConfigs(XmlNode root,List<EntityConfig> configs)
        {
            XmlNodeList nodes = root.SelectNodes("select");
            foreach (XmlNode node in nodes)
            {
                SelectConfig selectCfg = new SelectConfig();
                selectCfg.Id = node.Attributes["id"].Value;
                if(node.Attributes["parameterType"]!=null)
                    selectCfg.InputParameterType = node.Attributes["parameterType"].Value;
                if(node.Attributes["resultType"]!=null)
                    selectCfg.OutputParameterType = node.Attributes["resultType"].Value;
                selectCfg.Sql = node.InnerText;
                configs.Add(selectCfg);
            }
        }

        private void loadInsertConfigs(XmlNode root, List<EntityConfig> configs)
        {
            XmlNodeList nodes = root.SelectNodes("insert");
            foreach (XmlNode node in nodes)
            {
                InsertConfig insertCfg = new InsertConfig();
                insertCfg.Id = node.Attributes["id"].Value;
                if(node.Attributes["parameterType"]!=null)
                    insertCfg.InputParameterType = node.Attributes["parameterType"].Value;
                XmlNode keyNode = node.SelectSingleNode("selectKey");
                if (keyNode != null)
                {
                    insertCfg.KeyProperty = keyNode.Attributes["keyProperty"].Value;
                    insertCfg.KeyPropertySql = keyNode.InnerText;
                    switch (keyNode.Attributes["order"].Value.ToUpper())
                    {
                        case "BEFOR":
                            insertCfg.KeyMadeOrder = IdentityMadeOrder.Befor;
                            break;
                        case "INSERTING":
                            insertCfg.KeyMadeOrder = IdentityMadeOrder.Inserting;
                            break;
                        case "AFTER":
                            insertCfg.KeyMadeOrder = IdentityMadeOrder.After;
                            break;
                        default:
                            insertCfg.KeyMadeOrder = IdentityMadeOrder.Undefine;
                            break;
                    }
                }
                foreach (XmlNode child in node.ChildNodes)
                {
                    if(child.NodeType== XmlNodeType.Text)
                    {
                        insertCfg.Sql = child.InnerText;
                        break;
                    }
                }
                configs.Add(insertCfg);
            }
        }

        private void loadUpdateConfigs(XmlNode root, List<EntityConfig> configs)
        {
            XmlNodeList nodes = root.SelectNodes("update");
            foreach (XmlNode node in nodes)
            {
                UpdateConfig updateCfg = new UpdateConfig();
                updateCfg.Id = node.Attributes["id"].Value;
                if(node.Attributes["parameterType"]!=null)
                    updateCfg.InputParameterType = node.Attributes["parameterType"].Value;
                //selectCfg.OutputParameterType = node.Attributes["resultType"].Value;
                updateCfg.Sql = node.InnerText;
                configs.Add(updateCfg);
            }
        }

        private void loadDeleteConfigs(XmlNode root, List<EntityConfig> configs)
        {
            XmlNodeList nodes = root.SelectNodes("delete");
            foreach (XmlNode node in nodes)
            {
                DeleteConfig deleteCfg = new DeleteConfig();
                deleteCfg.Id = node.Attributes["id"].Value;
                if (node.Attributes["parameterType"] != null)
                    deleteCfg.InputParameterType = node.Attributes["parameterType"].Value;
                //selectCfg.OutputParameterType = node.Attributes["resultType"].Value;
                deleteCfg.Sql = node.InnerText;
                configs.Add(deleteCfg);
            }
        }
    }
}
