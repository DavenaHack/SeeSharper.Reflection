using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mimp.SeeSharper.Reflection.Test
{
    public partial class TestTypeExtensions
    {


        [TestMethod]
        public void TestGetInstanceProperty()
        {

            Assert.IsNotNull(typeof(Type).GetInstanceProperty(nameof(Type.FullName)));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(Type).GetInstanceProperty(nameof(Type.DefaultBinder));
            });

        }


        [TestMethod]
        public void TestGetStaticProperty()
        {

            Assert.IsNotNull(typeof(Type).GetStaticProperty(nameof(Type.DefaultBinder)));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(Type).GetStaticProperty(nameof(Type.FullName));
            });

        }


    }
}
