﻿using System.Collections.Generic;

namespace CSharpArgs
{
    public static class EnumeratorExtensions
    {
        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}