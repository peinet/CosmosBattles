/********************************************************************
    created:	2018/01/31 16:04:24
    filename: 	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK\ItemTableProxy.cs
    file path:	C:\workspaceUnity\sun\client\SUNClient\Assets\Editor\GDK
    author:		chens
    
    purpose:	
*********************************************************************/
using Assets.Editor.GDK.files;
using Assets.Editor.GDK.files.Parser;
using Assets.Editor.GDK.files.Parser.XLSX;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
    [InitializeOnLoad]
    [System.Serializable]
    class ItemTableProxy
    {
        private  int WHAT_IS_THIS = 1;
        private  string inputText = "";
        private  int delayTime = 0;
        private List<Dictionary<string,string>> itemTable = null;
        private FileBase file;
        private  ItemRender[] itemRenderArr = null;
        private  List<ItemRender> itemRenderVec = new List<ItemRender>();
        private  CommonWindow window;
        private  int gridSelectIndex = -1;
        private static ItemTableProxy _instance;
        int SHOW_ITEM_NUM = 50;

        public ItemTableProxy()
        {
        }
        public void showItemGrid()
        {
            CommonWindow.varDic["itemNum"] = "1";
            CommonWindow.varDic["itemID"] = "";
            CommonWindow.varDic["itemGrid"] = 0;
            delayTime = 0;
            //itemTable = ExcelTableManager.getTable("item");
            window = CommonWindow.show(() =>
            {
                GUILayout.BeginHorizontal(GUILayout.MinWidth(435));
                GUILayout.Label("道具ID或者名字", GUILayout.Width(90));
                CommonWindow.varDic["itemID"] = GUILayout.TextField((String)CommonWindow.varDic["itemID"], GUILayout.MinWidth(70));
                GUILayout.Space(30);
                GUILayout.Label("数量",GUILayout.Width(50));
                CommonWindow.varDic["itemNum"] = GUILayout.TextField((String)CommonWindow.varDic["itemNum"], GUILayout.MinWidth(70));
                if(GUILayout.Button("发送", GUILayout.Width(100)))
                {
                    if ((string)CommonWindow.varDic["itemID"] == "") return;
                    var cmd = 3;
                    if (Convert.ToInt32(CommonWindow.varDic["itemID"]) < 100) cmd = 1;
                    GMCommandProxy.sendGMCommand(cmd, Convert.ToInt32(CommonWindow.varDic["itemID"]), Convert.ToInt32(CommonWindow.varDic["itemNum"]), WHAT_IS_THIS);
                    WHAT_IS_THIS++;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("读取配置表", GUILayout.Width(100)))
                {
                    //itemTable = ExcelTableManager.getTable("item");
                    file = FileManager.getInstance().getFile(Application.dataPath + "/../../../../config/tables/item.xlsx", true , true);
                }
                GUILayout.Label("读取配置表后支持模糊搜索,最多显示SHOW_ITEM_NUM个，否则卡成狗");
                GUILayout.EndHorizontal();
                if(file != null && itemTable == null)
                {
                    itemTable = (file as ParserXLSX).getTableList("entry");
                }

                if (itemTable  != null && (String)CommonWindow.varDic["itemID"] != inputText)
                {// && (String)CommonWindow.varDic["itemID"] != "")
                    inputText = (String)CommonWindow.varDic["itemID"];
                    delayTime = 0;
                    //window.setUpdateCall(updateTime);这种计时方式太扯淡
                    refreshGrid();//限制数量SHOW_ITEM_NUM以后立即刷新看起来还可以
                }
                if (itemRenderArr != null)
                {
                    CommonWindow.varDic["itemGrid"] = GUILayout.SelectionGrid(Convert.ToInt32(CommonWindow.varDic["itemGrid"]),
                       itemRenderArr, 3);
                    if(gridSelectIndex != Convert.ToInt32(CommonWindow.varDic["itemGrid"]))
                    {
                        gridSelectIndex = Convert.ToInt32(CommonWindow.varDic["itemGrid"]);
                        CommonWindow.varDic["itemID"] = inputText = itemRenderArr[gridSelectIndex].entry;
                    }
                }
            });
        }

        private  void refreshGrid()
        {
            List<Dictionary<string,string>> matchList = new List<Dictionary<string, string>>();
            if(itemTable != null)
            {
                string searchStr = (string)CommonWindow.varDic["itemID"];
                for (int i = 0; i < itemTable.Count; i++)
                {
                    if(itemTable[i]["entry"].Contains(searchStr)|| itemTable[i]["name11"].Contains(searchStr))
                    {
                        matchList.Add(itemTable[i]);
                    }
                    if(matchList.Count >= SHOW_ITEM_NUM)
                    {
                        break;
                    }
                }
            }
            if (matchList != null)
            {
                itemRenderArr = new ItemRender[matchList.Count];
                for (int i = 0; i < matchList.Count; i++)
                {
                    if (itemRenderVec.Count == i)
                    {
                        itemRenderVec.Add(new ItemRender());
                    }
                    itemRenderArr[i] = itemRenderVec[i];
                    itemRenderArr[i].refresh(matchList[i]);
                }
            }
        }
        private  void updateTime()
        {
            if (delayTime > 2)
            {
                refreshGrid();
                window.setUpdateCall(null);
            }
            delayTime += 1;
        }
    }


    class ItemRender:GUIContent
    {
        public string entry = "";
        public string name = "";
        public void refresh(Dictionary<string, string> item)
        {
            entry = item["entry"];
            name = item["name11"];
            text = (string)item["entry"] + "   " +(string)item["name11"];
        }
    }
}
