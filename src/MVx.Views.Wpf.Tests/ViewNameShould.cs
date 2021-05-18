using NUnit.Framework;
using System.Collections.Generic;

namespace MVx.Views.Wpf.Tests
{
    public class Tests
    {
        public static IEnumerable<TestCaseData> ReturnTheExpectedNameTests
        {
            get
            {
                yield return new TestCaseData("ViewModel", "View");
                yield return new TestCaseData("MyProject.ViewModel", "MyProject.View");
                yield return new TestCaseData("MyProject.Embedded.ViewModel", "MyProject.Embedded.View");
            }
        }

        [TestCaseSource(nameof(ReturnTheExpectedNameTests))]
        public void ReturnTheExpectedName(string modelName, string expected)
        {
            Assert.That(ViewName.For(modelName), Is.EqualTo(expected));
        }
    }
}