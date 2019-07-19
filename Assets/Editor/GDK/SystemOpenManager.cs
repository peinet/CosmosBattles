/********************************************************************
	created:	2018/01/31 16:04:36
	filename: 	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\SystemOpenManagerProxy.cs
	file path:	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK
	author:		chens

	purpose:
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Editor.GDK
{
	class SystemOpenManager:ScriptableObject
	{
        private static SystemOpenManager _instance;
        public static SystemOpenManager getInstance()
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<SystemOpenManager>().FirstOrDefault();
            }
            if (_instance == null)
            {
                _instance = CreateInstance<SystemOpenManager>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }
            return _instance;
        }
        private string funcID = "";
        private string GMCmd = "";

        public void showFuncView()
		{
			//CommonWindow.varDic["ClientOpenFuncID"] = funcID;
			//CommonWindow.varDic["ClientOpenedFuncID"] = "";
   //         CommonWindow.varDic["ClientGMInput"] = "";
   //         CommonWindow.show(() =>
			//{
   //             if (GUILayout.Button("一键高级号"))
   //             {
   //                 GMCommandProxy.sendGMCommand(53, 1);
   //             }
			//	if (GUILayout.Button("打开所有功能"))
			//	{
			//		GMCommandProxy.sendGMCommand(8, 0);
   //             }
   //             if (GUILayout.Button("切换角色"))
   //             {
   //                 LuaScriptMgr.Instance.lua.DoString("UGameManager.TurnToSelectRole()");
   //             }

   //             GUILayout.BeginHorizontal();
			//	if (GUILayout.Button("前端强制开启某个功能"))
			//	{
   //                 //var table = LuaScriptMgr.Instance.lua.GetTable("SystemOpenManager");
   //                 //table.GetLuaFunction("AddFunction").Call(Int32.Parse( CommonWindow.varDic["ClientOpenFuncID"].ToString() ));
   //                 funcID = CommonWindow.varDic["ClientOpenFuncID"].ToString();
   //                 var str = "SystemOpenManager:AddFunction(" + CommonWindow.varDic["ClientOpenFuncID"].ToString()+ ")";
			//		LuaScriptMgr.Instance.lua.DoString(str);
			//	}
			//	CommonWindow.varDic["ClientOpenFuncID"] = GUILayout.TextField((String)CommonWindow.varDic["ClientOpenFuncID"]);

   //             if (GUILayout.Button("发送GM命令"))
   //             {
   //                 GMCmd = CommonWindow.varDic["ClientGMInput"].ToString();
   //                 var GMCmdArr = Regex.Split(GMCmd, ",");
   //                 int[] GMCMDParam = new int[GMCmdArr.Length - 1];
   //                 for (int i = 1; i < GMCmdArr.Length; i++)
   //                 {
   //                     GMCMDParam[i-1] = Convert.ToInt32(GMCmdArr[i]);
   //                 }
   //                 GMCommandProxy.sendGMCommand(Convert.ToInt32(GMCmdArr[0]), GMCMDParam);
   //             }
   //             CommonWindow.varDic["ClientGMInput"] = GUILayout.TextField((String)CommonWindow.varDic["ClientGMInput"]);
   //             GUILayout.EndHorizontal();
			//	if (GUILayout.Button("查看已经打开的功能ID"))
			//	{
			//		CommonWindow.varDic["ClientOpenedFuncID"] = "";
			//		try
			//		{
			//			var go = GameObject.Find("UGameManager");
			//			var lua = go.GetComponent<GlobalLua>();
			//			var funcDic = lua.LuaScriptMgr.GetTable("SystemOpenManager.FunctionList").ToDictTable();
			//			var list = funcDic.OrderBy(a => a.Key);
			//			var i = 0;
			//			foreach (var o in list)
			//			{
			//				i++;
			//				CommonWindow.varDic["ClientOpenedFuncID"] += o.Key + "\t";
			//				if (i % 10 == 0)
			//				{
			//					CommonWindow.varDic["ClientOpenedFuncID"] += "\n";
			//				}
			//			}
			//		}
			//		catch (Exception)
			//		{
			//			CommonWindow.varDic["ClientOpenedFuncID"] = "进入游戏在点这个按钮，我现在不知道服务器开启了哪些功能。";
			//		}
			//	}
			//	GUILayout.Label((String)CommonWindow.varDic["ClientOpenedFuncID"]);
			//});
		}
	}
}
