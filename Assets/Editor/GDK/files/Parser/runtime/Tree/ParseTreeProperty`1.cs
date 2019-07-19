// Copyright (c) Terence Parr, Sam Harwell. All Rights Reserved.
// Licensed under the BSD License. See LICENSE.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Antlr4.Runtime.Sharpen;

namespace Antlr4.Runtime.Tree
{
    /// <summary>Associate a property with a parse tree node.</summary>
    /// <remarks>
    /// Associate a property with a parse tree node. Useful with parse tree listeners
    /// that need to associate values with particular tree nodes, kind of like
    /// specifying a return value for the listener event method that visited a
    /// particular node. Example:
    /// <pre>
    /// ParseTreeProperty&lt;Integer&gt; values = new ParseTreeProperty&lt;Integer&gt;();
    /// values.put(tree, 36);
    /// int x = values.get(tree);
    /// values.removeFrom(tree);
    /// </pre>
    /// You would make one decl (values here) in the listener and use lots of times
    /// in your event methods.
    /// </remarks>
    public class ParseTreeProperty<V>
    {
        protected internal ConcurrentDictionary<IParseTree, V> annotations = new ConcurrentDictionary<IParseTree, V>();

        public virtual V Get(IParseTree node)
        {
            V value;
            if (!annotations.TryGetValue(node, out value))
                return default(V);

            return value;
        }

        public virtual void Put(IParseTree node, V value)
        {
            annotations[node] = value;
        }

        public virtual V RemoveFrom(IParseTree node)
        {
            V value;
            if (!annotations.TryRemove(node, out value))
                return default(V);

            return value;
        }
    }
}
