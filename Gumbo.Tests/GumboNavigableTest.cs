using NUnit.Framework;

namespace Gumbo
{
    public class GumboNavigableTest
    {
        public static readonly string TestHtml = "<html><body class=\"gumbo\">boo!<span>Pillz here!</span><p id=\"tag123\"></p></body></html>";

        [Test]
        public void TestSelectSingleNodeAttribute()
        {
            using (var gumbo = new Gumbo(TestHtml))
            {
                var nav = gumbo.CreateNavigator();
                var node = nav.SelectSingleNode("/html/body/@class");
                Assert.NotNull(node);
                Assert.AreEqual("gumbo", node.Value);
                Assert.AreEqual("class", node.Name);
                Assert.AreEqual("class", node.LocalName);
            }
        }

        [Test]
        public void TestSelectSingleNodeForElement()
        {
            using (var gumbo = new Gumbo(TestHtml))
            {
                var nav = gumbo.CreateNavigator();
                var node = nav.SelectSingleNode("/html/body/span");
                Assert.NotNull(node);
                Assert.AreEqual("Pillz here!", node.Value);
                Assert.AreEqual("span", node.Name);
                Assert.AreEqual("span", node.LocalName);
            }
        }

        [Test]
        public void TestMoveToId()
        {
            using (var gumbo = new Gumbo(TestHtml))
            {
                var nav = gumbo.CreateNavigator();
                nav.MoveToId("tag123");
                Assert.AreEqual("p", nav.Name);
            }
        }
    }
}
