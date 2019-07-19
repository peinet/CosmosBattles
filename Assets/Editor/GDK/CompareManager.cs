//todo 出现同名prefab对比会出现问题。
using Assets.Editor.GDK.common;
using EditorExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor.GDK
{
    class CompareManager
    {
        private static CompareManager _instance;
        private CommonWindow window;
        private bool checkCompontValue = false;
        private bool checkCompont = true;
        GameObject prefab1;
        GameObject prefab2;
        private static object logListView;
        private static object consoleWindow;
        private static FieldInfo logListViewCurrentRow;
        private static FieldInfo logEntryCondition;
        private static FieldInfo listViewFieldInfo;
        private static object logEntry;
        int getPrefabBtnWid = 200;
        string cnPath = "";
        string twPath = "";
        string wdPath = "";
        string krPath = "";

        public static MethodInfo LogEntriesGetEntry { get; private set; }

        internal static CompareManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CompareManager();
            }
            return _instance;
        }
        public void show()
        {
            //window = CommonWindow.show(() =>
            //{
            //    prefab1 = (GameObject)EditorGUILayout.ObjectField("当前项目的Prefab", prefab1, typeof(GameObject), true);

            //    prefab2 = (GameObject)EditorGUILayout.ObjectField("目标项目的Prefab", prefab2, typeof(GameObject), true);

            //    if(prefab1 != null)
            //    {
            //        cnPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_CN);
            //        twPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_TW);
            //        wdPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD);
            //        krPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_KR);
            //        if(string.IsNullOrEmpty(cnPath) == true && string.IsNullOrEmpty(twPath) == true
            //        && string.IsNullOrEmpty(wdPath) == true && string.IsNullOrEmpty(krPath) == true)
            //        {
            //            GUILayout.Label("在系统设置中设置各地区的项目路径，可以显示拉取按钮");
            //        }
            //        else
            //        {
            //            GUILayout.Label("同名prefab会被拉到SUNClient/Assets/ArtEdit/AllResources/UI/Resources/UI下面以对应地区简写结尾，不用时候记得删除");
            //        }
            //        EditorGUILayout.BeginHorizontal();
            //        if (string.IsNullOrEmpty(cnPath) == false && GUILayout.Button("拉取国服的同名Prefab", GUILayout.Width(getPrefabBtnWid)))
            //        {
            //            getPrefabFromPath(cnPath, prefab1 ,"cn");
            //        }
            //        if (string.IsNullOrEmpty(twPath) == false && GUILayout.Button("拉取台服的同名Prefab", GUILayout.Width(getPrefabBtnWid)))
            //        {
            //            getPrefabFromPath(twPath, prefab1 , "tw");
            //        }
            //        if (string.IsNullOrEmpty(wdPath) == false && GUILayout.Button("拉取世界的同名Prefab", GUILayout.Width(getPrefabBtnWid)))
            //        {
            //            getPrefabFromPath(wdPath, prefab1, "world");
            //        }
            //        if (string.IsNullOrEmpty(krPath) == false && GUILayout.Button("拉取韩国的同名Prefab", GUILayout.Width(getPrefabBtnWid)))
            //        {
            //            getPrefabFromPath(krPath, prefab1 , "kr");
            //        }
            //        EditorGUILayout.EndHorizontal();
            //    }

            //    checkCompont = GUILayout.Toggle(checkCompont, "是否检查组件");
            //    checkCompontValue = GUILayout.Toggle(checkCompontValue, "是否检查组件的值");
            //    GUILayout.TextField("LOG中红色代表两边GameObject有一边不存在");
            //    GUILayout.TextField("蓝色代表组件有一边不存在");
            //    GUILayout.TextField("橙色代表组件值两边不一致");
            //    GUILayout.Space(30);
            //    if (GUILayout.Button("对比"))
            //    {
            //        if (prefab1 != null && prefab2 != null)
            //        {
            //            if(checkCompont)
            //            {
            //                compareCompont(prefab1, prefab2, prefab1.name, prefab2.name);
            //                compareCompont(prefab2, prefab1, prefab2.name, prefab1.name, false);//反过来在对比一次,第二次对比时候不对比相同组件的值了
            //            }
            //            compareChildGameObject(prefab1, prefab2, prefab1.name, prefab2.name , checkCompont);
            //            compareChildGameObject(prefab2, prefab1, prefab2.name, prefab1.name, false);//反过来在对比一次,第二次对比时候不对比相同GameObject的组件了
            //        }
            //    }
            //    GetConsoleWindowListView();
            //});
        }

        private void getPrefabFromPath(string sourcePath ,GameObject curPrefab , string area)
        {
            //UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(curPrefab);
            //string prefab1Path = AssetDatabase.GetAssetPath(parentObject);//     Assets/ArtEdit/AllResources/UI/Resources/xxx.prefab
            var CRCpath = curPrefab.gameObject.transform.GetHierarchyPath();//   UGameManager/UIManager/ViewNode/GM/xxx
            
            string sourceFile = sourcePath + "/Assets/ArtEdit/AllResources/UI/Resources/" + curPrefab.name + ".prefab";
            string prefabPath = "/ArtEdit/AllResources/UI/Resources/UI/" + curPrefab.name + "_" + area + ".prefab";
            string toPath = Application.dataPath + prefabPath;
            FileUtil.ReplaceFile(sourceFile, toPath);
            //PrefabUtility.InstantiatePrefab();
            //var xx = PrefabUtility.GetPrefabObject(curPrefab);
            //var yy = PrefabUtility.GetPrefabParent(curPrefab);
            //PrefabUtility.ReplacePrefab
            GameObject objPrefab = AssetDatabase.LoadAssetAtPath("Assets"+ prefabPath, typeof(GameObject)) as GameObject;
            //objPrefab.gameObject.transform.SetParent(curPrefab.gameObject.transform.parent);
            AssetDatabase.Refresh();
        }

        //对比两边是否有一样的GameObject
        public void compareChildGameObject(GameObject GOA, GameObject GOB, string GOAPath, string GOBPath, bool isCompareCompont = true)
        {
            for (int i = 0; i < GOA.transform.childCount; i++)
            {
                bool has = false;
                var a = GOA.transform.GetChild(i);
                for (int j = 0; j < GOB.transform.childCount; j++)
                {
                    var b = GOB.transform.GetChild(j);
                    if (a.name == b.name)
                    {//两边有同样的GameObject
                        has = true;
                        if (isCompareCompont == true)
                        {
                            compareCompont(a.gameObject, b.gameObject, GOAPath + "/" + a.name, GOBPath + "/" + b.name);
                            compareCompont(b.gameObject, a.gameObject, GOBPath + "/" + a.name, GOAPath + "/" + b.name,false);//第二次就不对比组件值了
                        }
                        compareChildGameObject(a.gameObject, b.gameObject, GOAPath + "/" + a.name, GOBPath + "/" + b.name, isCompareCompont);
                        break;
                    }
                }
                if (has == false)
                {
                    Debug.Log("对象<color=red>" + GOAPath + "/" + a.name + "</color>在另外一个Prefab中不存在");
                }
            }
        }
        //对比组件是否存在
        public void compareCompont(GameObject GOA, GameObject GOB, string GOAPath, string GOBPath,bool isCompareCompontValue = true)
        {
            var ACompontArr = GOA.GetComponents<Component>();
            var BCompontArr = GOB.GetComponents<Component>();
            for (int i = 0; i < ACompontArr.Length; i++)
            {
                bool has = false;
                var a = ACompontArr[i];
                for (int j = 0; j < BCompontArr.Length; j++)
                {
                    var b = BCompontArr[j];
                    if (a.GetType() == b.GetType())
                    {//两边有同样的组件
                        has = true;
                        if(isCompareCompontValue == true && checkCompontValue == true)
                        {
                            compareCompontValue(a, b, GOAPath, GOBPath);
                        }
                        break;
                    }
                }
                if (has == false)
                {
                    Debug.Log("组件<color=blue>" + GOAPath + ":<" + a.GetType().ToString() + "></color>在另外一个Prefab上不存在");
                }
            }
        }
        //对比组件的值是否一致
        public void compareCompontValue(Component ACompont,Component BCompont, string GOAPath, string GOBPath)
        {
            if(ACompont.GetType() == typeof(RectTransform))
            {
                return;
            }
            PropertyInfo[] APropertys = ACompont.GetType().GetProperties();
            PropertyInfo[] BPropertys = BCompont.GetType().GetProperties();
            for (int i = 0; i < APropertys.Length; i++)
            {
                if (APropertys[i].CanWrite == true && APropertys[i].Name != "parent")
                {//不能写的属性没必要对比 parent要么肯定相等，要么没对比的必要
                    for (int j = 0; j < BPropertys.Length; j++)
                    {
                        if (APropertys[i].Name == BPropertys[j].Name)
                        {//同一个属性
                         //Debug.Log(APropertys[i].Name);
                            var aValue = APropertys[i].GetValue(ACompont, null);
                            var bValue = BPropertys[j].GetValue(BCompont, null);
                            if (aValue!= null && bValue != null && aValue.ToString() != bValue.ToString()
                                && APropertys[i].Name != "name" && APropertys[i].Name != "font" && APropertys[i].Name != "text"//这三个应该存在差异
                                && APropertys[i].Name != "overrideSprite"//一个sprite差异就可以了这个不用输出
                                )
                            {//暂时不对比这几个属性值
                                Debug.Log("组件<color=orange>" + GOAPath + ":<" + ACompont.GetType() + ">属性:" + APropertys[i].Name + "</color>的值与另外一个不相等");
                            }
                            break;
                        }
                    }

                }
            }
            //if (ACompont.GetType() == typeof(RectTransform))
            //{//对RectTransform的值进行对比
            //    RectTransform ATransform = ACompont as RectTransform;
            //    RectTransform BTransform = BCompont as RectTransform;
            //    //if (ATransform.anchoredPosition.x != BTransform.anchoredPosition.x ||
            //    //    ATransform.anchoredPosition.y != BTransform.anchoredPosition.y)
            //    //{
            //    //    Debug.Log(GOAPath + ":<Compont>" + ATransform.ToString() + "在的值与另外一个不相等");
            //    //}
            //}

        }
        public bool FindCode()
        {
            var str = GetLog();
            Match matches = Regex.Match(str, "[组件|对象]+<.*?>.*?/(.*?)[:|<.*?>|\u4e00-\u9fa5]+");
            if(matches.Groups != null && matches.Groups.Count > 1 )
            {
                var goPath = matches.Groups[1].ToString();
                //var goList = SceneManager.GetActiveScene().GetRootGameObjects();
                //for (int i = 0; i < goList.Length; i++)
                //{
                //    EditorGUIUtility.PingObject(goList[i].transform.Find(prefab1.name + "/" + goPath));
                //    EditorGUIUtility.PingObject(goList[i].transform.Find(prefab2.name + "/" + goPath));
                //}
                if(prefab1 != null && prefab2 != null)
                {
                    var go1 = GameObject.Find(prefab1.name + "/" + goPath);
                    var go2 = GameObject.Find(prefab2.name + "/" + goPath);
                    if(Selection.activeGameObject == go1)
                    {
                        EditorGUIUtility.PingObject(go2);
                        Selection.activeGameObject = go2;
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(go1);
                        Selection.activeGameObject = go1;

                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (!GetConsoleWindowListView() || (object)EditorWindow.focusedWindow != consoleWindow)
            {
                return false;
            }
            //if (instanceID == 3154)
            //{

            //}
            return CompareManager.GetInstance().FindCode();
        }    


        private static bool GetConsoleWindowListView()
        {
            if (logListView == null)
            {
                //获取类型为EditorWindow的程序集
                Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
                //从程序集中获取UnityEditor.ConsoleWindow的实例
                Type consoleWindowType = unityEditorAssembly.GetType("UnityEditor.ConsoleWindow");
                //从实例中获取ms_ConsoleWindow ms_ConsoleWindow是ConsoleWindow的局部变量
                FieldInfo fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                //获取ms_ConsoleWindow的值    
                consoleWindow = fieldInfo.GetValue(null);

                if (consoleWindow == null)
                {
                    logListView = null;
                    return false;
                }

                listViewFieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
                logListView = listViewFieldInfo.GetValue(consoleWindow);
                logListViewCurrentRow = listViewFieldInfo.FieldType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
                Type logEntriesType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntries");
                LogEntriesGetEntry = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
                Type logEntryType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntry");
                logEntryCondition = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
                logEntry = Activator.CreateInstance(logEntryType);
            }

            return true;
        }

        private static string GetLog()
        {
            int row = (int)logListViewCurrentRow.GetValue(logListView);
            LogEntriesGetEntry.Invoke(null, new object[] { row, logEntry });
            return logEntryCondition.GetValue(logEntry) as string;
        }

    }
}