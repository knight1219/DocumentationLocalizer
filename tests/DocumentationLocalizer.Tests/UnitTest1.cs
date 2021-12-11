using NUnit.Framework;

namespace DocumentationLocalizer.Tests
{
    public class Tests
    {
        private string filename = "Test.xml";
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DetectionOfLanguagesInDifferentDocumentations()
        {
            var localizer = new Localizer();
            var files = localizer.LocalizeXMLFile(filename);
            Assert.NotZero(files);
            Assert.Positive(files);
            Assert.AreEqual(files, 3);
        }

        [Test]
        public void DidAllFileGetPhysicallyCreated()
        {
            var localizer = new Localizer();
            var files = localizer.LocalizeXMLFile(filename);

            FileAssert.Exists("Test-en-US.xml");
            FileAssert.Exists("Test-fr.xml");
            FileAssert.Exists("Test-zh-CN.xml");

        }

        //[Test]
        //public void DidAllFileGetPhysicallyCreated()
        //{
        //    var localizer = new Localizer();
        //    var files = localizer.LocalizeXMLFile(filename);

        //    FileAssert.Exists("Test-en-US.xml");
        //    FileAssert.Exists("Test-fr.xml");
        //    FileAssert.Exists("Test-zh-CN.xml");

        //}
    }
}