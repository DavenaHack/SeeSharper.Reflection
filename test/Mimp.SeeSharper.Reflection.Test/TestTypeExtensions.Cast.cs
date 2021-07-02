using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestGetCastFunc()
        {

            var cast = typeof(object).GetCastFunc<int>();
            var i = cast(10);

            Assert.ThrowsException<InvalidCastException>(() =>
            {
                cast("");
            });

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(int).GetCastFunc<string>();
            });

        }


    }
}
