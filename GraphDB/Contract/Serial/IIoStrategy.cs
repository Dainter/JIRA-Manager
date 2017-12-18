using System.Xml;

using GraphDB.Contract.Enum;

namespace GraphDB.Contract.Serial
{
    public interface IIoStrategy//�ļ���д�㷨�ӿ�
    {
        string Path { get; set; }
        XmlElement ReadFile( out ErrorCode err );
        void SaveFile(XmlDocument doc, out ErrorCode err);

    }
}
