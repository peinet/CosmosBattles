using Antlr4.Runtime;
using AntlrDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using UnityEngine;

namespace Assets.Editor.GDK.files.Parser.Proto
{
    class ListenerProto : Protobuf3BaseListener
    {
        public override void EnterProto([NotNull] Protobuf3Parser.ProtoContext context)
        {
            context.syntax().EnterRule(this);
            var topLevelDef = context.topLevelDef();
            foreach (var item in topLevelDef)
            {
                item.EnterRule(this);
            }

        }
        public override void EnterSyntax([NotNull] Protobuf3Parser.SyntaxContext context)
        {
        }
        public override void EnterTopLevelDef([NotNull] Protobuf3Parser.TopLevelDefContext context)
        {
            Debug.Log(context.enumDefinition().enumName());
            context.enumDefinition().EnterRule(this);
        }
        public override void EnterEnumDefinition([NotNull] Protobuf3Parser.EnumDefinitionContext context)
        {
            context.enumBody().EnterRule(this);
        }
        public override void EnterEnumBody([NotNull] Protobuf3Parser.EnumBodyContext context)
        {
            var enumField = context.enumField();
            foreach (var item in enumField)
            {
                item.EnterRule(this);
            }
        }
        public override void EnterEnumField([NotNull] Protobuf3Parser.EnumFieldContext context)
        {
            Debug.Log(context.Ident()+"  "+ context.IntLit());
            
        }


    }
}
