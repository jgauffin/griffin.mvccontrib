using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace AdminTest.Modules
{
    public static class AutofacExtensions
    {
        public static void RegisterModules(this ContainerBuilder builder, Assembly assembly)
        {
            var moduleType = typeof (Module);
            foreach (var type in assembly.GetTypes().Where(moduleType.IsAssignableFrom))
            {
                builder.RegisterModule((Module) Activator.CreateInstance(type));
            }
        }
    }
}