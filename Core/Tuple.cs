using System;
using System.Collections.Generic;

namespace ModU3DToolkit.Core
{
	public struct Tuple<A, B>
	{
        public A First;
        public B Second;

        public Tuple(A first, B second)
        {
            First = first;
            Second = second;
        }
    }
}
