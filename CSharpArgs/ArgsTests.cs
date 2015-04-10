using NUnit.Framework;

namespace CSharpArgs
{
    [TestFixture]
    public class ArgsTests
    {
        [Test]
        public void TestCreateWithNoSchemaOrArguments()
        {
            var args = new Args("", new string[0]);
            Assert.AreEqual(0, args.Cardinality());
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
                new Args("*", new string[] { });
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
                new Args("f~", new string[] { });
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
            Assert.AreEqual(true, args.Get<bool>('x'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void TestSimpleStringPresent()
        {
            var args = new Args("x*", new[] { "-x", "param" });
            Assert.True(args.Has('x'));
            Assert.AreEqual("param", args.Get<string>('x'));
            Assert.AreEqual(1, args.Cardinality());
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
            Assert.AreEqual(2, args.Cardinality());
        }

        [Test]
        public void TestSimpleIntPresent()
        {
            var args = new Args("x#", new[] { "-x", "42" });
            Assert.True(args.Has('x'));
            Assert.AreEqual(42, args.Get<int>('x'));
            Assert.AreEqual(1, args.Cardinality());
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
            Assert.AreEqual(42.3, args.Get<double>('x'), .001);
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
        public void TestExtraArgumentsOnTheEnd()
        {
            var args = new Args("x,y*", new[] { "-x", "-y", "alpha", "beta" });
            Assert.True(args.Get<bool>('x'));
            Assert.AreEqual("alpha", args.Get<string>('y'));
            Assert.AreEqual(2, args.Cardinality());
        }

        [Test]
        public void TestExtraArgumentsInTheMiddle()
        {
            var args = new Args("x,y", new[] { "-x", "alpha", "-y", "beta" });
            Assert.True(args.Has('x'));
            Assert.True(args.Has('y'));
            Assert.True(args.Get<bool>('x'));
            Assert.True(args.Get<bool>('y'));
            Assert.AreEqual(2, args.Cardinality());
        }
    }
}