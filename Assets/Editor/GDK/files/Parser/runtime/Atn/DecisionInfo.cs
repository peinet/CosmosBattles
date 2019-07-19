// Copyright (c) Terence Parr, Sam Harwell. All Rights Reserved.
// Licensed under the BSD License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
#if !PORTABLE || NET45PLUS
using Stopwatch = System.Diagnostics.Stopwatch;
#endif

namespace Antlr4.Runtime.Atn
{
    /// <summary>This class contains profiling gathered for a particular decision.</summary>
    /// <remarks>
    /// This class contains profiling gathered for a particular decision.
    /// <p>
    /// Parsing performance in ANTLR 4 is heavily influenced by both static factors
    /// (e.g. the form of the rules in the grammar) and dynamic factors (e.g. the
    /// choice of input and the state of the DFA cache at the time profiling
    /// operations are started). For best results, gather and use aggregate
    /// statistics from a large sample of inputs representing the inputs expected in
    /// production before using the results to make changes in the grammar.</p>
    /// </remarks>
    /// <since>4.3</since>
    public class DecisionInfo
    {
        /// <summary>
        /// The decision number, which is an index into
        /// <see cref="ATN.decisionToState"/>
        /// .
        /// </summary>
        public readonly int decision;

        /// <summary>
        /// The total number of times
        /// <see cref="ParserATNSimulator.AdaptivePredict(ITokenStream, int, ParserRuleContext)"/>
        /// was
        /// invoked for this decision.
        /// </summary>
        public long invocations;

#if !PORTABLE || NET45PLUS
        /// <summary>
        /// The total time spent in
        /// <see cref="ParserATNSimulator.AdaptivePredict(ITokenStream, int, ParserRuleContext)"/>
        /// for
        /// this decision, in nanoseconds.
        /// <p>
        /// The value of this field is computed by <see cref="Stopwatch"/>,
        /// and is not adjusted to compensate for JIT
        /// and/or garbage collection overhead. For best accuracy, perform profiling
        /// in a separate process which is warmed up by parsing the input prior to
        /// profiling. If desired, call <see cref="ATNSimulator.ClearDFA()"/>
        /// to reset the DFA cache to its initial
        /// state before starting the profiling measurement pass.</p>
        /// </summary>
        public long timeInPrediction;
#endif

        /// <summary>The sum of the lookahead required for SLL prediction for this decision.</summary>
        /// <remarks>
        /// The sum of the lookahead required for SLL prediction for this decision.
        /// Note that SLL prediction is used before LL prediction for performance
        /// reasons even when
        /// <see cref="PredictionMode.Ll"/>
        /// or
        /// <see cref="PredictionMode.LlExactAmbigDetection"/>
        /// is used.
        /// </remarks>
        public long SLL_TotalLook;

        /// <summary>
        /// Gets the minimum lookahead required for any single SLL prediction to
        /// complete for this decision, by reaching a unique prediction, reaching an
        /// SLL conflict state, or encountering a syntax error.
        /// </summary>
        public long SLL_MinLook;

        /// <summary>
        /// Gets the maximum lookahead required for any single SLL prediction to
        /// complete for this decision, by reaching a unique prediction, reaching an
        /// SLL conflict state, or encountering a syntax error.
        /// </summary>
        public long SLL_MaxLook;

        /// <summary>
        /// Gets the
        /// <see cref="LookaheadEventInfo"/>
        /// associated with the event where the
        /// <see cref="SLL_MaxLook"/>
        /// value was set.
        /// </summary>
        public LookaheadEventInfo SLL_MaxLookEvent;

        /// <summary>The sum of the lookahead required for LL prediction for this decision.</summary>
        /// <remarks>
        /// The sum of the lookahead required for LL prediction for this decision.
        /// Note that LL prediction is only used when SLL prediction reaches a
        /// conflict state.
        /// </remarks>
        public long LL_TotalLook;

        /// <summary>
        /// Gets the minimum lookahead required for any single LL prediction to
        /// complete for this decision.
        /// </summary>
        /// <remarks>
        /// Gets the minimum lookahead required for any single LL prediction to
        /// complete for this decision. An LL prediction completes when the algorithm
        /// reaches a unique prediction, a conflict state (for
        /// <see cref="PredictionMode.Ll"/>
        /// , an ambiguity state (for
        /// <see cref="PredictionMode.LlExactAmbigDetection"/>
        /// , or a syntax error.
        /// </remarks>
        public long LL_MinLook;

        /// <summary>
        /// Gets the maximum lookahead required for any single LL prediction to
        /// complete for this decision.
        /// </summary>
        /// <remarks>
        /// Gets the maximum lookahead required for any single LL prediction to
        /// complete for this decision. An LL prediction completes when the algorithm
        /// reaches a unique prediction, a conflict state (for
        /// <see cref="PredictionMode.Ll"/>
        /// , an ambiguity state (for
        /// <see cref="PredictionMode.LlExactAmbigDetection"/>
        /// , or a syntax error.
        /// </remarks>
        public long LL_MaxLook;

        /// <summary>
        /// Gets the
        /// <see cref="LookaheadEventInfo"/>
        /// associated with the event where the
        /// <see cref="LL_MaxLook"/>
        /// value was set.
        /// </summary>
        public LookaheadEventInfo LL_MaxLookEvent;

        /// <summary>
        /// A collection of
        /// <see cref="ContextSensitivityInfo"/>
        /// instances describing the
        /// context sensitivities encountered during LL prediction for this decision.
        /// </summary>
        /// <seealso cref="ContextSensitivityInfo"/>
        public readonly IList<ContextSensitivityInfo> contextSensitivities = new List<ContextSensitivityInfo>();

