using System.Xml;

namespace GraphDB.Utility
{
    //XmlElement功能扩展方法
    public static class XmlElementExtension
    {
        //工具函数，从xml节点中读取某个标签的InnerText
        public static string GetText(this XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return "";
            }
            //遍历子节点列表
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//查找和指定内容相同的标签，返回其Innner Text
                    return xNode.InnerText;
                }
            }
            return "";
        }

        //工具函数，从xml节点中返回某个标签的标识的节点
        public static XmlElement GetNode(this XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return null;
            }
            //遍历子节点列表
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//查找和指定内容相同的标签，返回其Innner Text
                    return xNode;
                }
            }
            return null;
        }

        //工具函数，从xml节点中读取某个特性的取值
        public static string GetAttribute(this XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return "";
            }
            //遍历子节点列表
            foreach (XmlAttribute xAttr in curNode.Attributes)
            {
                if (xAttr.Name == sLabel)
                {//查找和指定内容相同的标签，返回其Innner Text
                    return xAttr.Value;
                }
            }
            return "";
        }
    }
}