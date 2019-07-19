using EditorExtension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor.GDK
{
    class ReplacePrefabTextKit
    {
        private static ReplacePrefabTextKit _instance;
        private CommonWindow window;
        GameObject prefab;
        private string LanguageKey = "";
        List<Text> texts = new List<Text>();
        const string OutPutDir = "LocalizeText/LocalizeTable";

        internal static ReplacePrefabTextKit GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ReplacePrefabTextKit();
            }
            return _instance;
        }
        public void show()
        {

            window = CommonWindow.show(() =>
            {
                //prefab = (GameObject)EditorGUILayout.ObjectField("要检查的Prefab", prefab, typeof(GameObject), true);
                //GUILayout.BeginHorizontal();
                //if (prefab != null)
                //{
                //    if (GUILayout.Button("检查"))
                //    {
                //        checkPrefab();
                //    }
                //}else
                //{
                //    texts = new List<Text>();
                //}
                //GUILayout.TextField("LanguageKey");
                //LanguageKey = GUILayout.TextArea(LanguageKey, GUILayout.MinWidth(300));
                //GUILayout.EndHorizontal();
                //GUILayout.Space(30);

                //foreach (var text in texts)
                //{
                //    var CRCpath = text.gameObject.transform.GetHierarchyPath();
                //    GUILayout.TextField(CRCpath);
                //    GUILayout.TextField(text.text);
                //    var localize = text.GetComponent<Localize>();
                //    if (localize != null)
                //    {
                //        GUILayout.TextField(localize.m_key.ToString());
                //    }else
                //    {
                //        GUILayout.TextField("");
                //    }
                //    GUILayout.BeginHorizontal();
                //    //换prefab字体
                //    if (GUILayout.Button("跳转", GUILayout.Width(80)))
                //    {
                //        var go = GameObject.Find(CRCpath);
                //        Selection.activeGameObject = go;
                //    }
                //    if (GUILayout.Button("提取", GUILayout.Width(80)))
                //    {
                //        if(String.IsNullOrEmpty(LanguageKey) == true)
                //        {
                //            Debug.LogError("需要填写语言标签");
                //            return;
                //        }
                //        if(localize != null && localize.m_key != 0 )
                //        {
                //            Debug.LogError("当前存在语言标签,无法重复提取");
                //            return;
                //        }

                //        int intStrNum = 0;
                //        if (int.TryParse(LanguageKey, out intStrNum))
                //        {
                //            try
                //            {
                //                if (localize == null)
                //                {
                //                    localize = text.gameObject.AddComponent<Localize>();
                //                }

                //                var keyStr = File.ReadAllText(OutPutDir + "/LocalizeTable.txt");
                //                Match matches = Regex.Match(keyStr, "(\\d*)\\s*"+ text.text);
                //                if (matches.Groups != null && matches.Groups.Count > 1)
                //                {
                //                    var key = matches.Groups[1].ToString();
                //                    Debug.Log("使用已经提取的标签" + key);
                //                    localize.m_key = Int32.Parse(key);
                //                }
                //                else
                //                {
                //                    File.AppendAllText(OutPutDir + "/LocalizeTable.txt", LanguageKey + " " + text.text + "\n");
                //                    Debug.Log("文字" + LanguageKey + " " + text.text + "\n" + "提取到" + OutPutDir + "/LocalizeTable.txt");
                //                    localize.m_key = intStrNum;
                //                    LanguageKey = (intStrNum + 1).ToString();
                //                }
                //            }
                //            catch (Exception)
                //            {
                //                Debug.LogError("标签提取错误");
                //            }
                //        }else
                //        {
                //            Debug.LogError("LanguageKey只能是数字");
                //        }
                //    }
                //    if (GUILayout.Button("清除文本", GUILayout.Width(80)))
                //    {
                //        text.text = "";
                //    }
                //    if (GUILayout.Button("清除组件", GUILayout.Width(80)))
                //    {
                //        localize = text.gameObject.GetComponent<Localize>();
                //        if (localize != null) GameObject.DestroyImmediate(localize);
                //    }
                //    GUILayout.EndHorizontal();
                //    GUILayout.Space(15);
                //};
            });
        
        }
        private void checkPrefab()
        {
            var textComponts = prefab.GetComponentsInChildren<Text>(true);
            texts = new List<Text>();
            foreach (var text in textComponts)
            {
                if (String.IsNullOrEmpty(text.text) == false)
                {
                    texts.Add(text);
                }
            }
        }
    }
}
