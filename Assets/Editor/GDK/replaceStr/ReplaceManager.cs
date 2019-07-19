/********************************************************************
	created:	2018/03/26 16:16:36
	author:		chens
	purpose:	
*********************************************************************/
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using AntlrDLL;
using Assets.Editor.GDK.common;
using Assets.Editor.GDK.files;
using Assets.Editor.GDK.files.Parser.lua;
using Assets.Editor.GDK.files.Parser.XLSX;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor.GDK.replaceStr
{
    class ReplaceManager
    {
        private static List<string> list = null;
        private static FileReplacer<LuaParser.ChunkContext> languageKeyReplacer;
        private static int index;
        public static string curUrl;
        internal static void replace()
        {
            list = new List<string>();
            //list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/Common", ".lua", true));//2
            //list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/Logic", ".lua", true));//108
            //list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/Protocol", ".lua", true));//7
            //list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/System", ".lua", true));//0
            //list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/UI", ".lua", true));//562  
            list.AddRange(FileManager.GetFileUrls(Application.dataPath + "/Luas/UI/UIWing", ".lua", true));//562  
            Debug.Log("需要检查的lua文件夹数量" + list.Count);
            index = 0;
            languageKeyReplacer = new FileReplacer<LuaParser.ChunkContext>();
            EditorApplication.update += Update;//防止界面卡住
        }
        public static void Update()
        {
            if (list.Count > index)
            {//下面这些目录不提取
                if (list[index].IndexOf("UIPnlMain.lua") > -1
                    || list[index].IndexOf("LoginPanel.lua") > -1
                    || list[index].IndexOf("UIPnlCertificationDefine.lua") > -1
                    //|| list[index].IndexOf("ServerListManager.lua") > -1
                    || list[index].IndexOf("LocalPlayer.lua") > -1
                    || list[index].IndexOf("Protocol.lua") > -1
                    || list[index].IndexOf("TabManager.lua") > -1
                    || list[index].IndexOf("ServerListHelper.lua") > -1
                    || list[index].IndexOf("UIShare.lua") > -1

                    )
                {
                    index++;
                    return;
                }
                if (EditorUtility.DisplayCancelableProgressBar(index + "/" + list.Count, "搜索" + list[index], (float)index / list.Count))
                {
                    Debug.Log("语言写入文件：" + Application.dataPath + "/../language.ini");
                    Debug.Log("一共搜索到" + languageKeyReplacer.num + "个中文。");
                    EditorApplication.update -= Update;
                    EditorUtility.ClearProgressBar();
                }
                curUrl = list[index];
                LuaFileParser file = FileManager.getInstance().getFile(list[index], true, true, false) as LuaFileParser;
                languageKeyReplacer.SearchAndEdit(file);
                index++;
            }
            else
            {
                Debug.Log("语言写入文件：" + Application.dataPath + "/../language.ini");
                Debug.Log("一共搜索到" + languageKeyReplacer.num + "个中文。");
                EditorApplication.update -= Update;
                EditorUtility.ClearProgressBar();
            }
        }
        //把在国服依然使用但是台湾已经删掉的字体替换成台湾现有的字体
        internal static void replaceFont()
        {
            var urls = FileManager.GetFileUrls(Application.dataPath + "/ArtEdit/AllResources/UI/Resources/");
            foreach (var url in urls)
            {
                var file = FileManager.getInstance().getFile(url, false,true);
                //Debug.Log(file.Path);
                if (file.FileString.Contains("53bd3b795840cbe48942c48daf83e53d"))
                {
                    Debug.Log(file.Path);
                    file.Save(file.FileString.Replace("53bd3b795840cbe48942c48daf83e53d", "97be28db01725cc4ebf0b90465d4c7cf"));
                    //break;
                }
            }
        }
        //把prefab中的简体字替换成繁体字
        internal static void replaceSimpToTrad()
        {
            var urls = FileManager.GetFileUrls(Application.dataPath + "/ArtEdit/AllResources/UI/Resources/");
            //int i = 0;
            foreach (var url in urls)
            {
                try
                {
                    if(url.EndsWith(".prefab") == false)
                    {
                        continue;
                    }
                    var file = FileManager.getInstance().getFile(url, false, true);
                    var str = file.FileString;
                    str = Regex.Replace(str,@"\\u\w{4}"  , simp => {
                        var fucker = ChineseStringUtility.ToTraditional(Regex.Unescape(simp.Value));


                        byte[] bytes = Encoding.Unicode.GetBytes(fucker);

                        fucker = @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2");

                        return fucker;
                    });
                    file.Save(str);
                    //Debug.Log(ChineseStringUtility.ToTraditional("\u4e00"));
                }
                catch (System.Exception)
                {
                    Debug.Log("文件未能成功轉換:" + url);
                    throw;
                }
                //break;
                //i++;
                //if (i == 50)
                //{
                //    break;
                //}
            }
        }
    }
    class FileReplacer<T> : AntlrDLL.LuaBaseVisitor<T>
    {
        public int num = 0;

        private LuaFileParser _file;
        private ParserXLSX table;
        //搜索字符串  修改字符串  并保存
        public void SearchAndEdit(LuaFileParser file)
        {
            _file = file;

            table = FileManager.getInstance().getFile(Application.dataPath + "/../../../../config/tables/System/systemLanguages.xlsx", true, false) as ParserXLSX;

            try
            {
                Visit(file.ASTree);
                file.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError("处理字符串发生未知问题" + file.Path + " " + e.ToString());
            }
        }
        protected internal override bool ShouldVisitNextChild(IRuleNode node, T currentResult)
        {
            if (node is ParserRuleContext)
            {
                ParserRuleContext context = node as ParserRuleContext;
                var funcList = context.GetRuleContexts<LuaParser.VarOrExpContext>();
                if (funcList.Length != 0)
                {
                    foreach (var item in funcList)
                    {
                        if (
                            item.GetText() == "error" |
                            item.GetText() == "warn" |
                            item.GetText() == "log" |
                            item.GetText() == "print" |
                            item.GetText() == "pb_debugFun" |
                            item.GetText() == "pb_alertFun" |
                            item.GetText() == "netDebug" |
                            item.GetText() == "netAlert" |
                            item.GetText() == "LogWarning" |
                            item.GetText() == "UserLog"
                            )
                        {
                            return false;
                        }
                    }
                }
            }
            return base.ShouldVisitNextChild(node, currentResult);
        }
        public override T VisitBlock([NotNull] LuaParser.BlockContext context)
        {
            return base.VisitBlock(context);
        }
        public override T VisitFunctioncall([NotNull] LuaParser.FunctioncallContext context)
        {//换完了，屏蔽掉加快速度
            //return base.VisitFunctioncall(context);
            if (replaceStringFormat(context))
            {
                return default(T);
            }
            return base.VisitFunctioncall(context);
        }
        public override T VisitPrefixexp([NotNull] LuaParser.PrefixexpContext context)
        {//换完了，屏蔽掉加快速度
            //return base.VisitPrefixexp(context);
            if (replaceStringFormat(context))
            {
                return default(T);
            }
            return base.VisitPrefixexp(context);
        }
        private bool replaceStringFormat(ParserRuleContext context)
        {
            LuaParser.VarOrExpContext[] methodArr;
            bool hasChinese = false;
            methodArr  = context.GetRuleContexts<LuaParser.VarOrExpContext>();
            foreach (var method in methodArr)
            {
                if (method.GetText() == "string.format")
                {
                    var argsArr = context.GetRuleContexts<LuaParser.NameAndArgsContext>();
                    foreach (var args in argsArr)
                    {
                        var argArr = args.GetRuleContexts<LuaParser.ArgsContext>();
                        foreach (var arg in argArr)
                        {
                            var explistArr = arg.GetRuleContexts<LuaParser.ExplistContext>();
                            foreach (var explist in explistArr)
                            {
                                var expArr = explist.GetRuleContexts<LuaParser.ExpContext>();
                                foreach (var exp in expArr)
                                {
                                    var strArr = exp.GetRuleContexts<LuaParser.StringContext>();
                                    foreach (var item in strArr)
                                    {
                                        var patten = "[\u4e00-\u9fa5]";
                                        var strList = Regex.Matches(item.GetText(), patten);
                                        if (strList.Count > 0)
                                        {
                                            var index = getLanguageKey();
                                            _file.Rewriter.Replace(context.start, exp.stop, "LanguagesManager.GetFormatString(" + index);
                                            writeLanguageKey(index ,exp.GetText(), ReplaceManager.curUrl);
                                            hasChinese = true;
                                        }
                                        //Debug.Log(item.GetText());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return hasChinese;
        }
        public override T VisitExp([NotNull] LuaParser.ExpContext context)
        {//已经提取，屏蔽加速
            //return base.VisitExp(context);
            //第二部用..连接的字符串提取
            var strcatArr = context.GetRuleContexts<LuaParser.OperatorStrcatContext>();
            if (strcatArr.Length > 0 && strcatArr[0].GetText() == "..")
            {
                List<string> CNStr = new List<string>();
                List<string> tokens = new List<string>();
                getFuckers(context, CNStr, tokens);
                bool hasCNStr = false;
                string languageStr = "";
                string tokenStr = "";
                if (CNStr.Count > 0)
                {
                    foreach (var str in CNStr)
                    {
                        var patten = "[\u4e00-\u9fa5]";
                        var strList = Regex.Matches(str, patten);
                        if (strList.Count > 0 && hasCNStr == false)
                        {
                            hasCNStr = true;
                        }
                        languageStr += str;
                    }
                    languageStr = languageStr.Replace("\"", "");
                    foreach (var token in tokens)
                    {
                        tokenStr += ",";
                        tokenStr += token;
                    }
                }
                if (hasCNStr)
                {
                    var index = getLanguageKey();
                    _file.Rewriter.Replace(context.start, context.stop, "LanguagesManager.GetFormatString(" + index + tokenStr + ")");
                    writeLanguageKey( index,languageStr, ReplaceManager.curUrl);
                    return default(T);
                }
            }
            return base.VisitExp(context);
        }
        private void getFuckers(ParserRuleContext context, List<string> CNStrList, List<string> otherList)
        {
            for (int i = 0; i < context.ChildCount; i++)
            {
                ParserRuleContext child = context.GetChild(i) as ParserRuleContext;
                if (child is LuaParser.StringContext)
                {
                    CNStrList.Add(child.GetText());
                }
                else if (child is LuaParser.ExpContext)
                {
                    getFuckers(child, CNStrList, otherList);
                }
                else if (child is LuaParser.OperatorStrcatContext)
                {
                    if (CNStrList.Count == 0) CNStrList.Add("%s");
                    if (CNStrList.Count > 0 && CNStrList[CNStrList.Count - 1] != "%s") CNStrList.Add("%s");

                }
                else if (child != null)
                {
                    otherList.Add(child.GetText());
                }

            }
        }

        public override T VisitOperatorStrcat([NotNull] LuaParser.OperatorStrcatContext context)
        {
            return base.VisitOperatorStrcat(context);
        }


        public override T VisitVarOrExp([NotNull] LuaParser.VarOrExpContext context)
        {
            return base.VisitVarOrExp(context);
        }
        public override T VisitString([NotNull] LuaParser.StringContext context)
        {//第三步替换普通中文，已经替换，屏蔽之
            //return base.VisitString(context);
            var patten = "[\u4e00-\u9fa5]";
            var strList = Regex.Matches(context.GetText(), patten);
            if (strList.Count > 0)
            {
                var index = getLanguageKey();
                _file.Rewriter.Replace(context.start, context.stop, "LanguagesManager.GetFormatString(" + index + ")");
                writeLanguageKey(index,context.GetText().Replace("\"", ""), ReplaceManager.curUrl);
            }
            return base.VisitString(context);
        }

        private int languageIndex = 9990575;//配置表中当前最大值
        public int getLanguageKey(bool next = true)
        {
            if(next)
            {
                num++;
                languageIndex++;//判断配置表
                var t = table.getValueByKey("entry", languageIndex.ToString());
                while (t != null)
                {
                    //Log.Info("表中包含index:" + languageIndex);
                    languageIndex++;
                    t = table.getValueByKey("entry", languageIndex.ToString());
                }
            }
            return languageIndex;
        }

        private void writeLanguageKey(int key,string content,string fileUrl)
        {
            content = content.Replace("%s", "%s%");
            GDKApplication.writeLanguageString("\r\n" + key + "\tclient\t" + content);
            Debug.Log(key+":"+content+" 所属文件："+fileUrl);
        }
    }
}
