using System;
using CSharpArgs;
using NUnit.Framework;

namespace CSharpArgsTests
{
    [TestFixture]
    public class ArgsTests
    {
        [Test]
        public void TestCreateWithNoSchemaOrArguments()
        {
            var args = new Args("", new String[0]);
            Assert.AreEqual(0, args.NextArgument());
        }

        [Test]
        public void TestWithNoSchemaButWithOneArgument()
        {
            try
            {
                new Args("", new[] { "-x" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.UnexpectedArgument, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestWithNoSchemaButWithMultipleArguments()
        {
            try
            {
                new Args("", new[] { "-x", "-y" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.UnexpectedArgument, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestNonLetterSchema()
        {
            try
            {
                new Args("*", new String[] { });
                Assert.Fail("Args constructor should have thrown exception");
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidArgumentName, e.ErrorCode);
                Assert.AreEqual('*', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestInvalidArgumentFormat()
        {
            try
            {
                new Args("f~", new String[] { });
                Assert.Fail("Args constructor should have throws exception");
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidArgumentFormat, e.ErrorCode);
                Assert.AreEqual('f', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestSimpleBooleanPresent()
        {
            var args = new Args("x", new[] { "-x" });
            Assert.AreEqual(true, args.GetBoolean('x'));
            Assert.AreEqual(1, args.NextArgument());
        }

        [Test]
        public void TestSimpleStringPresent()
        {
            var args = new Args("x*", new[] { "-x", "param" });
            Assert.True(args.Has('x'));
            Assert.AreEqual("param", args.GetString('x'));
            Assert.AreEqual(2, args.NextArgument());
        }

        [Test]
        public void TestMissingStringArgument()
        {
            try
            {
                new Args("x*", new[] { "-x" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.MissingString, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestSpacesInFormat()
        {
            var args = new Args("x, y", new[] { "-xy" });
            Assert.True(args.Has('x'));
            Assert.True(args.Has('y'));
            Assert.AreEqual(1, args.NextArgument());
        }

        [Test]
        public void TestSimpleIntPresent()
        {
            var args = new Args("x#", new[] { "-x", "42" });
            Assert.True(args.Has('x'));
            Assert.AreEqual(42, args.GetInt('x'));
            Assert.AreEqual(2, args.NextArgument());
        }

        [Test]
        public void TestInvalidInteger()
        {
            try
            {
                new Args("x#", new[] { "-x", "Forty two" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidInteger, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
                Assert.AreEqual("Forty two", e.ErrorParameter);
            }
        }

        [Test]
        public void TestMissingInteger()
        {
            try
            {
                new Args("x#", new[] { "-x" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.MissingInteger, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestSimpleDoublePresent()
        {
            var args = new Args("x##", new[] { "-x", "42.3" });
            Assert.True(args.Has('x'));
            Assert.AreEqual(42.3, args.GetDouble('x'), .001);
        }

        [Test]
        public void TestInvalidDouble()
        {
            try
            {
                new Args("x##", new[] { "-x", "Forty two" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidDouble, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
                Assert.AreEqual("Forty two", e.ErrorParameter);
            }
        }

        [Test]
        public void TestMissingDouble()
        {
            try
            {
                new Args("x##", new[] { "-x" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.MissingDouble, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestStringArray()
        {
            var args = new Args("x[*]", new[] { "-x", "alpha" });
            Assert.True(args.Has('x'));
            var result = args.GetStringArray('x');
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("alpha", result[0]);
        }

        [Test]
        public void TestMissingStringArrayElement()
        {
            try
            {
                new Args("x[*]", new[] { "-x" });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.MissingString, e.ErrorCode);
                Assert.AreEqual('x', e.ErrorArgumentId);
            }
        }

        [Test]
        public void TestExtraArguments()
        {
            var args = new Args("x,y*", new[] { "-x", "-y", "alpha", "beta" });
            Assert.True(args.GetBoolean('x'));
            Assert.AreEqual("alpha", args.GetString('y'));
            Assert.AreEqual(3, args.NextArgument());
        }

        [Test]
        public void TestExtraArgumentsThatLookLikeFlags()
        {
            var args = new Args("x,y", new[] { "-x", "alpha", "-y", "beta" });
            Assert.True(args.Has('x'));
            Assert.False(args.Has('y'));
            Assert.True(args.GetBoolean('x'));
            Assert.False(args.GetBoolean('y'));
            Assert.AreEqual(1, args.NextArgument());
        }
    }
}