using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection.Test
{
    [TestClass]
    public partial class TestAssemblyExtensions
    {


        [TestMethod]
        public void GetAssemblies()
        {

            var assembly = typeof(AssemblyExtensions).Assembly;
            var assemblies = assembly.GetAssemblies().ToArray();

            Assert.IsFalse(assemblies.Contains(assembly));
            Assert.IsTrue(assembly.GetReferencedAssemblies()
                .SelectMany(name => new[] { name }.Concat(Assembly.Load(name).GetReferencedAssemblies()))
                .All(name => assemblies.Any(assembly => AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), name))));

        }


    }
}
