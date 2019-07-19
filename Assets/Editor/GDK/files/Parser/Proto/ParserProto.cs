/********************************************************************
	created:	2018/02/26 0:21:45
	author:		chens
	purpose:	
*********************************************************************/
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using AntlrDLL;
using Antlr4.Runtime;
using Assets.Editor.GDK.files.Parser.Proto;

namespace Assets.Editor.GDK.files.Parser.proto
{
    class ParserProto : ParserBase
    {
        public override ParserRuleContext GetASTree()
        {
            throw new NotImplementedException();
        }

        public override TokenStreamRewriter GetRewriter()
        {
            throw new NotImplementedException();
        }

        protected override void startParse()
        {
            var steam = new AntlrInputStream(FileString);
            var lexer = new Protobuf3Lexer(steam);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Protobuf3Parser(tokens);
            parser.proto().EnterRule(new ListenerProto());

        }
    }
}
