// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Draco.Core.Extensions
{
    /// <summary>
    /// A bunch of convenience extension methods that are used across the platform.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// The inverse of C#'s native LINQ "Any" operator.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool None<T>(this IEnumerable<T> source) => !source.Any();

        /// <summary>
        /// The inverse of C#'s native LINQ "Any" operator + a predicate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => !source.Any(predicate);

        /// <summary>
        /// Converts a collection of objects to a comma-delimited string.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> source) => source.ToSeparatedString(", ");

        /// <summary>
        /// Converts a collection of objects to a space-delimited string.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToSpaceSeparatedString<T>(this IEnumerable<T> source) => source.ToSeparatedString(" ");

        /// <summary>
        /// Converts a collection of objects to a [separator]-delimited string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToSeparatedString<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source.ToArray());
    }
}
