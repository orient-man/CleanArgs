using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CSharpArgs2
{
    public static class Playground
    {
        private static int SumList(IEnumerable<int> list)
        {
            return list.Any()
                ? list.First() + SumList(list.Skip(1))
                : 0;
        }

        [Test]
        public static void SumListTest()
        {
            Assert.That(SumList(new List<int> { 1, 3, 4 }), Is.EqualTo(8));
        }

        private static T FoldR<T>(T seed, Func<T, T, T> func, IEnumerable<T> source)
        {
            return source.Any()
                ? func(source.First(), FoldR(seed, func, source.Skip(1)))
                : seed;
        }

        [Test]
        public static void FoldTest()
        {
            Func<IEnumerable<int>, int> sum = list => FoldR(0, (a, b) => a + b, list);
            Assert.That(sum(new List<int> { 1, 3, 4 }), Is.EqualTo(8));
        }

        private static TAccumulate Aggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            var result = seed;
            foreach (TSource element in source) result = func(result, element);
            return result;
        }

        [Test]
        public static void AggregateTest()
        {
            Assert.That(
                new List<int> { 1, 3, 4 }.Aggregate(0, (a, b) => a + b),
                Is.EqualTo(8));
        }
    }
}