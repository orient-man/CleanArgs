using CSharpArgs;
using NUnit.Framework;

namespace CSharpArgsTests
{
    [TestFixture]
    public class ArgsExceptionTests
    {
        [Test]
        public void TestUnexpectedMessage()
        {
            var e = new ArgsException(ErrorCode.UnexpectedArgument, 'x', null);
            Assert.AreEqual("Argument -x unexpected.", e.GetErrorMessage());
        }

        [Test]
        public void TestMissingStringMessage()
        {
            var e = new ArgsException(ErrorCode.MissingString, 'x', null);
            Assert.AreEqual("Could not find string parameter for -x.", e.GetErrorMessage());
        }

        [Test]
        public void TestInvalidIntegerMessage()
        {
            var e = new ArgsException(ErrorCode.InvalidInteger, 'x', "Forty two");
            Assert.AreEqual("Argument -x expects an integer but was 'Forty two'.", e.GetErrorMessage());
        }

        [Test]
        public void TestMissingIntegerMessage()
        {
            var e = new ArgsException(ErrorCode.MissingInteger, 'x', null);
            Assert.AreEqual("Could not find integer parameter for -x.", e.GetErrorMessage());
        }

        [Test]
        public void TestInvalidDoubleMessage()
        {
            var e = new ArgsException(ErrorCode.InvalidDouble, 'x', "Forty two");
            Assert.AreEqual("Argument -x expects a double but was 'Forty two'.", e.GetErrorMessage());
        }

        [Test]
        public void TestMissingDoubleMessage()
        {
            var e = new ArgsException(ErrorCode.MissingDouble, 'x', null);
            Assert.AreEqual("Could not find double parameter for -x.", e.GetErrorMessage());
        }

        [Test]
        public void TestInvalidArgumentName()
        {
            var e = new ArgsException(ErrorCode.InvalidArgumentName, '#', null);
            Assert.AreEqual("'#' is not a valid argument name.", e.GetErrorMessage());
        }

        [Test]
        public void TestInvalidFormat()
        {
            var e = new ArgsException(ErrorCode.InvalidArgumentFormat, 'x', "$");
            Assert.AreEqual("'$' is not a valid argument format.", e.GetErrorMessage());
        }
    }
}