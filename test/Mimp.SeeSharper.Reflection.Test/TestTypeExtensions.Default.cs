using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestGetDefaultDelegate()
        {

            var dic = typeof(Dictionary<string, string>).GetDefaultFunc<IDictionary<string, string>>()();
            Assert.AreEqual(dic, default(Dictionary<string, string>));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(int).GetDefaultFunc<string>();
            });

        }


        [TestMethod]
        public void TestDefault()
        {

            var pair = typeof(KeyValuePair<string, string>).Default();
            Assert.AreEqual(pair, default(KeyValuePair<string, string>));

        }


    }
}
