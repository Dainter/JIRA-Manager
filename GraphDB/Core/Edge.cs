using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Edge: IEdge//图数据库连边类：负责存储网络连边信息
    {
        //成员变量
        private INode myFromNode;//连边起点
        private INode myToNode;//连边终点
        private string myFromGuid;//连边起点GUID
        private string myToGuid;//连边终点GUID
        private readonly string myAttribute;//连边类型
        private string myValue;//连边取值

        //属性//////////////////////////
        public INode From
        {
            get
            {
                return myFromNode;
            }
            set
            {
                myFromNode = value;
                if( value != null )
                {
                    myFromGuid = value.Guid;
                }
                else
                {
                    myFromGuid = "";
                }
            }
        }
        public INode To
        {
            get
            {
                return myToNode;
            }
            set
            {
                myToNode = value;
                if (value != null)
                {
                    myToGuid = value.Guid;
                }
                else
                {
                    myToGuid = "";
                }
                
            }
        }
        [XmlSerializable]
        public string FromGuid
        {
            get
            {
                return myFromGuid;
            }
            set
            {
                myFromGuid = value;
            }
        }
        [XmlSerializable]
        public string ToGuid
        {
            get
            {
                return myToGuid;
            }
            set
            {
                myToGuid = value;
            }
        }
        [XmlSerializable]
        public string Attribute => myAttribute;
        [XmlSerializable]
        public string Value
        {
            get
            {
                return myValue;
            }
            set
            {
                myValue = value;
            }
        }

        //方法/////////////////////////
        //连边类Edge构造函数
        public Edge(string newAttribute, string newValue = "1")//构造函数 对三个变量进行赋值
        {
            myFromGuid = "";
            myToGuid = "";
            myAttribute = newAttribute;
            myValue = newValue;
        }
        //连边类Edge构造函数
        public Edge( XmlElement xNode)//构造函数 对三个变量进行赋值
        {
            //取出制定标签的Inner Text
            myFromGuid = xNode.GetText("FromGuid");
            myToGuid = xNode.GetText("ToGuid");
            myAttribute = xNode.GetText("Attribute");
            myValue = xNode.GetText("Value");
        }
    }
}
