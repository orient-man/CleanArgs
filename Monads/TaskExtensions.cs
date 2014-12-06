using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Monads
{
    [TestFixture]
    public class TaskMonad
    {
        [Test]
        public void CompositionWithLinqMonad()
        {
            var r =
                from a in Compute5()
                from b in Compute7()
                from c in Add(a, b)
                select c * 2;

            r.Result.Should().Be(24);
        }

        public async void CompositionWithAsyncAwait()
        {
            var a = Compute5();
            var b = Compute7();
            var r = await Add(await a, await b) * 2;
            r.Should().Be(24);
        }

        private static async Task<int> Compute5()
        {
            return await 5.ToTask();
        }

        private static async Task<int> Compute7()
        {
            return await 7.ToTask();
        }

        private static async Task<int> Add(int x, int y)
        {
            return await (x + y).ToTask();
        }
    }

    public static class TaskExtensions
    {
        public static Task<T> ToTask<T>(this T value) // aka "unit"
        {
            return Task<T>.Factory.StartNew(() => value);
        }

        public static Task<B> Bind<A, B>(
            this Task<A> a,
            Func<A, Task<B>> func)
        {
            return a.ContinueWith(prev => func(prev.Result)).Unwrap();
        }

        public static Task<C> SelectMany<A, B, C>(
            this Task<A> a,
            Func<A, Task<B>> func,
            Func<A, B, C> select)
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToTask()));
        }
    }
}