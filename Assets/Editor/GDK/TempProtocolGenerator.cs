/********************************************************************
	created:	2018/09/30 18:31:36
	author:		chens
	purpose:	代替python,使用c#重写协议生成
    一共分为4步：
    1，根据config/protos/client目录中的.proto生成对应的.pb到项目的Assets\Luas\Protocol\Pbc目录中,同时生成EmmyDoc对应的目录中
    2，找到config/protos/client中的5个文件 msg_common.proto,opcode.proto,msg_error.proto,table_enum.proto,table_enum2.proto生成.lua到\Assets\Luas\Protocol\Reverse目录中
    3，根据config/protos/entity目录中的.proto生成对应的.lua到\Assets\Luas\Protocol\Reverse目录中
    4，根据config/tables/Player中的配置表playerAttribute.xlsx生成对应的\Assets\Luas\TabFiles\Attribute.lua(如果这个表有修改，需要检查define.BAT_USE_Function也需要手动添加)
*********************************************************************/
using Assets.Editor.GDK.common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempProtocolGenerator
{
    private static TempProtocolGenerator _instance;

    static List<string> paths = new List<string>();
    static List<string> clientProtoFiles = new List<string>();
    static List<string> args = new List<string>();
    static int argIndex = 0;
    static List<string> commons = new List<string>();
    static int commonIndex = 0;
    static List<string> entitys = new List<string>();
    static int entityIndex = 0;
    static string reservTarget = "";
    static string pbcTarget = "";
    static string attrTarget = "";
    static bool isimmediately = false;
    static string emmyDocClientTarget = "";
    static string emmyDocEntityTarget = "";

    public static TempProtocolGenerator getInstance()
    {
        if (_instance == null) _instance = new TempProtocolGenerator();
        return _instance;
    }
    [MenuItem("UGameTools/GenProto")]
    public static void GeneratePbFile()
    {
        reservTarget = Application.dataPath + "/Luas/Protocol/Reverse/";
        pbcTarget = Application.dataPath + "/Luas/Protocol/Pbc/";
        attrTarget = Application.dataPath + "/Luas/TabFiles/Attribute.lua";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {//windows环境目录兼容
            TempProtocolGenerator.getInstance().generate(
                Application.dataPath + "/../../../../config/protos/client",
                Application.dataPath + "/../../../../config/protos/entity",
                Application.dataPath + "/../../../../config/tables/Player/playerAttribute.xlsx", true
                );
            emmyDocClientTarget = Application.dataPath + "/../../../../config/EmmyDoc/proto/client/";
            emmyDocEntityTarget = Application.dataPath + "/../../../../config/EmmyDoc/proto/entity/";
        }
        else
        {
            TempProtocolGenerator.getInstance().generate(
                Application.dataPath + "/../../../config/protos/client",
                Application.dataPath + "/../../../config/protos/entity",
                Application.dataPath + "/../../../config/tables/Player/playerAttribute.xlsx", true
                );
            emmyDocClientTarget = Application.dataPath + "/../../../config/EmmyDoc/proto/client/";
            emmyDocEntityTarget = Application.dataPath + "/../../../config/EmmyDoc/proto/entity/";
        }
    }
    [MenuItem("UGameTools/GenProtoToBrach")]
    public static void GeneratePbFileToBrach()
    {

        Scene scene = SceneManager.GetActiveScene();
        var goArr = scene.GetRootGameObjects();
        //ClientBranchManager branchManager = null;
        //for (int i = 0; i < goArr.Length; i++)
        //{
        //    branchManager = goArr[i].GetComponent<ClientBranchManager>();
        //    if(branchManager != null)
        //    {
        //        break;
        //    }
        //}
        //if (branchManager == null)
        //{
        //    UnityEngine.Debug.LogError("未找到分支组件，检查场景是否正确");
        //}
        var branchName = "None";// branchManager.GetBranchName();
        if (branchName == "None")
        {
            UnityEngine.Debug.LogWarning("当前分支选择为None,自动生成协议到主干");
            GeneratePbFile();
        }else
        {
            string brachPath = Application.dataPath + "/Branchs/" + branchName;
            if (Directory.Exists(brachPath + "/Luas") == false) Directory.CreateDirectory(brachPath + "/Luas");
            if (Directory.Exists(brachPath + "/Luas/Protocol") == false) Directory.CreateDirectory(brachPath + "/Luas/Protocol");
            if (Directory.Exists(brachPath + "/Luas/Protocol/Reverse") == false) Directory.CreateDirectory(brachPath + "/Luas/Protocol/Reverse");
            if (Directory.Exists(brachPath + "/Luas/TabFiles") == false)  Directory.CreateDirectory(brachPath + "/Luas/TabFiles");
            reservTarget = brachPath + "/Luas/Protocol/Reverse/";
            pbcTarget = brachPath + "/Luas/Protocol/Pbc/";
            attrTarget = brachPath + "/Luas/TabFiles/Attribute.lua";

            TempProtocolGenerator.getInstance().generate(
                Application.dataPath + "/../../../../config/protos/client",
                Application.dataPath + "/../../../../config/protos/entity",
                Application.dataPath + "/../../../../config/tables/Player/playerAttribute.xlsx", true
                );
        }
        
    }
    public void generate(string clientPath, string entityPath, string excelPath , bool isImm = false)
    {
        try
        {
            isimmediately = isImm;
            clientProtoFiles.Clear();
            paths.Clear();
            args.Clear();
            commons.Clear();
            entitys.Clear();
            //检查需要生成的PB
            argIndex = 0;
            string[] protoDir = { clientPath, entityPath };
            if (!Directory.Exists(pbcTarget))
                Directory.CreateDirectory(pbcTarget);
            for (int i = 0; i < protoDir.Length; i++)
            {
                Recursive(protoDir[i]);
            }


            string mutiDir = "";
            for (int i = 0; i < protoDir.Length; i++)
            {
                mutiDir += " --proto_path=" + protoDir[i];
            }

            foreach (string oneFile in clientProtoFiles)
            {
                string name = Path.GetFileName(oneFile);
                string ext = Path.GetExtension(oneFile);
                string prefix = name.Replace(ext, string.Empty);

                if (!ext.Equals(".proto")) continue;

                string argStr = " --descriptor_set_out=" + pbcTarget + prefix + ".pb" + mutiDir + " --include_imports " + oneFile;
                args.Add(argStr.ToLower());//推入list 待update时候生成
            }
           //检查需要生成的common
           commonIndex = 0;
            var fileList = Directory.GetFiles(clientPath);
            foreach (var file in fileList)
            {
                if (file.EndsWith("msg_common.proto") ||
                    file.EndsWith("opcode.proto") ||
					file.EndsWith("opcode2.proto") ||
                    file.EndsWith("msg_error.proto") ||
                    file.EndsWith("table_enum.proto") ||
                    file.EndsWith("table_enum2.proto"))
                {
                    commons.Add(file);//推入list 待update时候生成
                }
            }
            //检查需要生成的entity
            entityIndex = 0;
            fileList = Directory.GetFiles(entityPath);
            foreach (var file in fileList)
            {
                entitys.Add(file);//推入list 待update时候生成
            }

            GenAttribute(excelPath);
            if (isimmediately)
            {
                while (isimmediately == true)
                {
                    Update();
                }
            }
            else
            {
                EditorApplication.update += Update;//防止界面卡住
            }
        }
        catch (Exception e)
        {
            cancelGenerate();
            throw e;
        }
        
    }

    public static void Update()
    {
        //根据protos中client和entity中的.proto生成.pb
        if (args.Count > argIndex)
        {
            if (EditorUtility.DisplayCancelableProgressBar(
                "步骤一："+argIndex + "/" + args.Count, 
                "生成=>" + Regex.Match(args[argIndex], 
                "pbc/(.*?\\.pb)").Groups[1],
                (float)argIndex / args.Count)
                )
            {//取消操作
                cancelGenerate();
                return;
            }
            var arg = args[argIndex];
            ExecuteOne(arg.ToLower());

            if (clientProtoFiles.Count > argIndex)
            {
                if (Regex.IsMatch(clientProtoFiles[argIndex], "protos/client/.*\\.proto") == true)
                {
                    GenEmmyDoc(clientProtoFiles[argIndex], emmyDocClientTarget);
                }
                else
                {
                    GenEmmyDoc(clientProtoFiles[argIndex], emmyDocEntityTarget);
                }
            }
            argIndex++;
        }else if (commons.Count > commonIndex)
        {//根据protos/client中的.proto生成几个lua文件，就5个。
            if (EditorUtility.DisplayCancelableProgressBar("步骤二：" + commonIndex + "/" + commons.Count, "生成" + commons[commonIndex], (float)commonIndex / commons.Count))
            {//取消操作
                cancelGenerate();
                return;
            }
            var reserve = commons[commonIndex];
            GenCommon(reserve);
            commonIndex++;
        }else if (entitys.Count > entityIndex)
        { //根据protos/entity生成lua到Reverse目录中
            if (EditorUtility.DisplayCancelableProgressBar("步骤三：" + entityIndex + "/" + entitys.Count, "生成" + entitys[entityIndex], (float)entityIndex / entitys.Count))
            {//取消操作
                cancelGenerate();
                return;
            }
            GenEntity(entitys[entityIndex]);
            entityIndex++;
        }


        if (args.Count <= argIndex && commons.Count <= commonIndex && entitys.Count <= entityIndex)
        {
            cancelGenerate();
        }
    }

    static void cancelGenerate()
    {
        EditorApplication.update -= Update;
        EditorUtility.ClearProgressBar();
        isimmediately = false;
    }

    static void ExecuteOne(string args)
    {
        ProcessStartInfo info = new ProcessStartInfo();
        info.CreateNoWindow = true;
        info.RedirectStandardOutput = true;
        info.RedirectStandardInput = true;
        info.RedirectStandardError = true;
        info.UseShellExecute = false;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            info.FileName = Application.dataPath + "/Editor/GDK/protoc.exe";
        }
        else
        {
            info.FileName = Application.dataPath + "/Editor/GDK/protoc";
        }
        info.Arguments = args;
        Process pro = Process.Start(info);
        StreamReader sr = pro.StandardError;
        string str = sr.ReadToEnd();
        if (!string.IsNullOrEmpty(str))
        {
            UnityEngine.Debug.LogError("message:" + str);
        }
        pro.WaitForExit();
        sr.Close();//关闭流
    }
    public static void TestGenEmmyDoc()
    {

        if (emmyDocClientTarget == "") emmyDocClientTarget = Application.dataPath + "/../../../../config/EmmyDoc/proto/client/";
        if (emmyDocEntityTarget == "") emmyDocEntityTarget = Application.dataPath + "/../../../../config/EmmyDoc/proto/entity/";

        Recursive(Application.dataPath + "/../../../../config/protos/client");

        for (int i = 0; i < clientProtoFiles.Count; i++)
        {
            GenEmmyDoc(clientProtoFiles[i], emmyDocClientTarget);
        }
        //GenEmmyDoc(Application.dataPath + "/../../../../config/protos/client/msg_activity.proto", 0);

    }
    static void GenEmmyDoc(string path, string targetPath)
    {
        if (Directory.Exists(targetPath) == false)
        {
            return;
        }
        string targetStr = "";
        if (targetPath == emmyDocClientTarget)
        {
            targetStr = "---此文件由C#根据protos/client中对应的.proto自动生成,禁止手动修改\r\n";
        }
        else if(targetPath == emmyDocEntityTarget)
        {
            targetStr = "---此文件由C#根据protos/entity中对应的.proto自动生成,禁止手动修改\r\n";
        }
        var info = new FileInfo(path);
        var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        var fileString = sr.ReadToEnd();
        try
        {
            if(Regex.IsMatch(fileString,"message") == false)
            {
                return;
            }
            var messageList = Regex.Matches(fileString, "([//\\s\\S]*?)message[\\s]*(\\w+)?[\\s]*\n*?{([\\s\\S\\n]*?)}");
            for (int i = 0; i < messageList.Count; i++)
            {
                string classCommont = Regex.Replace(messageList[i].Groups[1].ToString(), "\\s", "");
                targetStr += "---@class " + messageList[i].Groups[2].ToString() + " " + classCommont + "\r\n";
                var body = messageList[i].Groups[3].ToString();
                var lineList = Regex.Matches(body, ".*\n");//把结构体中的每一行拿出来
                string commont = "";
                foreach (var item in lineList)
                {
                    if (String.IsNullOrEmpty(Regex.Match(item.ToString(), "^[\\s]*?//").ToString()) == false)
                    {//注释
                        commont += Regex.Replace(item.ToString(), "[\\s]*?//(.*)?\\n", "$1 ");
                        continue;
                    }
                    var keyList = Regex.Matches(item.ToString(), "(required|optional|repeated)\\s*(\\w+)\\s*(\\w+)\\s*=\\s*(\\d*).*;(.*)");
                    if (keyList.Count > 0)
                    {//替换每个key 边分析边生成目标字符串
                        string fieldType = "";
                        if (keyList[0].Groups[2].ToString() == "double" ||
                            keyList[0].Groups[2].ToString() == "float" ||
                            keyList[0].Groups[2].ToString() == "int32" ||
                            keyList[0].Groups[2].ToString() == "uint32" ||
                            keyList[0].Groups[2].ToString() == "int64" ||
                            keyList[0].Groups[2].ToString() == "uint64" ||
                            keyList[0].Groups[2].ToString() == "sint32" ||
                            keyList[0].Groups[2].ToString() == "sing64" ||
                            keyList[0].Groups[2].ToString() == "fixed32" ||
                            keyList[0].Groups[2].ToString() == "fixed64" ||
                            keyList[0].Groups[2].ToString() == "sfixed32" ||
                            keyList[0].Groups[2].ToString() == "sfixed64")
                        {
                            fieldType = "number";
                            if (keyList[0].Groups[1].ToString() != "repeated")
                            {
                                fieldType += "  @";//number后面的注释如果是数字会导致Emmylua识别为默认值？还是数组？还是说明其他的BUG？
                            }
                        }
                        else if (keyList[0].Groups[2].ToString() == "string" ||
                            keyList[0].Groups[2].ToString() == "bytes")
                        {
                            fieldType = "string";
                        }
                        else if (keyList[0].Groups[2].ToString() == "bool")
                        {
                            fieldType = "boolean";
                        }
                        else
                        {
                            fieldType = keyList[0].Groups[2].ToString();//自定义结构
                        }
                        if (keyList[0].Groups[1].ToString() == "repeated")
                        {
                            fieldType += "[]";
                        }
                        if(String.IsNullOrEmpty(keyList[0].Groups[4].ToString()) == false)
                        {
                            commont += keyList[0].Groups[5].ToString();
                        }
                        commont = Regex.Replace(commont, "\\s", "");//用之前把注释里面看不见的字符去掉。
                        targetStr += "---@field " + keyList[0].Groups[3].ToString() + " " + fieldType + " " + commont + "\r\n";
                        commont = "";
                    }
                }
                targetStr += "\r\n\r\n";
            }
            File.WriteAllText(targetPath + Regex.Replace(info.Name, ".proto", ".lua"), targetStr);
        }
        catch (Exception)
        {
            //.Error("解析错误:" + path);
        }
        sr.Close();
    }
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            clientProtoFiles.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }


    static void GenCommon(string path)
    {
        var info = new FileInfo(path);
        var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        var fileString = sr.ReadToEnd();
        sr.Close();
        var luaStr = Regex.Replace(fileString, "enum[\\s]*(\\w+)?[\\s]*\n*?{", "---@class $1\n$1={");
        luaStr = Regex.Replace(luaStr, "syntax.*", "");
        luaStr = Regex.Replace(luaStr, "package.*", "");
        luaStr = Regex.Replace(luaStr, "//", "---");
        luaStr = Regex.Replace(luaStr, ";", ",");
        stream.Close();
        File.WriteAllText(reservTarget + Regex.Replace(info.Name, ".proto", ".lua"), luaStr);
    }
    private static void GenEntity(string path)
    {
        var info = new FileInfo(path);
        var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        var fileString = sr.ReadToEnd();
        try
        {
            string reverseStr = "---此文件由C#根据protos/entity中对应的.proto自动生成,禁止手动修改\r\n";
            string functionStr = "";
            var messageList = Regex.Matches(fileString, "message[\\s]*(\\w+)?[\\s]*\n*?{([\\s\\S\\n]*)}");
            for (int i = 0; i < messageList.Count; i++)
            {
                var name = info.Name.Replace(".proto", "");//messageList[i].Groups[1].ToString();
                reverseStr += "---@class " + name + "Reverse" + "\n" + name + "Reverse={";
                functionStr += "---@class " + name + "Function" + "\n" + name + "Function={";
                var body = messageList[i].Groups[2].ToString();
                var lineList = Regex.Matches(body, ".*\n");//把结构体中的每一行拿出来
                foreach (var item in lineList)
                {
                    var keyList = Regex.Matches(item.ToString(), "(required|optional|repeated)\\s*(\\w+)\\s*(\\w+)\\s*=\\s*(\\d*).*;");
                    if (keyList.Count > 0)
                    {//替换每个key 边分析边生成目标字符串
                     //bool isArr = keyList[0].Groups[1].ToString() == "repeated";
                        var keyName = keyList[0].Groups[3].ToString();
                        var keyValue = keyList[0].Groups[4].ToString();
                        reverseStr += "\n\t[" + keyValue + "] = \"" + keyName + "\",";
                        functionStr += "\n\t[" + keyValue + "] = \"Set" + keyName.ToUpper() + "\",";
                    }
                    else if (String.IsNullOrEmpty(Regex.Match(item.ToString(), "[\\s]*?//").ToString()) == false)
                    {//替换注释
                        var itemStr = Regex.Replace(item.ToString(), "[\\s]*?//(.*)\\r*\\n", "\n\t---$1");
                        reverseStr += itemStr;
                        functionStr += itemStr;
                    }
                }
                reverseStr += "\n}";
                functionStr += "\n}";
            }
            File.WriteAllText(reservTarget + Regex.Replace(info.Name, ".proto", "Reverse.lua"), reverseStr + "\n" + functionStr);
        }
        catch (Exception)
        {
            //Log.Error("解析错误:" + path);
        }
        sr.Close();
    }

    private static void GenAttribute(string excelPath)
    {
        var list = GDKApplication.ReadEXCEL(excelPath);
        if (list.Count <= 0)
        {
            //Debugger.LogError("tables/Player/playerAttribute.xlsx读取错误");
            return;
        }
        List<AttributeTable> attrList = new List<AttributeTable>();
        //0 描述 
        //1 entry attribute val description Round type coefficient beizhu sortIndex
        //2 类型
        //3 skipserver还是skipall
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 3 && string.IsNullOrEmpty(list[i][0]) == false )
            {
                var table = new AttributeTable();
                table.setData(list[i]);
                attrList.Add(table);
            }
        }

        attrList.Sort((x, y) =>
        {
            if (x.Type > y.Type)
            {
                return 1;
            }
            else if (x.Type < y.Type)
            {
                return -1;
            }
            else
            {
                if (x.Val > y.Val)
                {
                    return 1;
                }
                else if (x.Val < y.Val)
                {
                    return -1;
                }
            }
            return 0;
        });
        //为了兼顾原来的格式，暂时这么写。
        string baseStr = "---@class\nBASE={\n";
        string baseReverseStr = "---@class\nBASEReverse={\n";
        string baseFunctionStr = "---@class\nBASEFunction={\n";
        string batStr = "---@class\nBAT={\n";
        string batReverseStr = "---@class\nBATReverse={\n";
        string batFunctionStr = "---@class\nBATFunction={\n";
        string modStr = "---@class\nMOD={\n";
        string modReverseStr = "---@class\nMODReverse={\n";
        string modFunctionStr = "---@class\nMODFunction={\n";
        for (int i = 0; i < attrList.Count; i++)
        {
            if (attrList[i].Type == 0)
            {
                baseStr += "    ---" + attrList[i].Description + "\n    " + attrList[i].Head + "_" + attrList[i].Key + " = " + attrList[i].Val + ",\n";
                var fuckerRuleStr = Regex.Replace(attrList[i].Key, "[A-Z]", match => { return "_" + match.ToString(); });
                baseReverseStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] = \"" + attrList[i].Head + fuckerRuleStr.ToLower() + "\",\n";
                baseFunctionStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] = \"Set" + attrList[i].Key.ToUpper() + "\",\n";
            }
            if (attrList[i].Type == 1)
            {
                batStr += "    ---" + attrList[i].Description + "\n    " + attrList[i].Head + "_" + attrList[i].Key + " = " + attrList[i].Val + ",\n";
                batReverseStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] =\"" + attrList[i].Head + "_" + attrList[i].Key + "\",\n";
                batFunctionStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] = \"Set" + attrList[i].Key.ToUpper() + "\",\n";
            }
            if (attrList[i].Type == 2)
            {
                modStr += "    ---" + attrList[i].Description + "\n    " + attrList[i].Head + "_" + attrList[i].Key + " = " + attrList[i].Val + ",\n";
                modReverseStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] = \"" + attrList[i].Head + "_" + attrList[i].Key + "\",\n";
                modFunctionStr += "    ---" + attrList[i].Description + "\n    [" + attrList[i].Val + "] = \"Set_" + attrList[i].Key.ToUpper() + "\",\n";
            }
        }
        baseStr += "}\n";
        baseReverseStr += "}\n";
        baseFunctionStr += "}\n";
        batStr += "}\n";
        batReverseStr += "}\n";
        batFunctionStr += "}\n";
        modStr += "}\n";
        modReverseStr += "}\n";
        modFunctionStr += "}\n";
        File.WriteAllText(attrTarget,
            "--此脚本是根据config / tables / Player中的配置表playerAttribute.xlsx生成, 禁止手动修改\n--如果这个表有修改，需要检查define.BAT_USE_Function也需要手动添加\n" +
            baseStr +
            batStr +
            modStr +
            baseReverseStr +
            batReverseStr +
            modReverseStr +
            baseFunctionStr +
            batFunctionStr +
            modFunctionStr
            );
    }
}


//表的映射，每个对象对应表中的一个行
struct AttributeTable
{
    //0BASE 1MOD 2BAT
    private int type;

    //BASE.XXX
    private string attribute;
    //去掉BASE.只留XXX
    private string key;
    //去掉XXX只留BASE
    private string head;

    private int val;
    //注释
    private string description;

    public string Key
    {
        get
        {
            return key;
        }
    }

    public int Type
    {
        get
        {
            return type;
        }
    }

    public string Attribute
    {
        get
        {
            return attribute;
        }

    }

    public int Val
    {
        get
        {
            return val;
        }

    }

    public string Description
    {
        get
        {
            return description;
        }

    }


    public string Head
    {
        get
        {
            return head;
        }
    }

    public void setData(string[] v)
    {
        attribute = v[1];
        var keys = attribute.Split(new char[1] { '.' });
        if (keys[0].ToString() == "BASE")
        {
            type = 0;

        }
        if (keys[0].ToString() == "BAT")
        {
            type = 1;
        }
        if (keys[0].ToString() == "MOD")
        {
            type = 2;
        }
        head = keys[0];
        key = keys[1];
        Int32.TryParse(v[2], out val);
        val += 1;//在lua里面没有0
        description = v[3];
    }
}
