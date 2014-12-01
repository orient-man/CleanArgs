using NUnit.Framework;

namespace CSharpArgs2
{
    [TestFixture]
    public class ArgsExceptionTests
    {
        [Test]
        public void UnexpectedMessage()
        {
            var e = new ArgsException(ErrorCode.UnexpectedArgument, 'x');
            Assert.AreEqual("Argument -x unexpected.", e.GetErrorMessage());
        }

        [Test]
        public void MissingStringMessage()
        {
            var e = new ArgsException(ErrorCode.MissingString, 'x');
            Assert.AreEqual("Could not find string parameter for -x.", e.GetErrorMessage());
        }

        [Test]
        public void InvalidIntegerMessage()
        {
            var e = new ArgsException(ErrorCode.InvalidInteger, 'x', "Forty two");
            Assert.AreEqual(
                "Argument -x expects an integer but was 'Forty two'.",
                e.GetErrorMessage());
        }

        [Test]
        public void MissingIntegerMessage()
        {
            var e = new ArgsException(ErrorCode.MissingInteger, 'x');
            Assert.AreEqual(
                "Could not find integer parameter for -x.",
                e.GetErrorMessage());
        }

        [Test]
        public void InvalidDobuleMessage()
        {
            var e = new ArgsException(ErrorCode.InvalidDouble, 'x', "Forty two");
            Assert.AreEqual(
                "Argument -x expects a double but was 'Forty two'.",
                e.GetErrorMessage());
        }

        [Test]
        public void MissingDoubleMessage()
        {
            var e = new ArgsException(ErrorCode.MissingDouble, 'x');
            Assert.AreEqual(
                "Could not find double parameter for -x.",
                e.GetErrorMessage());
        }

        [Test]
        public void InvalidArgumentName()
        {
            var e = new ArgsException(ErrorCode.InvalidArgumentName, '#');
            Assert.AreEqual("'#' is not a valid argument name.", e.GetErrorMessage());
        }

        [Test]
        public void InvalidFormat()
        {
            var e = new ArgsException(ErrorCode.InvalidArgumentFormat, 'x', "$");
            Assert.AreEqual("'$' is not a valid argument format.", e.GetErrorMessage());
        }
    }
}