using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Edge: IEdge//ͼ���ݿ������ࣺ����洢����������Ϣ
    {
        //��Ա����
        private INode myFromNode;//�������
        private INode myToNode;//�����յ�
        private string myFromGuid;//�������GUID
        private string myToGuid;//�����յ�GUID
        private readonly string myAttribute;//��������
        private string myValue;//����ȡֵ

        //����//////////////////////////
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

        //����/////////////////////////
        //������Edge���캯��
        public Edge(string newAttribute, string newValue = "1")//���캯�� �������������и�ֵ
        {
            myFromGuid = "";
            myToGuid = "";
            myAttribute = newAttribute;
            myValue = newValue;
        }
        //������Edge���캯��
        public Edge( XmlElement xNode)//���캯�� �������������и�ֵ
        {
            //ȡ���ƶ���ǩ��Inner Text
            myFromGuid = xNode.GetText("FromGuid");
            myToGuid = xNode.GetText("ToGuid");
            myAttribute = xNode.GetText("Attribute");
            myValue = xNode.GetText("Value");
        }
    }
}
