
using System;
using System.IO;
using System.Xml;

using GraphDB.Contract.Enum;
using GraphDB.Contract.Serial;

namespace GraphDB.IO
{
    public class XMLStrategy:IIoStrategy//XML文件读写算法
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

        //XMLStrategy算法读取函数
        public XmlElement ReadFile(out ErrorCode err)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                var stream = new FileStream(myFilePath, FileMode.Open);
                doc.Load(stream);               //从流文件读入xml文档
                stream.Close();
            }
            catch (Exception)
            {
                err = ErrorCode.OpenFileFailed;
                return null;
            }
            //创建网络
            XmlElement graph =(XmlElement)doc.FirstChild;
            err = ErrorCode.NoError;
            return graph;
        }

        //XMLStrategy算法保存函数
        public void SaveFile(XmlDocument doc, out ErrorCode err)
        {
            try
            {
                var stream = new FileStream(myFilePath, FileMode.Create);
                doc.Save(stream);               //保存xml文档到流
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
