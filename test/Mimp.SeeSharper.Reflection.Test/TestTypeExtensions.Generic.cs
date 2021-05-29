using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimp.SeeSharper.Reflection.Test.Mock;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestGetGenericArguments()
        {

            var types = typeof(IDictionary<string, object>).GetGenericArgumentsRequired(typeof(IDictionary<,>)).ToArray();

            Assert.IsTrue(types[0] == typeof(string));
            Assert.IsTrue(types[1] == typeof(object));

        }


        [TestMethod]
        public void TestResolveInheritGenericTypes()
        {

            Assert.IsTrue(typeof(IUnaryDictionary<>).ResolveInheritGenericType(typeof(IDictionary<string, string>)) == typeof(IUnaryDictionary<string>));
            Assert.ThrowsException<InvalidOperationException>(() => typeof(IUnaryDictionary<>).ResolveInheritGenericType(typeof(IDictionary<string, object>)));

            Assert.IsTrue(typeof(IReverseDictionary<,>).ResolveInheritGenericType(typeof(IDictionary<string, object>)) == typeof(IReverseDictionary<object, string>));

            var type = typeof(ITripleDictionary<,,>).ResolveInheritGenericType(typeof(IDictionary<string, object>));
            Assert.IsTrue(type.ContainsGenericParameters);
            var args = type.GetGenericArguments();
            Assert.IsTrue(args[0] == typeof(string));
            Assert.IsTrue(args[1] == typeof(object));

            type = type.ResolveInheritGenericType(typeof(IComparer<int>));
            Assert.IsFalse(type.ContainsGenericParameters);
            args = type.GetGenericArguments();
            Assert.IsTrue(args[2] == typeof(int));
            Assert.IsTrue(type == typeof(ITripleDictionary<string, object, int>));

        }


    }
}
