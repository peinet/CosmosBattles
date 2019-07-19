#if NET35PLUS

#if !COMPACT
using System;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Func<>))]
[assembly: TypeForwardedTo(typeof(Func<,>))]
[assembly: TypeForwardedTo(typeof(Func<,,>))]
#endif

#else

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// <OWNER>[....]</OWNER>
// 

namespace System {

    public delegate TResult AntlrFunc<out TResult>();

    public delegate TResult AntlrFunc<in T, out TResult>(T arg);

    public delegate TResult AntlrFunc<in T1, in T2, out TResult>(T1 arg1, T2 arg2);

}

#endif
