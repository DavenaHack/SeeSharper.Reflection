using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestGetInstanceField()
        {

            Assert.IsNotNull(typeof(string).GetField("_stringLength", false, false, true));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(string).GetInstanceField(nameof(string.Empty));
            });

        }


        [TestMethod]
        public void TestGetStaticField()
        {

            Assert.IsNotNull(typeof(string).GetStaticField(nameof(string.Empty)));
            Assert.IsNotNull(typeof(int).GetStaticField(nameof(int.MinValue)));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(string).GetField("_stringLength", false, true, true);
            });

        }


    }
}
