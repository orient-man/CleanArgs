using System;
using System.Runtime.InteropServices;
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
            Func<Task<int>> compute5 = () => 5.ToTask();
            Func<Task<int>> compute7 = () => 7.ToTask();
            Func<int, int, Task<int>> add = (x, y) => (x + y).ToTask();

            var r =
                from a in compute5()
                from b in compute7()
                from c in add(a, b)
                select c * 2;

            r.Result.Should().Be(24);
        }

        public async void CompositionWithAsyncAwait()
        {
            var a = Compute5();
            var b = Compute7();
            var r = await Add(await a, await b) * 2;
            r.Should().Be(12);
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
            return a.ContinueWith(b => func(b.Result)).Unwrap();
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