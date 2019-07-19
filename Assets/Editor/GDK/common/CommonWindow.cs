/********************************************************************
	created:	2018/01/31 15:36:22
	filename: 	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\common\CommonWindow.cs
	file path:	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\common
	author:		chens
	
	purpose:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
	class CommonWindow : EditorWindow
	{
		public delegate void LayoutFunc();
		private static string _showString = "";
		private static LayoutFunc _layoutFunc = null;
		private static LayoutFunc _updateFunc = null;
		private Vector2 v2;
		public static Dictionary<object, object> varDic = new Dictionary<object, object>();
		static CommonWindow()
		{
		}
		public static CommonWindow show(string str)
		{
			_layoutFunc = null;
			var s_Window = GetWindow<CommonWindow>();
			GUIContent content = new GUIContent();
			content.text = "kr版DEV环境";
			s_Window.titleContent = content;
			_showString = str;
			_updateFunc = null;
			return s_Window;
		}
		/// <summary>
		/// 一个通用的面板
		/// </summary>
		/// <param name="func">当更新时候会调用这个函数更新</param>
		/// <returns>无</returns>
		public static CommonWindow show(LayoutFunc func)
		{
			_showString = "";
			var s_Window = GetWindow<CommonWindow>();
			GUIContent content = new GUIContent();
			content.text = "kr版DEV环境";
			s_Window.titleContent = content;
			_layoutFunc = func;
			_updateFunc = null;
			return s_Window;
		}
		public CommonWindow()
		{
		}
		private void OnGUI()
		{
			v2 = EditorGUILayout.BeginScrollView(v2);
			if(_showString != "")
			{
				GUILayout.Label(_showString);
			}
			if(_layoutFunc != null)
			{
				_layoutFunc();
			}
			EditorGUILayout.EndScrollView();
		}
		public void Update()
		{
			if(_updateFunc != null)
			{
				_updateFunc();
			}
		}
		public void setUpdateCall(LayoutFunc updateFunc)
		{
			_updateFunc = updateFunc;
		}

	}
}
