using AntlrDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using UnityEngine;
using Antlr4.Runtime.Tree;

namespace Assets.Editor.GDK.files.Parser.lua
{
    class VisitorLua <T> : AntlrDLL.LuaBaseVisitor<T>
    {
        public override T VisitVarOrExp([NotNull] LuaParser.VarOrExpContext context)
        {
            //Debug.Log(node.GetText());
            return base.VisitVarOrExp(context);
        }
        public override T VisitExp([NotNull] LuaParser.ExpContext context)
        {
            if( context.operatorStrcat() != null)
            {

            }
            var strArr = context.GetRuleContexts<LuaParser.StringContext>();
            if (strArr.Length != 0)
            {
                foreach (var item in strArr)
                {
                    Debug.Log(item.GetText());
                }
            }
            return base.VisitExp(context);
        }
        public override T VisitString([NotNull] LuaParser.StringContext context)
        {
            return base.VisitString(context);
        }
        protected internal override bool ShouldVisitNextChild(IRuleNode node, T currentResult)
        {
            if(node.GetText() == "print")
            {
                return false;
            }
            return true;
        }
    }

}
