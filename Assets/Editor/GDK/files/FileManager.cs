/********************************************************************
	created:	2018/02/24 23:37:15
	author:		chens
	purpose:    文件库.所有使用此类获得的文件都会放入文件库,方便后续调用.
    此库会反复的被序列化和反序列化，所以使用此库之前应该确认是否正在进行序列化和反序列化。
    一般不再构造函数中或者声明代码中引用此库的单例都不会出现问题。
*********************************************************************/
using Assets.Editor.GDK.common;
using Assets.Editor.GDK.files.Parser;
using Assets.Editor.GDK.files.Parser.lua;
using Assets.Editor.GDK.files.Parser.proto;
using Assets.Editor.GDK.files.Parser.XLSX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

//Log.Info(EditorApplication.applicationPath);//C:/Program Files/Unity/Editor/Unity.exe
//Log.Info(EditorApplication.applicationContentsPath);//C:/Program Files/Unity/Editor/Data
//Log.Info(Application.dataPath);//C:/workspaceUnity/sun/client/SUNClient/Assets
//Log.Info(Application.persistentDataPath);//C:/Users/chens/AppData/LocalLow/DaxiaStudio/天使纪元
//Log.Info(Application.streamingAssetsPath);//C:/workspaceUnity/sun/client/SUNClient/Assets/StreamingAssets
//Log.Info(Application.temporaryCachePath);//C:/Users/chens/AppData/Local/Temp/DaxiaStudio/天使纪元

namespace Assets.Editor.GDK.files
{
    class FileManager : ScriptableObject
    {
        private static FileManager _instance;

        public static FileManager getInstance()
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<FileManager>().FirstOrDefault();
            }
            if (_instance == null)
            {
                _instance = CreateInstance<FileManager>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }
            return _instance;
        }
        public static string BrowseFile(string title, string extension = "*", string directory = "")
        {//不用.xx 直接传xx后缀名就可以
            if(directory == "")
            {
                directory = Application.dataPath + "/Luas";
            }
            return EditorUtility.OpenFilePanel(title, directory, extension);
            //filePath =  Directory.GetFiles(path);
            //foreach (string filePath in path)
            //{
            //    if (filePath.EndsWith(".lua"))
            //    {
            //        luaFilePath = filePath;
            //    }
            //}
        }
        public static string BrowseDir(string title, string extension = "", string directory = "")
        {
            return EditorUtility.OpenFolderPanel(title, directory, extension);
        }
        public static List<string> GetFileUrls(string dir,string extension = "", bool recursive = false)
        {
            List<string> urlList = new List<string>();
            
            if(Directory.Exists(dir))
            {
                var fileList = Directory.GetFiles(dir);
                foreach (var item in fileList)
                {
                    if(extension == "")
                    {
                        urlList.Add(item);
                    }else if (item.EndsWith(extension))
                    {
                        urlList.Add(item);
                    }
                }
                if(recursive)
                {
                    var dirList = Directory.GetDirectories(dir);
                    foreach (var item in dirList)
                    {
                        urlList.AddRange(GetFileUrls(item, extension, recursive));
                    }
                }
            }
            return urlList;
        }

        private Dictionary<string,Type> parserDic = new Dictionary<string, Type>();
        //字典是无法被序列化的，所以使用两个相互关联的数组实现。fileUrlList[0]的文件地址就是FileList[0]这个对象
        public List<string> fileUrlList = new List<string>();
        public List<FileBase> fileList = new List<FileBase>();

        public FileManager()
        {
            parserDic.Add(".proto", typeof(ParserProto));
            parserDic.Add(".xlsx", typeof(ParserXLSX));
            parserDic.Add(".lua", typeof(LuaFileParser));
            //游戏Asset目录下文件用这个方法侦听性能比较好。
            FileListener.OnFileChanged = onFileChanged;
        }

        private void onFileChanged(string[] importAssets, string[] deletedAssets, string[] movedAssets, string[] moveFromAssetsPath)
        {//Assets/Luas/xxxxx.lua
            //Debug.Log(importAssets.Length);
            //foreach (var item in importAssets)
            //{
            //    Debug.Log(item);
            //}
            //Debug.Log(deletedAssets.Length);
            //Debug.Log(movedAssets.Length);
            //Debug.Log(moveFromAssetsPath.Length);
        }
        public void OnEnable()
        {
            //先尝试实现侦听式，当需要Unity关闭时候依然缓存文件时候在考虑使用哈希对比，毕竟那样开启unity时候会比较慢
            //这东西性能不太好，看起来像是轮训来实现的,所以只侦听文件夹变化
            //如果文件夹发生变化再去重新检查已经加载并解析的文件是否需要重新处理
            initWatcher(Application.dataPath + "/../../../../config/protos/client", "*.*");
        }

        void initWatcher(string path,string filter)
		{//重命名不是触发的renamed 是一次Deleted和一次Created 这样会导致解析文件被删除，先不处理了。
			var fileSystemWatcher = new FileSystemWatcher(path, filter);
			fileSystemWatcher.Created += new FileSystemEventHandler(onChanged);
			fileSystemWatcher.Changed += new FileSystemEventHandler(onChanged);
			fileSystemWatcher.Deleted += new FileSystemEventHandler(onChanged);
			//fileSystemWatcher.Renamed += new RenamedEventHandler(onChanged);
			fileSystemWatcher.IncludeSubdirectories = true;//不能包含子目录。卡
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		private void onChanged(object sender, FileSystemEventArgs e)
		{
            Debug.Log("发现文件变化：" + e.FullPath + " 变化方式：" + e.ChangeType);
            var index = fileUrlList.IndexOf(e.FullPath);
            if (index > -1)
			{
                var file = fileList[index];
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    fileList.RemoveAt(index);
                    fileUrlList.RemoveAt(index);
                    Destroy(file);
                }
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    getFile(e.FullPath, file.AutoParse, true);
                }
                
            }
		}
		public FileBase getFile(string path, bool autoParse = false ,bool forceReload = false ,bool cache = true)
		{
            if (fileUrlList.Contains(path) == true && forceReload == false)
            {
                FileBase f = fileList[fileUrlList.IndexOf(path)];
                if (autoParse) f.parse();
                return f;
            }
            var info = new FileInfo(path);
            if (info.Exists == false)
            {
                Debug.LogError("无法找到文件:" + path);
                return null;
            }
			var ext = info.Extension;
            FileBase file;
            if (parserDic.ContainsKey(ext))
            {
                file = ScriptableObject.CreateInstance(parserDic[ext]) as FileBase;

            }
            else
            {
                file = ScriptableObject.CreateInstance<FileBase>();
            }
            //EditorUtility.SetDirty(file);
            file.Init(path, autoParse);
            file.hideFlags = HideFlags.HideAndDontSave;
            if (fileUrlList.Contains(path) == true)
            {//测试时候,判断下每次都更新下。
                fileList[fileUrlList.IndexOf(path)] = file;
            }else if(cache)
            {
                fileUrlList.Add(path);
                fileList.Add(file);
            }
            return file;
        }

    }
    class FileListener:AssetPostprocessor
    {
        internal static FileChanged OnFileChanged;
        public static void OnPostprocessAllAssets(string [] importAssets , string[] deletedAssets , string[] movedAssets, string[]moveFromAssetsPath)
        {
            if(OnFileChanged != null)
            {
                OnFileChanged(importAssets, deletedAssets, movedAssets, moveFromAssetsPath);
            }
        }
        public delegate void FileChanged(string[] importAssets, string[] deletedAssets, string[] movedAssets, string[] moveFromAssetsPath);
    }



}
