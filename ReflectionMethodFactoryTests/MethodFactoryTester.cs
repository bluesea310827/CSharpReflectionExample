using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReflectionMethodFactory;

namespace ReflectionMethodFatoryTests
{
    [TestClass]
    public class MethodFactoryTester
    {
        [TestMethod]
        public void AddtionProcessTest()
        {
            var methodInvoker = new MethodFactory();

            var firstNumber = 20;
            var secondNumber = 30;
            var expressionMethod = "AddtionProcess";

            // Get Method
            var method = methodInvoker.GetInvokeMethod(expressionMethod, null);

            // Call function
            var returnResult = (int)method.DynamicInvoke(new object[] { firstNumber, secondNumber });

            Assert.AreEqual(firstNumber+secondNumber, returnResult);
        }

        [TestMethod]
        public void MinusProcessTest()
        {
            var methodInvoker = new MethodFactory();

            var firstNumber = 300;
            var secondNumber = 30;
            var expressionMethod = "MinusProcess";

            // Get Method
            var method = methodInvoker.GetInvokeMethod(expressionMethod, null);

            // Call function
            var returnResult = (int)method.DynamicInvoke(new object[] { firstNumber, secondNumber });

            Assert.AreEqual(firstNumber-secondNumber, returnResult);
        }

        [TestMethod]
        public void MultiplicationProcessTest()
        {
            var methodInvoker = new MethodFactory();

            var firstNumber = 20;
            var secondNumber = 30;
            var expressionMethod = "MultiplicationProcess";

            // Get Method
            var method = methodInvoker.GetInvokeMethod(expressionMethod, null);

            // Call function
            var returnResult = (int)method.DynamicInvoke(new object[] { firstNumber, secondNumber });

            Assert.AreEqual(firstNumber*secondNumber, returnResult);
        }

        [TestMethod]
        public void DivisionProcessTest()
        {
            var methodInvoker = new MethodFactory();

            var firstNumber = 20;
            var secondNumber = 30;
            var expressionMethod = "DivisionProcess";

            // Get Method
            var method = methodInvoker.GetInvokeMethod(expressionMethod, null);

            // Call function
            var returnResult = (int)method.DynamicInvoke(new object[] { firstNumber, secondNumber });

            Assert.AreEqual(firstNumber/secondNumber, returnResult);
        }
    }
}
