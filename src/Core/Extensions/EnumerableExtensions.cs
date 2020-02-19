// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Draco.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool None<T>(this IEnumerable<T> source) => !source.Any();
        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => !source.Any(predicate);

        public static string ToCommaSeparatedString<T>(this IEnumerable<T> source) => source.ToSeparatedString(", ");
        public static string ToSpaceSeparatedString<T>(this IEnumerable<T> source) => source.ToSeparatedString(" ");
        public static string ToSeparatedString<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source.ToArray());
    }
}
