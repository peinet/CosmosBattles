using Assets.Editor.GDK.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Editor.GDK
{
    class ServerToolsManager
    {
        private static ServerToolsManager _instance;
        private CMDProcess cmd;
        public static ServerToolsManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new ServerToolsManager();
            }
            return _instance;
        }
        ServerToolsManager()
        {
            cmd = CMDProcess.CreateAInstance(false);
        }
        public void show()
        {
            CommonWindow.show(() =>
            {
                GUILayout.Label("-----------------------------------------------------");

                if (GUILayout.Button("更新后端所有文件"))
                {
                    string error = "";
                    string output = "";
                    var serverPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD_SERVER);
                    GDKApplication.exucteCMD("svn up " + serverPath + " --accept=theirs-full", out output, out error);
                    if (error != "")
                    {
                        Debug.LogError(error);
                    }
                    else
                    {
                        Debug.Log(output);
                    }
                }
                if (GUILayout.Button("更新后端Proto"))
                {
                    var serverPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD_SERVER);
                    string error = "";
                    string output = "";
                    GDKApplication.exucteCMD("svn up " + serverPath + "/protos --accept=theirs-full", out output, out error);
                    if (error != "")
                    {
                        Debug.LogError(error);
                    }
                    else
                    {
                        Debug.Log(output);
                    }
                }

                if (GUILayout.Button("更新前端配置表"))
                {
                    string error = "";
                    string output = "";
                    GDKApplication.exucteCMD("svn up " + Application.dataPath + "/../../../../config --accept=theirs-full", out output, out error);
                    if (error != "")
                    {
                        Debug.LogError(error);
                    }
                    else
                    {
                        Debug.Log(output);
                    }
                }
                if (GUILayout.Button("调用AutoGenTool.py"))
                {
                    var serverPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD_SERVER);
                    cmd.Start();
                    cmd.setDataReceived(new System.Diagnostics.DataReceivedEventHandler(delegate (object sender, System.Diagnostics.DataReceivedEventArgs e)
                    {
                        Debug.Log(e.Data);
                    }));
                    cmd.StandardInput.WriteLine("cd " + serverPath + "/python_tool");
                    cmd.StandardInput.AutoFlush = true;
                    cmd.StandardInput.WriteLine("python AutoGenTool.py");
                    cmd.exit();
                }



                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
                //if (GUILayout.Button("单独调用某个Fucker"))
                //{
                //}
            });
        }
    }
}
