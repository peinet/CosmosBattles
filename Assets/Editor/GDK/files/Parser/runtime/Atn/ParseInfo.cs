// Copyright (c) Terence Parr, Sam Harwell. All Rights Reserved.
// Licensed under the BSD License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;

namespace Antlr4.Runtime.Atn
{
    /// <summary>
    /// This class provides access to specific and aggregate statistics gathered
    /// during profiling of a parser.
    /// </summary>
    /// <since>4.3</since>
    public class ParseInfo
    {
        protected internal readonly ProfilingATNSimulator atnSimulator;

        public ParseInfo(ProfilingATNSimulator atnSimulator)
        {
            this.atnSimulator = atnSimulator;
        }

        /// <summary>
        /// Gets an array of
        /// <see cref="DecisionInfo"/>
        /// instances containing the profiling
        /// information gathered for each decision in the ATN.
        /// </summary>
        /// <returns>
        /// An array of
        /// <see cref="DecisionInfo"/>
        /// instances, indexed by decision
        /// number.
        /// </returns>
        [NotNull]
        public virtual Antlr4.Runtime.Atn.DecisionInfo[] DecisionInfo
        {
            get
            {
                return atnSimulator.DecisionInfo;
            }
        }

        /// <summary>
        /// Gets the decision numbers for decisions that required one or more
        /// full-context predictions during parsing.
        /// </summary>
        /// <remarks>
        /// Gets the decision numbers for decisions that required one or more
        /// full-context predictions during parsing. These are decisions for which
        /// <see cref="DecisionInfo.LL_Fallback"/>
        /// is non-zero.
        /// </remarks>
        /// <returns>
        /// A list of decision numbers which required one or more
        /// full-context predictions during parsing.
        /// </returns>
        [return: NotNull]
        public virtual IList<int> GetLLDecisions()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            IList<int> Ll = new List<int>();
            for (int i = 0; i < decisions.Length; i++)
            {
                long fallBack = decisions[i].LL_Fallback;
                if (fallBack > 0)
                {
                    Ll.Add(i);
                }
            }
            return Ll;
        }

#if !PORTABLE || NET45PLUS
        /// <summary>
        /// Gets the total time spent during prediction across all decisions made
        /// during parsing.
        /// </summary>
        /// <remarks>
        /// Gets the total time spent during prediction across all decisions made
        /// during parsing. This value is the sum of
        /// <see cref="DecisionInfo.timeInPrediction"/>
        /// for all decisions.
        /// </remarks>
        public virtual long GetTotalTimeInPrediction()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long t = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                t += decisions[i].timeInPrediction;
            }
            return t;
        }
#endif

        /// <summary>
        /// Gets the total number of SLL lookahead operations across all decisions
        /// made during parsing.
        /// </summary>
        /// <remarks>
        /// Gets the total number of SLL lookahead operations across all decisions
        /// made during parsing. This value is the sum of
        /// <see cref="DecisionInfo.SLL_TotalLook"/>
        /// for all decisions.
        /// </remarks>
        public virtual long GetTotalSLLLookaheadOps()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long k = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                k += decisions[i].SLL_TotalLook;
            }
            return k;
        }

        /// <summary>
        /// Gets the total number of LL lookahead operations across all decisions
        /// made during parsing.
        /// </summary>
        /// <remarks>
        /// Gets the total number of LL lookahead operations across all decisions
        /// made during parsing. This value is the sum of
        /// <see cref="DecisionInfo.LL_TotalLook"/>
        /// for all decisions.
        /// </remarks>
        public virtual long GetTotalLLLookaheadOps()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long k = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                k += decisions[i].LL_TotalLook;
            }
            return k;
        }

        /// <summary>
        /// Gets the total number of ATN lookahead operations for SLL prediction
        /// across all decisions made during parsing.
        /// </summary>
        public virtual long GetTotalSLLATNLookaheadOps()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long k = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                k += decisions[i].SLL_ATNTransitions;
            }
            return k;
        }

        /// <summary>
        /// Gets the total number of ATN lookahead operations for LL prediction
        /// across all decisions made during parsing.
        /// </summary>
        public virtual long GetTotalLLATNLookaheadOps()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long k = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                k += decisions[i].LL_ATNTransitions;
            }
            return k;
        }

        /// <summary>
        /// Gets the total number of ATN lookahead operations for SLL and LL
        /// prediction across all decisions made during parsing.
        /// </summary>
        /// <remarks>
        /// Gets the total number of ATN lookahead operations for SLL and LL
        /// prediction across all decisions made during parsing.
        /// <p>
        /// This value is the sum of
        /// <see cref="GetTotalSLLATNLookaheadOps()"/>
        /// and
        /// <see cref="GetTotalLLATNLookaheadOps()"/>
        /// .</p>
        /// </remarks>
        public virtual long GetTotalATNLookaheadOps()
        {
            Antlr4.Runtime.Atn.DecisionInfo[] decisions = atnSimulator.DecisionInfo;
            long k = 0;
            for (int i = 0; i < decisions.Length; i++)
            {
                k += decisions[i].SLL_ATNTransitions;
                k += decisions[i].LL_ATNTransitions;
            }
            return k;
        }

        /// <summary>
        /// Gets the total number of DFA states stored in the DFA cache for all
        /// decisions in the ATN.
        /// </summary>
        public virtual int GetDFASize()
        {
            int n = 0;
            DFA[] decisionToDFA = atnSimulator.atn.decisionToDFA;
            for (int i = 0; i < decisionToDFA.Length; i++)
            {
                n += GetDFASize(i);
            }
            return n;
        }

        /// <summary>
        /// Gets the total number of DFA states stored in the DFA cache for a
        /// particular decision.
        /// </summary>
        public virtual int GetDFASize(int decision)
        {
            DFA decisionToDFA = atnSimulator.atn.decisionToDFA[decision];
            return decisionToDFA.states.Count;
        }
    }
}
