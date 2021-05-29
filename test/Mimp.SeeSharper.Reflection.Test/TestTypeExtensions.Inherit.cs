using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimp.SeeSharper.Reflection.Test.Mock;
using System.Collections.Generic;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestInherit()
        {

            Assert.IsTrue(typeof(IEnumerable<int>).Inherit(typeof(IEnumerable<>)));
            Assert.IsFalse(typeof(IEnumerable<>).Inherit(typeof(IEnumerable<int>)));
            Assert.IsTrue(typeof(IDictionary<string, object>).Inherit(typeof(IEnumerable<KeyValuePair<string, object>>)));
            Assert.IsFalse(typeof(IEnumerable<string>).Inherit(typeof(IEnumerable<int>)));

            var type = typeof(ITripleDictionary<,,>).MakeGenericType(typeof(string), typeof(object), typeof(ITripleDictionary<,,>).GetGenericArguments()[2]);
            Assert.IsTrue(type.Inherit(typeof(ITripleDictionary<,,>)));
            Assert.IsTrue(typeof(ITripleDictionary<string, object, int>).Inherit(type));

        }



        [TestMethod]
        public void TestInheritGenericParameter()
        {

            var type = typeof(ITripleDictionary<,,>).MakeGenericType(typeof(string), typeof(object), typeof(ITripleDictionary<,,>).GetGenericArguments()[2]);
            Assert.IsTrue(typeof(int).InheritGenericParameter(type.GetGenericArguments()[2], type));

        }


    }
}
