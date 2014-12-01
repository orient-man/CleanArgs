using NUnit.Framework;

namespace CSharpArgs2
{
    [TestFixture]
    public class ArgsTests
    {
        [Test]
        public void CreateWithNoSchemaOrArguments()
        {
            var args = new Args("", new string[0]);
            Assert.AreEqual(0, args.Cardinality());
        }

        [Test]
        public void NoSchemaButWithOneArgument()
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
        public void NoSchemaButWithMultipleArguments()
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
        public void NonLetterSchema()
        {
            try
            {
                new Args("*", new string[] { });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidArgumentName, e.ErrorCode);
                Assert.AreEqual('*', e.ErrorArgumentId);
            }
        }

        [Test]
        public void InvalidArgumentFormat()
        {
            try
            {
                new Args("f~", new string[] { });
                Assert.Fail();
            }
            catch (ArgsException e)
            {
                Assert.AreEqual(ErrorCode.InvalidArgumentFormat, e.ErrorCode);
                Assert.AreEqual('f', e.ErrorArgumentId);
            }
        }

        [Test]
        public void SimpleBooleanPresent()
        {
            var args = new Args("x", new[] { "-x" });
            Assert.AreEqual(true, args.GetBoolean('x'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void WhenArgumentNotPresentGetBooleanReturnsFalse()
        {
            var args = new Args("x#", new[] { "-x", "42" });
            Assert.AreEqual(false, args.GetBoolean('y'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void WhenArgumentNotPresentGetIntReturnsZero()
        {
            var args = new Args("x", new[] { "-x" });
            Assert.AreEqual(0, args.GetInt('y'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void WhenArgumentNotPresentGetStringReturnEmptyString()
        {
            var args = new Args("x", new[] { "-x" });
            Assert.AreEqual("", args.GetString('y'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void SimpleStringPresent()
        {
            var args = new Args("x*", new[] { "-x", "param" });
            Assert.True(args.Has('x'));
            Assert.AreEqual("param", args.GetString('x'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void MissingStringArgument()
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
        public void SpacesInFormat()
        {
            var args = new Args("x, y", new[] { "-xy" });
            Assert.True(args.Has('x'));
            Assert.True(args.Has('y'));
            Assert.AreEqual(2, args.Cardinality());
        }

        [Test]
        public void SimpleIntPresent()
        {
            var args = new Args("x#", new[] { "-x", "42" });
            Assert.True(args.Has('x'));
            Assert.AreEqual(42, args.GetInt('x'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void InvalidInteger()
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
            }
        }

        [Test]
        public void MissingInteger()
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
        public void SimpleDoublePresent()
        {
            var args = new Args("x##", new[] { "-x", "42.13" });
            Assert.True(args.Has('x'));
            Assert.AreEqual(42.13, args.GetDouble('x'));
            Assert.AreEqual(1, args.Cardinality());
        }

        [Test]
        public void InvalidDouble()
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
            }
        }

        [Test]
        public void MissingDouble()
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
        public void ExtraArguments()
        {
            var args = new Args("x,y*", new[] { "-x", "-y", "alpha", "beta" });
            Assert.True(args.GetBoolean('x'));
            Assert.AreEqual("alpha", args.GetString('y'));
            Assert.AreEqual(2, args.Cardinality());
        }
    }
}