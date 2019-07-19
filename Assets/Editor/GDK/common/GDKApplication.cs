/********************************************************************
	created:	2018/01/31 15:35:57
	filename: 	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\common\GDKApplication.cs
	file path:	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\common
	author:		chens
	
	purpose:	
*********************************************************************/
using Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK.common
{
    [InitializeOnLoad]
    class GDKApplication
    {
        public const string PROJECT_TW = "PROJECT_TW";
        public const string PROJECT_KR = "PROJECT_KR";
        public const string PROJECT_WORLD = "PROJECT_WORLD";
        public const string PROJECT_CN = "PROJECT_CN";
        public const string PROJECT_WORLD_SERVER = "PROJECT_WORLD_SERVER";
        public static string configIni = Application.dataPath + "/Editor/GDK/GDKConfig.ini";
        static GDKApplication()
        {
        }
        public static void copyTo(string src,string dst)
        {
            src = src.Replace("\\", "/");
            dst = dst.Replace("\\", "/");
            var pathArr = dst.Split(new char[1] { '/' });
            if (pathArr == null || pathArr.Length < 1 || pathArr[0].ToString().Contains(":") == false)
            {
                UnityEngine.Debug.LogError("复制的目标路径应该为完整路径如C:/xxx/xxx/xxx,路径不合法：" + dst);
            }
            var rootPath = "";
            for (int i = 0; i < pathArr.Length; i++)
            {
                if(i == 0)
                {
                    rootPath = pathArr[i];
                }else if(i != pathArr.Length - 1)
                {
                    rootPath += "/"+ pathArr[i];
                    if (Directory.Exists(rootPath) == false) Directory.CreateDirectory(rootPath);
                }
            }
            if (File.Exists(src))
            {
                UnityEngine.Debug.Log("复制文件:" + src + "\n--->" + dst);
                File.Copy(src, dst, true);
            }
            else if (Directory.Exists(src))
            {
                var files = Directory.GetFiles(src);
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = files[i].Replace(src, "");
                    copyTo(files[i], dst + fileName);
                }

                var dir = Directory.GetDirectories(src);
                for (int i = 0; i < dir.Length; i++)
                {
                    var dirName = dir[i].Replace(src, "");
                    copyTo(dir[i], dst + dirName);
                }
            }
            else
            {
            }

           
        }
        public static List<string[]> ReadEXCEL(string path)
        {
            List<string[]> rowList = new List<string[]>();
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            string[] lineArr = readLine(excelReader);
            while (lineArr != null)
            {
                rowList.Add(lineArr);
                lineArr = readLine(excelReader);
            }
            excelReader.Dispose();
            return rowList;
        }
        private static string[] readLine(IExcelDataReader excelReader)
        {
            if (excelReader.Read() == false) return null;
            string[] temp = new string[excelReader.FieldCount];
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
                temp[i] = value;
            }
            return temp;
        }
        public static void exucteCMD(string cmd, out string StandardOutput , out string StandardError,bool CreateNoWindow = true)
        {
            Process p = CMDProcess.CreateAInstance();
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine("exit");
            StandardOutput = p.StandardOutput.ReadToEnd();
            StandardError = p.StandardError.ReadToEnd();
            p.WaitForExit();
            p.StandardError.Close();//关闭流
            p.StandardOutput.Close();
            p.Close();
        }

        internal static void writeNetworkFilter(string value)
        {
            var str = Regex.Replace(value, "[^0-9,]*", "");
            var arr = str.Split(',');
            List<int> list = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != "" && list.Contains(Convert.ToInt32(arr[i])) == false)
                {
                    list.Add(Convert.ToInt32(arr[i]));
                }
            }
            list.Sort();
            str = string.Join(",", list.ConvertAll(x => x.ToString()).ToArray());
            WritePrivateProfileString("network", "filter", str, configIni);
        }
        internal static string readNetworkFilter()
        {
            StringBuilder temp = new StringBuilder(1000);
            GetPrivateProfileString("network", "filter", "", temp, 1000, configIni);
            return temp.ToString();
        }
        public static void writeLuaPath(string value)
        {
            WritePrivateProfileString("lua", "luapath", value, configIni);
        }
        public static string readTestLuaPath()
        {
            StringBuilder temp = new StringBuilder(1000);
            GetPrivateProfileString("lua", "luapath", "", temp, 1000, configIni);
            return temp.ToString();
        }

        public static void writeLanguageString(string value)
        {
            WritePrivateProfileString("language", "str", value, configIni);
        }
        public static string readLanguageString()
        {
            StringBuilder temp = new StringBuilder(1000);
            GetPrivateProfileString("language", "str", "", temp, 100000, configIni);
            return temp.ToString();
        }
        public static string getProjectPath(string project)
        {
            StringBuilder temp = new StringBuilder(1000);
            GetPrivateProfileString("ProjectPath", project, "", temp, 100000, configIni);
            return temp.ToString();
        }
        public static void setProjectPath(string project,string value)
        {
            WritePrivateProfileString("ProjectPath", project, value, configIni);
        }

        //返回值非0表示成功 0表示失败。
        [DllImport("kernel32.dll")]
        public static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        //返回字符串的真实长度？defalut 是读取不到时候的默认值是什么
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string section, string key, string defalut,
            StringBuilder ipReturnedString,
            int nSize,
            string filePath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileInt(string section, string key, int defalut, string filePath);
    }/// <summary>
     /// 中文字符工具类
     /// </summary>
    public static class ChineseStringUtility
    {
        private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        /// <summary>
        /// 讲字符转换为繁体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToTraditional(string source)
        {
            String target = new String(' ', source.Length);
            LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
    }
}
