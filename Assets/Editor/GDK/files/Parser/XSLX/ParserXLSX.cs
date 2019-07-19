/********************************************************************
	created:	2018/02/27 20:52:43
	author:		chens
	purpose:	
*********************************************************************/
using Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Antlr4.Runtime;
using OfficeOpenXml;

namespace Assets.Editor.GDK.files.Parser.XLSX
{
    class ParserXLSX : ParserBase , ISerializationCallbackReceiver
    {
        private List<string[]> rowList = new List<string[]>();
        private List<string> serializableList = new List<string>();

        public List<string[]> RowList
        {
            get
            {
                return rowList;
            }

            //set
            //{
            //    rowList = value;
            //}
        }
        //获取一个dic,这个dic是根据传进来的key所在行作为key,值为传进来的value
        //如下表:
        //----------------------------------------
        //  字段1  字段2  字段3  字段4  字段5  字段6
        //  entry  aaa    bbb    ccc    ddd    eee   
        //    1     a      b      c      d      e
        //   呵呵  嘿嘿   哈哈   哦哦   嗯嗯   啊啊      
        //----------------------------------------
        //getValueByKey(entry,呵呵) 返回的是 第二行为key第四行为值的字典
        public Dictionary<string, string> getValueByKey(string key,string Value)
        {
            var table = getTableList(key);
            for (int i = 0; i < table.Count; i++)
            {
                if (table[i][key] == Value)
                {
                    return table[i];
                }
            }
            return null;
        }
        //todo 有需要在写这个方法
        public Dictionary<string, string> getValueByKeys(params string[] args)
        {
            return null;
        }
        private List<Dictionary<string, string>> tempTable = new List<Dictionary<string, string>>();
        [NonSerialized]
        private string[] titleArr = null;
        //获取一个list,list每个值是一个dic  byKey作为字典的key,值是剩余的每行
        //先搜索传入来的key所在行，然后把这个byKey所在行的每个值作为dic的key
        public List<Dictionary<string,string>> getTableList(string byKey)
        {
            if (titleArr != null && titleArr.Contains(byKey)) return tempTable;
            tempTable = new List<Dictionary<string, string>>();
            titleArr = null;
            for (int i = 0; i < rowList.Count; i++)
            {
                if(titleArr == null)
                {
                    for (int j = 0; j < rowList[i].Length; j++)
                    {
                        if (rowList[i][j] == byKey)
                        {
                            titleArr = rowList[i];
                            break;
                        }
                    }
                }else
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    for (int j = 0; j < rowList[i].Length; j++)
                    {
                        dic[titleArr[j]] = rowList[i][j];
                    }
                    tempTable.Add(dic);
                }
            }
            if (titleArr == null) return null;
            return tempTable;
        }

        override protected void startParse()
        {
            parseByRead();
        }
        public void parseByRead()
        {
            if (Stream != null)
            {
                rowList.Clear();
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(Stream);
                string[] lineArr = readLine(excelReader);
                while (lineArr != null)
                {
                    rowList.Add(lineArr);
                    lineArr = readLine(excelReader);
                }
            }
        }
        //如果表中有公式则会导致卡死
        public void parseByReadWrite()
        {
            rowList.Clear();
            ExcelPackage package;
            if (Stream != null)
            {
                package = new ExcelPackage(Stream);
            }else
            {
                package = new ExcelPackage(FileInfo);
            }
            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
            int rowNum = workSheet.Dimension.End.Row;
            int columnNum = workSheet.Dimension.End.Column;
            Debug.Log("rowNum :" + rowNum + " columnNum :" + columnNum);
            for (int i = 1; i <= rowNum; i++)
            {//row和column都是从1开始的
                string[] temp = new string[columnNum];
                bool isAllColumnNull = true;
                for (int j = 1; j <= columnNum; j++)
                {
                    string value = workSheet.Cells[i, j].Value == null ? "" : workSheet.Cells[i, j].Value.ToString();
                    if (value != "")
                    {
                        isAllColumnNull = false;
                    }
                    temp[j - 1] = value;
                }
                if (isAllColumnNull == false)
                {
                    rowList.Add(temp);
                }
                else
                {
                    Debug.Log("行：" + i + "的所有字段均为空，抛弃。");
                }
            }
        }
        private static string[] readLine(IExcelDataReader excelReader)
        {
            if (excelReader.Read() == false) return null;
            string[] temp = new string[excelReader.FieldCount];
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
                temp[i] = value;
            }
            return temp;
        }

        public override void Serialize()
        {
            serializableList.Clear();
            for (int i = 0; i < rowList.Count; i++)
            {
                serializableList.Add(String.Join("@~@",rowList[i]));
            }
        }
        public override void Deserialize()
        {
            rowList.Clear();
            for (int i = 0; i < serializableList.Count; i++)
            {
                rowList.Add(serializableList[i].Split(new string[] { "@~@" }, StringSplitOptions.None));
            }
        }

        public override ParserRuleContext GetASTree()
        {
            throw new NotImplementedException();
        }

        public override TokenStreamRewriter GetRewriter()
        {
            throw new NotImplementedException();
        }
    }
}
