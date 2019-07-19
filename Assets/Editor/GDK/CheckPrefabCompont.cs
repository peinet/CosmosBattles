using EditorExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor.GDK
{
    class CheckPrefabCompont
    {
        private static CheckPrefabCompont _instance;
        private CommonWindow window;
        GameObject prefab;
        private List<Component> comps = new List<Component>();

        internal static CheckPrefabCompont GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CheckPrefabCompont();
            }
            return _instance;
        }
        public void show()
        {
            //CommonWindow.varDic["compontName"] = "";
            //CommonWindow.varDic["compontAttr"] = "";
            //CommonWindow.varDic["compontAttrValue"] = "";

            //window = CommonWindow.show(() =>
            //{
            //    GUILayout.Label("输入要搜索的组件名称，支持模糊搜索,支持正则表达式");
            //    CommonWindow.varDic["compontName"] = GUILayout.TextField((String)CommonWindow.varDic["compontName"], GUILayout.MinWidth(70));
            //    GUILayout.Label("输入要搜索的组件属性，支持模糊搜索,支持正则表达式,填属性就必须填值");
            //    CommonWindow.varDic["compontAttr"] = GUILayout.TextField((String)CommonWindow.varDic["compontAttr"], GUILayout.MinWidth(70));
            //    GUILayout.Label("输入要搜索的组件属性值，支持模糊搜索,支持正则表达式,填属性就必须填值");
            //    CommonWindow.varDic["compontAttrValue"] = GUILayout.TextField((String)CommonWindow.varDic["compontAttrValue"], GUILayout.MinWidth(70));
            //    prefab = (GameObject)EditorGUILayout.ObjectField("要检查的Prefab", prefab, typeof(GameObject), true);
            //    if (prefab != null)
            //    {
            //        if (GUILayout.Button("检查"))
            //        {
            //            if(String.IsNullOrEmpty(CommonWindow.varDic["compontName"].ToString()) == true)
            //            {
            //                Debug.LogError("输入要搜索的组件名称，支持模糊搜索,支持正则表达式");
            //                return;
            //            }
            //            comps = new List<Component>();
            //            checkPrefab(prefab);
            //        }
            //    }

            //    foreach (var comp in comps)
            //    {
            //        var CRCpath = comp.gameObject.transform.GetHierarchyPath();
            //        var compName = comp.GetType().ToString();
            //        GUILayout.TextField(CRCpath);
            //        GUILayout.TextField(compName);
            //        if (GUILayout.Button("跳转", GUILayout.Width(80)))
            //        {
            //            var go = GameObject.Find(CRCpath);
            //            Selection.activeGameObject = go;
            //        }
            //    }
            //});
        }
        private void checkPrefab(GameObject child)
        {
            var curComps  = child.GetComponents<Component>();
            for (int j = 0; j < curComps.Length; j++)
            {
                var compName = curComps[j].GetType().ToString().ToLower();
                var compontNameRegex = CommonWindow.varDic["compontName"].ToString().ToLower();
                var compontAttrRegex = CommonWindow.varDic["compontAttr"].ToString().ToLower();
                var compontAttrValueRegex = CommonWindow.varDic["compontAttrValue"].ToString().ToLower();
                Match matches = Regex.Match(compName, "(" + compontNameRegex + ")");
                if (matches.Groups != null && matches.Groups.Count > 1)
                {
                    //}
                    //if ( compName.Contains( CommonWindow.varDic["compontName"].ToString()) == true)
                    //{
                    //Debug.Log("发现" + child.gameObject.transform.GetHierarchyPath());
                    if(String.IsNullOrEmpty(compontAttrRegex) == false && String.IsNullOrEmpty(compontAttrValueRegex) == false)
                    {
                        PropertyInfo[] propertys = curComps[j].GetType().GetProperties();
                        foreach (var item in propertys)
                        {//检查属性名字
                            try
                            {
                                Match attrMatches = Regex.Match(item.Name, "(" + compontAttrRegex + ")");
                                Match valueMatches = Regex.Match(item.GetValue(curComps[j], null).ToString(), "(" + compontAttrValueRegex + ")");
                                if (attrMatches.Groups != null && attrMatches.Groups.Count > 1
                                    && valueMatches.Groups != null && valueMatches.Groups.Count > 1)
                                {
                                    comps.Add(curComps[j]);
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                        }
                    }else
                    {
                        comps.Add(curComps[j]);
                    }
                }
            }

            for (int i = 0; i < child.transform.childCount; i++)
            {
                checkPrefab(child.transform.GetChild(i).gameObject);
            }
        }

    }
}
