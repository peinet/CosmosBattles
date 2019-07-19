/********************************************************************
	created:	2018/03/12 15:24:51
	author:		chens
	purpose:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AntlrDLL;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Assets.Editor.GDK.files.Parser.lua
{
    class LuaFileParser : ParserBase
    {
        private LuaParser.ChunkContext _tree;
        private TokenStreamRewriter _rewriter;

        public override ParserRuleContext GetASTree()
        {
            return _tree;
        }

        public override TokenStreamRewriter GetRewriter()
        {
            return _rewriter;
        }

        protected override void startParse()
        {
            var steam = new AntlrInputStream(FileString);
            var lexer = new LuaLexer(steam);
            var tokens = new CommonTokenStream(lexer);
            var parser = new LuaParser2(tokens);
            _tree = parser.chunk();
            _rewriter = new TokenStreamRewriter(tokens);
            //ParseTreeWalker walker = new ParseTreeWalker(new ListenerLua());
            //var visitor = new VisitorLua<LuaParser.ChunkContext>();
            //visitor.Visit(tree);
            //tree.Accept<LuaParser.ChunkContext>(visitor);

        }

    }
    class LuaParser2 : LuaParser
    {
        public LuaParser2(ITokenStream input) : base(input)
        {

            ErrorHandler = new LuaErrorHandler();
        }
    }

    class LuaErrorHandler : IAntlrErrorStrategy
    {
        public bool InErrorRecoveryMode(Antlr4.Runtime.Parser recognizer)
        {
            //Debug.Log("InErrorRecoveryMode");
            return false;
        }

        public void Recover(Antlr4.Runtime.Parser recognizer, RecognitionException e)
        {
            //Debug.Log("Recover");
        }

        [return: NotNull]
        public IToken RecoverInline(Antlr4.Runtime.Parser recognizer)
        {
            //Debug.Log("RecoverInline");
            return null;
        }

        public void ReportError(Antlr4.Runtime.Parser recognizer, RecognitionException e)
        {
            //Debug.Log("ReportError");
        }

        public void ReportMatch(Antlr4.Runtime.Parser recognizer)
        {
            //Debug.Log("ReportMatch");
        }

        public void Reset(Antlr4.Runtime.Parser recognizer)
        {
            //Debug.Log("Reset");
        }

        public void Sync(Antlr4.Runtime.Parser recognizer)
        {
            //Debug.Log("Sync");
        }
    }

}
