/********************************************************************
	created:	2018/02/28 0:24:09
	author:		chens
	purpose:	
*********************************************************************/
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Editor.GDK.files.Parser
{
    class FileBase : ScriptableObject, ISerializationCallbackReceiver
    {
        private FileInfo fileInfo;
        private FileStream stream;
        private bool autoParse;
        private string fileString;
        private string _path;

        public void Init(string path, bool isAutoParse = false)
        {
            _path = path;
            fileInfo = new FileInfo(path);
            stream = File.Open(path, FileMode.Open, FileAccess.Read);
            autoParse = isAutoParse;
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            fileString = sr.ReadToEnd();
            if (isAutoParse) parse();
            stream.Close();
            stream = null;
        }
        public void parse()
        {
            try
            {
                startParse();
            }
            catch (System.Exception e )
            {
                Debug.LogError("文件发生解析错误,检查文件语法是否正确:" + Path + " _ " + e.ToString());
                //stream.Close();
            }

        }
        virtual protected void startParse()
        {

        }

        public virtual void Save(string fileStr = "")
        {
            if(string.IsNullOrEmpty(fileString))
            {
                File.WriteAllText(Path, fileString);//todo fileString是只读的
            }
            else
            {
                File.WriteAllText(Path, fileStr);
            }
            Reload();
        }
        

        public FileInfo FileInfo
        {
            get
            {
                return fileInfo;
            }
        }

        public FileStream Stream
        {
            get
            {
                return stream;
            }
        }

        public bool AutoParse
        {
            get
            {
                return autoParse;
            }

        }

        public string FileString
        {
            get
            {
                return fileString;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }
        public virtual void Reload()
        {
            Init(Path, AutoParse);
        }

        public void OnBeforeSerialize()
        {
            //if (fileInfo != null)
            //{
            //    Log.Info(fileInfo.Name + "文件被序列化");
            //}
            //else
            //{
            //    Log.Info("fileInfo丢失，文件序列化失败");
            //}
            Serialize();
        }
        public virtual void Serialize()
        {

        }

        public void OnAfterDeserialize()
        {
            //if (fileInfo != null)
            //{
            //    Log.Info(fileInfo.Name + "文件被反序列化");
            //}
            //else
            //{
            //    Log.Info("文件反序列化失败");
            //}
            Deserialize();
        }
        public virtual void Deserialize()
        {

        }

    }
}
