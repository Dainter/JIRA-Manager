
using System;
using System.IO;
using System.Xml;

using GraphDB.Contract.Enum;
using GraphDB.Contract.Serial;

namespace GraphDB.IO
{
    public class XMLStrategy:IIoStrategy//XML�ļ���д�㷨
    {
        string myFilePath;

        public string Path
        {
            get
            {
                return myFilePath;
            }
            set
            {
                myFilePath = value;
            }
        }

        public XMLStrategy(string sPath)
        {
            myFilePath = sPath;
        }

        //XMLStrategy�㷨��ȡ����
        public XmlElement ReadFile(out ErrorCode err)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                var stream = new FileStream(myFilePath, FileMode.Open);
                doc.Load(stream);               //�����ļ�����xml�ĵ�
                stream.Close();
            }
            catch (Exception)
            {
                err = ErrorCode.OpenFileFailed;
                return null;
            }
            //��������
            XmlElement graph =(XmlElement)doc.FirstChild;
            err = ErrorCode.NoError;
            return graph;
        }

        //XMLStrategy�㷨���溯��
        public void SaveFile(XmlDocument doc, out ErrorCode err)
        {
            try
            {
                var stream = new FileStream(myFilePath, FileMode.Create);
                doc.Save(stream);               //����xml�ĵ�����
                stream.Close();
            }
            catch (Exception)
            {
                err = ErrorCode.SaveFileFailed;
                return;
            }
            err = ErrorCode.NoError;
        }
    }
}
