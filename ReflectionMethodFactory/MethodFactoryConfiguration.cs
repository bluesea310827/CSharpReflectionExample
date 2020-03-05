using System.Collections.Generic;

namespace ReflectionMethodFactory
{
    public class MethodFactoryConfiguration
    {
        /// <summary>
        /// Filter assemblies with assembly's prefix
        /// </summary>
        /// <value></value>
        public List<string> AssemblyPrefix { get; set; } = new List<string>();

        /// <summary>
        /// Filter assemblies with assembly's suffix (without extension)
        /// </summary>
        /// <value></value>
        public List<string> AssemblySuffix { get; set; } = new List<string>();

        /// <summary>
        /// Assembly's path
        /// </summary>
        /// <value></value>
        public string AssemblyRelatedPath { get; set; } = string.Empty;
    }
}