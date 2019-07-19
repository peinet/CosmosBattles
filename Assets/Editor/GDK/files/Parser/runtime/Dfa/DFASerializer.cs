// Copyright (c) Terence Parr, Sam Harwell. All Rights Reserved.
// Licensed under the BSD License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;

namespace Antlr4.Runtime.Dfa
{
    /// <summary>A DFA walker that knows how to dump them to serialized strings.</summary>
    public class DFASerializer
    {
        [NotNull]
        private readonly DFA dfa;

        [NotNull]
        private readonly IVocabulary vocabulary;

        [Nullable]
        internal readonly string[] ruleNames;

        [Nullable]
        internal readonly ATN atn;

        [System.ObsoleteAttribute(@"Use DFASerializer(DFA, Antlr4.Runtime.IVocabulary) instead.")]
        public DFASerializer(DFA dfa, string[] tokenNames)
            : this(dfa, Vocabulary.FromTokenNames(tokenNames), null, null)
        {
        }

        public DFASerializer(DFA dfa, IVocabulary vocabulary)
            : this(dfa, vocabulary, null, null)
        {
        }

        public DFASerializer(DFA dfa, IRecognizer parser)
            : this(dfa, parser != null ? parser.Vocabulary : Vocabulary.EmptyVocabulary, parser != null ? parser.RuleNames : null, parser != null ? parser.Atn : null)
        {
        }

        [System.ObsoleteAttribute(@"Use DFASerializer(DFA, Antlr4.Runtime.IVocabulary, string[], Antlr4.Runtime.Atn.ATN) instead.")]
        public DFASerializer(DFA dfa, string[] tokenNames, string[] ruleNames, ATN atn)
            : this(dfa, Vocabulary.FromTokenNames(tokenNames), ruleNames, atn)
        {
        }

        public DFASerializer(DFA dfa, IVocabulary vocabulary, string[] ruleNames, ATN atn)
        {
            this.dfa = dfa;
            this.vocabulary = vocabulary;
            this.ruleNames = ruleNames;
            this.atn = atn;
        }

        public override string ToString()
        {
            if (dfa.s0.Get() == null)
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            if (dfa.states != null)
            {
                List<DFAState> states = new List<DFAState>(dfa.states.Values);
                states.Sort(new _IComparer_103());
                foreach (DFAState s in states)
                {
                    IEnumerable<KeyValuePair<int, DFAState>> edges = s.EdgeMap;
                    IEnumerable<KeyValuePair<int, DFAState>> contextEdges = s.ContextEdgeMap;
                    foreach (KeyValuePair<int, DFAState> entry in edges)
                    {
                        if ((entry.Value == null || entry.Value == ATNSimulator.Error) && !s.IsContextSymbol(entry.Key))
                        {
                            continue;
                        }
                        bool contextSymbol = false;
                        buf.Append(GetStateString(s)).Append("-").Append(GetEdgeLabel(entry.Key)).Append("->");
                        if (s.IsContextSymbol(entry.Key))
                        {
                            buf.Append("!");
                            contextSymbol = true;
                        }
                        DFAState t = entry.Value;
                        if (t != null && t.stateNumber != int.MaxValue)
                        {
                            buf.Append(GetStateString(t)).Append('\n');
                        }
                        else
                        {
                            if (contextSymbol)
                            {
                                buf.Append("ctx\n");
                            }
                        }
                    }
                    if (s.IsContextSensitive)
                    {
                        foreach (KeyValuePair<int, DFAState> entry_1 in contextEdges)
                        {
                            buf.Append(GetStateString(s)).Append("-").Append(GetContextLabel(entry_1.Key)).Append("->").Append(GetStateString(entry_1.Value)).Append("\n");
                        }
                    }
                }
            }
            string output = buf.ToString();
            if (output.Length == 0)
            {
                return null;
            }
            //return Utils.sortLinesInString(output);
            return output;
        }

        private sealed class _IComparer_103 : IComparer<DFAState>
        {
            public _IComparer_103()
            {
            }

            public int Compare(DFAState o1, DFAState o2)
            {
                return o1.stateNumber - o2.stateNumber;
            }
        }

        protected internal virtual string GetContextLabel(int i)
        {
            if (i == PredictionContext.EmptyFullStateKey)
            {
                return "ctx:EMPTY_FULL";
            }
            else
            {
                if (i == PredictionContext.EmptyLocalStateKey)
                {
                    return "ctx:EMPTY_LOCAL";
                }
            }
            if (atn != null && i > 0 && i <= atn.states.Count)
            {
                ATNState state = atn.states[i];
                int ruleIndex = state.ruleIndex;
                if (ruleNames != null && ruleIndex >= 0 && ruleIndex < ruleNames.Length)
                {
                    return "ctx:" + i.ToString() + "(" + ruleNames[ruleIndex] + ")";
                }
            }
            return "ctx:" + i.ToString();
        }

        protected internal virtual string GetEdgeLabel(int i)
        {
            return vocabulary.GetDisplayName(i);
        }

        internal virtual string GetStateString(DFAState s)
        {
            if (s == ATNSimulator.Error)
            {
                return "ERROR";
            }
            int n = s.stateNumber;
            string stateStr = "s" + n;
            if (s.IsAcceptState)
            {
                if (s.predicates != null)
                {
                    stateStr = ":s" + n + "=>" + Arrays.ToString(s.predicates);
                }
                else
                {
                    stateStr = ":s" + n + "=>" + s.Prediction;
                }
            }
            if (s.IsContextSensitive)
            {
                stateStr += "*";
                foreach (ATNConfig config in s.configs)
                {
                    if (config.ReachesIntoOuterContext)
                    {
                        stateStr += "*";
                        break;
                    }
                }
            }
            return stateStr;
        }
    }
}
