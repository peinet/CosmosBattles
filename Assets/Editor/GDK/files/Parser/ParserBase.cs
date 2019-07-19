/********************************************************************
	created:	2018/02/26 0:21:31
	author:		chens
	purpose:	
*********************************************************************/
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Editor.GDK.files.Parser
{

    abstract class ParserBase : FileBase
    {
        private ParserRuleContext _asTree;
        private TokenStreamRewriter _rewriter;
        //抽象类需要解析后的语法树，当解析完成以后返回给抽象类。
        abstract public ParserRuleContext GetASTree();
        //抽象类需要一个重写者，当解析完成以后需要把token给重写者，然后返回给抽象类,警告：如果返回空，则无法保存修改内容。
        abstract public TokenStreamRewriter GetRewriter();

        public ParserRuleContext ASTree
        {
            get
            {
                return GetASTree();
            }

        }
        public TokenStreamRewriter Rewriter
        {
            get
            {
                return GetRewriter();
            }
        }
        public void Save()
        {
            Save(GetRewriter().GetText());
        }
    }
}
