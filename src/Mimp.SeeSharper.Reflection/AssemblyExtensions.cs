using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class AssemblyExtensions
    {


        /// <summary>
        /// Return all reference assemblies
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Assembly> GetAssemblies(this Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            var assemblies = new List<AssemblyName> { assembly.GetName() };

            var next = new List<Assembly> { assembly };
            while (next.Count > 0)
            {
                var current = next;
                next = new List<Assembly>();
                foreach (var currentAssembly in current)
                    foreach (var name in currentAssembly.GetReferencedAssemblies())
                        if (!assemblies.Any(def => AssemblyName.ReferenceMatchesDefinition(name, def)))
                        {
                            assemblies.Add(name);
                            var assm = Assembly.Load(name);
                            yield return assm;
                            next.Add(assm);
                        }
            }
        }


    }
}
