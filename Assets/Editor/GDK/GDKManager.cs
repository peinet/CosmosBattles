/********************************************************************
    created:	2018/01/31 16:28:50
    author:		chens
    purpose:
*********************************************************************/
using UnityEditor;
using UnityEngine;
using Assets.Editor.GDK.files;
using Assets.Editor.GDK.replaceStr;
using System.Collections;
using Assets.Editor.GDK.common;
using System;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using Assets.Editor.GDK.files.Parser.XLSX;

namespace Assets.Editor.GDK
{
    class GDKManager : EditorWindow, ISerializationCallbackReceiver
    {
        private static GDKManager s_Window;
        [MenuItem("UGameTools/小伙工具箱")]
        private static void ShowGDKWindow()
        {
            s_Window = GetWindow<GDKManager>();
            GUIContent content = new GUIContent();
            content.text = "小伙工具";
            s_Window.titleContent = content;
            s_Window.minSize = new Vector2(140, 100);
        }
        private ItemTableProxy itemTable;
        private Vector2 v2;


        private void OnGUI()
        {
            var wid = 120;
            var hei = 20;
            //GUILayout.Window(0, new Rect(50, 50, 100, 50), id => Log.Info("显示了吗"), "呵呵");

            //if (EditorApplication.isPlaying)
            //{
            //}
            //else
            //{
            //    CommonWindow.show("进入游戏在发道具，我现在不知道你要登录哪个服务器");
            //EditorUtility.DisplayDialog("", "11", "OK");
            //}
            v2 = EditorGUILayout.BeginScrollView(v2);
            //if (GUILayout.Button("连接测试服", GUILayout.Height(hei), GUILayout.Width(wid)))
            //{
            //    NetworkManagerProxy.StartClient();
            //}
            //if (GUILayout.Button("服务器信息", GUILayout.Height(hei), GUILayout.Width(wid)))
            //{
            //    NetworkManagerProxy.getInstance().serverInfo();
            //}
            //todo 触发器
            if (GUILayout.Button("系统配置", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                GDKConfigManager.getInstance().show();
            }
            if (GUILayout.Button("发送道具", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                if (itemTable == null)
                {
                    itemTable = new ItemTableProxy();
                }
                itemTable.showItemGrid();
            }
            if (GUILayout.Button("看看通信", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                NetworkManagerProxy.getInstance().showNetWorkMessage();
            }
            if (GUILayout.Button("功能相关", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                SystemOpenManager.getInstance().showFuncView();
            }
            if (GUILayout.Button("执行LUA", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                LuaExcuteManager.getInstance().show();
            }
            if ( string.IsNullOrEmpty(GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD_SERVER)) == false)
            {
                if (GUILayout.Button("后端玩的东西", GUILayout.Height(hei), GUILayout.Width(wid)))
                {
                    ServerToolsManager.getInstance().show();
                }
            }
            GUILayout.Label("-----分割线-----");
            if (GUILayout.Button("替换lua语言包", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                ReplaceManager.replace();
            }
            //if (GUILayout.Button("换prefab字体", GUILayout.Height(hei), GUILayout.Width(wid)))
            //{
            //    ReplaceManager.replaceFont();考虑是否整合到comparePrefab中
            //}
            if (GUILayout.Button("prefab文字->提取", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                ReplacePrefabTextKit.GetInstance().show();
            }
            if (GUILayout.Button("prefab对比", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                CompareManager.GetInstance().show();
            }
            if (GUILayout.Button("prefab组件搜索", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                CheckPrefabCompont.GetInstance().show();
            }
            if (GUILayout.Button("拷贝到分支", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                CopyManager.getInstance().copyToBranch();
            }
            if (GUILayout.Button("按单号拉配置", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                CopyManager.getInstance().copyToConfig();
            }
            GUILayout.Label("-----测试按钮----");
            if (GUILayout.Button("测试", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                //ExcelPackage package = new ExcelPackage(new FileInfo(Application.dataPath + "/../../../config/tables/item.xlsx"));
                //ParserXLSX file = FileManager.getInstance().getFile(Application.dataPath + "/../../../config/tables/System/systemLanguages2.xlsx", false, true) as ParserXLSX;
                //file.parseByReadWrite();
                //Debug.Log(file.Path);
                //ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                //var a = new MessagePack(4003 , null);
                TempProtocolGenerator.TestGenEmmyDoc();
            }
            if (GUILayout.Button("触发器", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                //FileManager.getInstance().getFile(Application.dataPath + "/../../../config-dev/protos/client/opcode.proto", true);
                //FileManager.getInstance().getFile(Application.dataPath + "/Luas/UI/UIPnlActivity/LoginNotice/LoginNoticeMediator.lua", true);
                //FileManager.getInstance().getFile(Application.dataPath + "/Luas/Protocol/Reverse/opcode.lua", true);
                FileManager.getInstance().getFile(Application.dataPath + "/Luas/TestLuaFile.lua", true, true);
            }

            EditorGUILayout.EndScrollView();

        }
        void Update()
        {
            //GMCommandProxy.sendGMCommand(90, 1);
        }
        public IEnumerator OnThreadLoop()
        {
            while (true)
            {
                Debug.Log("looper");
                yield return null;
            }
        }

        public void OnBeforeSerialize()
        {
            //Log.Info("GDK Window被序列化");
        }

        public void OnAfterDeserialize()
        {
            //Log.Info("GDK Window反序列化");
        }
    }
}

