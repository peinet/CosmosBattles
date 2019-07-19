using Assets.Editor.GDK.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
    class ToolSet : EditorWindow, ISerializationCallbackReceiver
    {
        private Vector2 v2;
        private static ToolSet s_Window;
        [MenuItem("UGameTools/工具集")]
        private static void ShowToolSetWindow()
        {
            s_Window = GetWindow<ToolSet>();
            GUIContent content = new GUIContent();
            content.text = "工具集";
            s_Window.titleContent = content;
            s_Window.minSize = new Vector2(140, 100);
        }

        private void OnGUI()
        {
            var wid = 120;
            var hei = 20;
            v2 = EditorGUILayout.BeginScrollView(v2);
            if (GUILayout.Button("系统配置", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                GDKConfigManager.getInstance().show();
            }
            if (GUILayout.Button("prefab对比", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                CompareManager.GetInstance().show();
            }
            if (GUILayout.Button("生成协议", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                TempProtocolGenerator.GeneratePbFile();
            }
            if (GUILayout.Button("生成分支协议", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                TempProtocolGenerator.GeneratePbFileToBrach();
            }
            if (GUILayout.Button("更新配置和Proto", GUILayout.Height(hei), GUILayout.Width(wid)))
            {
                string error = "";
                string output = "";
                GDKApplication.exucteCMD("svn up "+ Application.dataPath+ "/../../../../config --accept=theirs-full", out output,out error);
                if(error != "")
                {
                    Debug.LogError(error);
                }else
                {
                    Debug.Log(output);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        void Update()
        {
            //GMCommandProxy.sendGMCommand(90, 1);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
