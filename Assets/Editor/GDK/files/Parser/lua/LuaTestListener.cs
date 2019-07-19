using AntlrDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using UnityEngine;
using Antlr4.Runtime;

namespace Assets.Editor.GDK.files.Parser.lua
{
    class LuaTestListener : LuaBaseListener
    {
        private void EnterRule(ParserRuleContext rule)
        {
            if (rule != null)
            {
                rule.EnterRule(this);
            }
        }

        public override void EnterChunk([NotNull] LuaParser.ChunkContext context)
        {
            EnterRule(context.block());
        }
        public override void EnterBlock([NotNull] LuaParser.BlockContext context)
        {
            LuaParser.StatContext[] statList = context.stat();
            foreach (var stat in statList)
            {
                EnterRule(stat.varlist());
                EnterRule(stat.explist());
                EnterRule(stat.functioncall());
                EnterRule(stat.label());
                EnterRule(stat.namelist());
                EnterRule(stat.funcname());
                EnterRule(stat.funcbody());
            }
        }
       

        public override void EnterVar([NotNull] LuaParser.VarContext context)
        {
            //Debug.Log("in var");
        }
        public override void EnterExplist([NotNull] LuaParser.ExplistContext context)
        {                                                   
            LuaParser.ExpContext[] exp = context.exp();
            foreach (var item in exp)
            {
                //item.exp();
            }
            //Debug.Log("in exp");
        }
        public override void EnterFunctioncall([NotNull] LuaParser.FunctioncallContext context)
        {
            //Debug.Log("in function");
        }
        public override void EnterLabel([NotNull] LuaParser.LabelContext context)
        {
            //Debug.Log("in label");
        }
        public override void EnterNamelist([NotNull] LuaParser.NamelistContext context)
        {
            //Debug.Log("in namelist");
        }
        public override void EnterFuncname([NotNull] LuaParser.FuncnameContext context)
        {
            //var nameList = context.NAME();
            //string str = "";
            //foreach (var item in nameList)
            //{
            //    str += item.ToString() + " ";
            //}
            //Debug.Log("函数名字"+str);
        }
        public override void EnterFuncbody([NotNull] LuaParser.FuncbodyContext context)
        {
            //Debug.Log("in funcbody");
        }
    }
}
