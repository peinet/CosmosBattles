/********************************************************************
	created:	2018/01/31 16:04:12
	filename: 	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\GMCommandProxy.cs
	file path:	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK
	author:		chens
	
	purpose:	
*********************************************************************/
using Assets.Editor.GDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Assets.Editor.GDK
{
	class GMCommandProxy
	{
		//private static LuaFunction SendFunc = null;

		private static string script2 =
			@"
			local gmRequestBuffer = protobuf.encode('logic.proto.GmRequest', gmRequestReq)
			NetManager:SendMessage(opcode.CM_GM_REQUEST, gmRequestBuffer)
		";


		public static void sendGMCommand(int cmd , params int[] args)
		{
			try
			{
				//UGUtil.CallMethod("Protocol", "SendGMCommand",cmd, args);
				var str = "local gmRequestReq = {";
				str += "gm_cmd =" + cmd+",";
				str += "parms={";
				for (int i = 0; i < args.Count(); i++)
				{
					str += "["+(i+1)+"]="+args[i] + ",";
				}
				str += "}}";
				//var lua = LuaScriptMgr.Instance.lua;
				//lua.DoString(str+script2);


                //if (testFunc == null)
                //{
                //    LuaTable _G = LuaScriptMgr.Instance.lua.GetTable("_G");
                //    if (_G != null)
                //    {
                //        testFunc = _G.GetLuaFunction("leiTestFunc");
                //    }
                //}
                //testFunc.BeginPCall();
                //int count = 1;

                //if (!testFunc.luaState.LuaCheckStack(count + 6))
                //{
                //    testFunc.EndPCall();
                //    throw new LuaException("stack overflow");
                //}
                //testFunc.argCount = 1;
                //LuaDLL.lua_createtable(testFunc.luaState.GetL(), 5, 0);
                //for (int i = 0; i < 5; i++)
                //{
                //    LuaDLL.lua_pushinteger(testFunc.luaState.GetL(), 5 + i);
                //    LuaDLL.lua_rawseti(testFunc.luaState.GetL(), -2, i + 1);
                //}

                //testFunc.PCall();
                //testFunc.EndPCall();

            }
            catch// (SystemException e)
			{
				CommonWindow.show("     进入游戏在使用这个功能");//\n错误详情：\n" + e
			}
		}
	}
}
