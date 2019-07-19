using Assets.Editor.GDK.common;
using Assets.Editor.GDK.files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
    class LuaExcuteManager : ScriptableObject
    {
        private static LuaExcuteManager _instance;

        public static LuaExcuteManager getInstance()
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<LuaExcuteManager>().FirstOrDefault();
            }
            if (_instance == null)
            {
                _instance = CreateInstance<LuaExcuteManager>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }
            return _instance;
        }
        private string inputLuaText = "";
        private string luaFilePath = "";
        private CommonWindow window;

        public void show()
        {
            luaFilePath = GDKApplication.readTestLuaPath();
            window = CommonWindow.show(() =>
            {
                GUILayout.Label("游戏在运行过程中可以使用此功能。");

                inputLuaText = GUILayout.TextArea(inputLuaText, GUILayout.MinWidth(300));
                if (GUILayout.Button("执行lua文本，麻痹不支持粘贴"))
                {
                    try
                    {
                        //LuaScriptMgr.Instance.lua.DoString(inputLuaText);
                    } catch (Exception e)
                    {
                        Debug.LogErrorFormat("运行时候才可以执行代码:" + e.ToString());
                    }
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("文件路径");
                GUILayout.Label(luaFilePath);
                if (GUILayout.Button("浏览"))
                {
                    luaFilePath = FileManager.BrowseFile("!!!!!!!!!!!!!找到你需要执行的.lua文件!!!!!!!!!!!!!!!!!","*", luaFilePath);
                    GDKApplication.writeLuaPath(luaFilePath);

                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("执行lua文件"))
                {
                    var file = FileManager.getInstance().getFile(luaFilePath, false, true);
                    try
                    {
                        //LuaScriptMgr.Instance.lua.DoString(file.FileString);
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("运行时候才可以执行代码:" + e.ToString());
                    }
                }
                //if (GUILayout.Button("浏览Table"))
                //{
                //    //luaFilePath = FileManager.BrowseFile("!!!!!!!!!!!!!excel!!!!!!!!!!!!!!!!!","*", Application.dataPath + "/../../../config-dev/tables/");
                //    EditorUtility.OpenFolderPanel("",Application.dataPath,"");
                //}


            });
        }
    }
}