        /// <summary>
        /// A collection of
        /// <see cref="ErrorInfo"/>
        /// instances describing the parse errors
        /// identified during calls to
        /// <see cref="ParserATNSimulator.AdaptivePredict(ITokenStream, int, ParserRuleContext)"/>
        /// for
        /// this decision.
        /// </summary>
        /// <seealso cref="ErrorInfo"/>
        public readonly IList<ErrorInfo> errors = new List<ErrorInfo>();

        /// <summary>
        /// A collection of
        /// <see cref="AmbiguityInfo"/>
        /// instances describing the
        /// ambiguities encountered during LL prediction for this decision.
        /// </summary>
        /// <seealso cref="AmbiguityInfo"/>
        public readonly IList<AmbiguityInfo> ambiguities = new List<AmbiguityInfo>();

        /// <summary>
        /// A collection of
        /// <see cref="PredicateEvalInfo"/>
        /// instances describing the
        /// results of evaluating individual predicates during prediction for this
        /// decision.
        /// </summary>
        /// <seealso cref="PredicateEvalInfo"/>
        public readonly IList<PredicateEvalInfo> predicateEvals = new List<PredicateEvalInfo>();

        /// <summary>
        /// The total number of ATN transitions required during SLL prediction for
        /// this decision.
        /// </summary>
        /// <remarks>
        /// The total number of ATN transitions required during SLL prediction for
        /// this decision. An ATN transition is determined by the number of times the
        /// DFA does not contain an edge that is required for prediction, resulting
        /// in on-the-fly computation of that edge.
        /// <p>
        /// If DFA caching of SLL transitions is employed by the implementation, ATN
        /// computation may cache the computed edge for efficient lookup during
        /// future parsing of this decision. Otherwise, the SLL parsing algorithm
        /// will use ATN transitions exclusively.</p>
        /// </remarks>
        /// <seealso cref="SLL_ATNTransitions"/>
        /// <seealso cref="ParserATNSimulator.ComputeTargetState"/>
        /// <seealso cref="LexerATNSimulator.ComputeTargetState"/>
        public long SLL_ATNTransitions;

        /// <summary>
        /// The total number of DFA transitions required during SLL prediction for
        /// this decision.
        /// </summary>
        /// <remarks>
        /// The total number of DFA transitions required during SLL prediction for
        /// this decision.
        /// <p>If the ATN simulator implementation does not use DFA caching for SLL
        /// transitions, this value will be 0.</p>
        /// </remarks>
        /// <seealso cref="ParserATNSimulator.GetExistingTargetState"/>
        /// <seealso cref="LexerATNSimulator.GetExistingTargetState"/>
        public long SLL_DFATransitions;

        /// <summary>
        /// Gets the total number of times SLL prediction completed in a conflict
        /// state, resulting in fallback to LL prediction.
        /// </summary>
        /// <remarks>
        /// Gets the total number of times SLL prediction completed in a conflict
        /// state, resulting in fallback to LL prediction.
        /// <p>Note that this value is not related to whether or not
        /// <see cref="PredictionMode.Sll"/>
        /// may be used successfully with a particular
        /// grammar. If the ambiguity resolution algorithm applied to the SLL
        /// conflicts for this decision produce the same result as LL prediction for
        /// this decision,
        /// <see cref="PredictionMode.Sll"/>
        /// would produce the same overall
        /// parsing result as
        /// <see cref="PredictionMode.Ll"/>
        /// .</p>
        /// </remarks>
        public long LL_Fallback;

        /// <summary>
        /// The total number of ATN transitions required during LL prediction for
        /// this decision.
        /// </summary>
        /// <remarks>
        /// The total number of ATN transitions required during LL prediction for
        /// this decision. An ATN transition is determined by the number of times the
        /// DFA does not contain an edge that is required for prediction, resulting
        /// in on-the-fly computation of that edge.
        /// <p>
        /// If DFA caching of LL transitions is employed by the implementation, ATN
        /// computation may cache the computed edge for efficient lookup during
        /// future parsing of this decision. Otherwise, the LL parsing algorithm will
        /// use ATN transitions exclusively.</p>
        /// </remarks>
        /// <seealso cref="LL_DFATransitions"/>
        /// <seealso cref="ParserATNSimulator.ComputeTargetState"/>
        /// <seealso cref="LexerATNSimulator.ComputeTargetState"/>
        public long LL_ATNTransitions;

        /// <summary>
        /// The total number of DFA transitions required during LL prediction for
        /// this decision.
        /// </summary>
        /// <remarks>
        /// The total number of DFA transitions required during LL prediction for
        /// this decision.
        /// <p>If the ATN simulator implementation does not use DFA caching for LL
        /// transitions, this value will be 0.</p>
        /// </remarks>
        /// <seealso cref="ParserATNSimulator.GetExistingTargetState"/>
        /// <seealso cref="LexerATNSimulator.GetExistingTargetState"/>
        public long LL_DFATransitions;

        /// <summary>
        /// Constructs a new instance of the
        /// <see cref="DecisionInfo"/>
        /// class to contain
        /// statistics for a particular decision.
        /// </summary>
        /// <param name="decision">The decision number</param>
        public DecisionInfo(int decision)
        {
            this.decision = decision;
        }

        public override string ToString()
        {
            return "{" + "decision=" + decision + ", contextSensitivities=" + contextSensitivities.Count + ", errors=" + errors.Count + ", ambiguities=" + ambiguities.Count + ", SLL_lookahead=" + SLL_TotalLook + ", SLL_ATNTransitions=" + SLL_ATNTransitions + ", SLL_DFATransitions=" + SLL_DFATransitions + ", LL_Fallback=" + LL_Fallback + ", LL_lookahead=" + LL_TotalLook + ", LL_ATNTransitions=" + LL_ATNTransitions + '}';
        }
    }
}
