using Assets.Editor.GDK.common;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor.GDK
{
    class CopyManager
    {
        private static CopyManager _instance;
        private CommonWindow window;
        private string output = "";
        private string error = "";

        internal static CopyManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new CopyManager();
            }
            return _instance;
        }
        public void copyToConfig()
        {//将design的表拷贝到config,根据单号
            CommonWindow.varDic["TaskCode"] = "";
            window = CommonWindow.show(() =>
            {
                GUILayout.Label("将策划的表从config_design 拷贝到 config当中,根据单号copy,此功能不是按行合并，是直接复制");
                CommonWindow.varDic["TaskCode"] = GUILayout.TextField((String)CommonWindow.varDic["TaskCode"], GUILayout.MinWidth(70));
                if(GUILayout.Button("开始拷贝"))
                {
                    GDKApplication.exucteCMD("svn log " + Application.dataPath + "/../../../config-design -l 100 -v --xml",out output, out error);//只看最近100条文件记录。
                    //GDKApplication.copyTo();
                    Debug.Log(output);
                }
            });
        }
        //将本地SVN差异文件和新增文件复制到分支上，只复制luas和prefab,其他文件不复制，在svn忽略列表中的文件也不会复制
        public void copyToBranch()
        {
            Scene scene = SceneManager.GetActiveScene();
            var goArr = scene.GetRootGameObjects();
            //ClientBranchManager branchManager = null;
            //for (int i = 0; i < goArr.Length; i++)
            //{
            //    branchManager = goArr[i].GetComponent<ClientBranchManager>();
            //    if (branchManager != null)
            //    {
            //        break;
            //    }
            //}
            //if (branchManager == null)
            //{
            //    UnityEngine.Debug.LogError("未找到分支组件，检查场景是否正确");
            //}
            var branchName = "None" ;//branchManager.GetBranchName();
            if (branchName == "None")
            {
                UnityEngine.Debug.LogWarning("当前分支选择为None,取消复制");
            }
            else
            {
                string error = "";
                string output = "";


                GDKApplication.exucteCMD("svn status " + Application.dataPath + "/Luas -u", out output, out error);
                Debug.Log("svn status " + Application.dataPath + "/Luas -u");
                if (error != "")
                {
                    Debug.LogError(error);
                }
                else
                {
                    output = Regex.Replace(output, "--- Changelist[\\s\\S]*", "");
                    var matches = Regex.Matches(output, "(M\\s+.*?\\d+|\\?)+\\s+(\\S*)");
                    for (int i = 0; i < matches.Count; i++)
                    {
                        if (matches[i].Groups != null && matches[i].Groups.Count > 1)
                        {
                            var luaPath = matches[i].Groups[2].ToString();
                            if(luaPath.EndsWith(".meta") == false && 
                                luaPath.EndsWith("Luas") == false) //过滤掉部分.meta文件，过滤掉Luas整目录的copy
                            {
                                var relativePath = Regex.Replace(luaPath, "\\\\", "/");
                                relativePath = Regex.Replace(relativePath, ".*?Assets", "");
                                GDKApplication.copyTo(luaPath, Application.dataPath + "/Branchs/" + branchName + relativePath);
                                //Debug.Log("复制文件" + luaPath);
                            }
                        }
                    }
                }



                GDKApplication.exucteCMD("svn status " + Application.dataPath + "/ArtEdit/AllResources/UI/Resources -u", out output, out error);
                if (error != "")
                {
                    Debug.LogError(error);
                }
                else
                {
                    output = Regex.Replace(output, "--- Changelist[\\s\\S]*", "");
                    var matches = Regex.Matches(output, "(M|\\?)+\\s+\\d*.*?(\\S*?\\.prefab)\\s");//如果文件名有空格会有bug,先忽略
                    for (int i = 0; i < matches.Count; i++)
                    {
                        if (matches[i].Groups != null && matches[i].Groups.Count > 1)
                        {
                            var prefabPath = matches[i].Groups[2].ToString();
                            var relativePath = Regex.Replace(prefabPath, "\\\\", "/");
                            relativePath = Regex.Replace(relativePath, ".*?/Resources", "");
                            GDKApplication.copyTo(prefabPath, Application.dataPath + "/Branchs/" + branchName + "/BranchResource" + relativePath);
                            //Debug.Log("复制文件" + prefabPath);
                        }
                    }
                }


                AssetDatabase.Refresh();

            }
        }
    }

}
