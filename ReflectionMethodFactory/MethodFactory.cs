using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ReflectionMethodFactory
{
    public class MethodFactory
    {
        private static List<(Type MethodAssemblyType, MethodInfo Method)> AssemblyMethods { get; set; } = new List<(Type MethodAssemblyType, MethodInfo Method)>();

        public MethodFactory() : this (new MethodFactoryConfiguration())
        {
        }

        public MethodFactory(MethodFactoryConfiguration configuration)
        {
            LoadAssemblyMethods(configuration);
        }

        /// <summary>
        /// Dynamic get method.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Delegate GetInvokeMethod(string methodName, object[] args)
        {
            // 取得指定的MethodInfo
            var methodInfo = AssemblyMethods.Where(x=>x.Method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            // 找不到時回 null
            if(methodInfo.MethodAssemblyType == null) return null;
            
            // 建立該MethodInfo的實體物件
            var targetInstance = methodInfo.MethodAssemblyType.Assembly.CreateInstance(methodInfo.MethodAssemblyType.FullName, true, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, args, null, null);

            // 取得該Method的輸入參數和回傳結果的Type (For delegate)
            Func<Type[], Type> getType;
            var isAction = methodInfo.Method.ReturnType.Equals((typeof(void)));
            var types = methodInfo.Method.GetParameters().Select(p => p.ParameterType);

            if (isAction) 
            {
                // 建立Action<>
                getType = Expression.GetActionType;
            }
            else 
            {
                // 建立Func<>
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.Method.ReturnType });
            }

            // 建立Delegate
            if (methodInfo.Method.IsStatic) return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo.Method);

            return Delegate.CreateDelegate(getType(types.ToArray()), targetInstance, methodName);
        }

        /// <summary>
        /// Load all assembly's methods
        /// </summary>
        /// <param name="configuration"></param>
        private void LoadAssemblyMethods(MethodFactoryConfiguration configuration)
        {
            var assemblyFiles = LoadEntryAssemblyFiles(configuration);

            foreach(var assemblyFile in assemblyFiles)
            {
                var assembly = Assembly.LoadFrom(assemblyFile);
                var assemblyTypes = assembly.GetTypes().Where(x=>x.IsClass && !x.IsDefined(typeof(CompilerGeneratedAttribute), false));

                foreach(var assemblyType in assemblyTypes)
                {
                    var methodInfos = assemblyType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    var assemblyMethods = methodInfos.Select(x=> (assemblyType, x));
                    AssemblyMethods.AddRange(assemblyMethods);
                }
            }
        }

        /// <summary>
        /// Get assemblies
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>Assembly list (Absolute path)</returns>
        private List<string> LoadEntryAssemblyFiles(MethodFactoryConfiguration configuration)
        {
            var searchDirectories = new List<string>() { this.GetEntryAssemblyDirectory(configuration.AssemblyRelatedPath), AppDomain.CurrentDomain.BaseDirectory };
            
            //SharedFolder開頭雙斜線不能任意清除
            searchDirectories = searchDirectories.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Replace("\\", @"/").TrimEnd(new[] { '/', '\\' })).Distinct().ToList();
            var assemblyFiles = new List<string>();
            var properDirectories = new List<string>();
            foreach (var sd in searchDirectories)
            {
                var localAssemblyFiles = Directory.GetFiles(sd).Where(x => Path.GetExtension(x).Equals(".dll", StringComparison.OrdinalIgnoreCase)
                                                                        && (AssemblyFilter(configuration.AssemblyPrefix, x) || AssemblyFilter(configuration.AssemblySuffix, x)))
                    .Select(x => Path.GetFullPath(x).Replace("\\", @"/")) 
                    .ToList();
                assemblyFiles.AddRange(localAssemblyFiles);
                properDirectories.Add(sd);
            }

            return assemblyFiles.Distinct().ToList();
        }

        /// <summary>
        /// Assembly Filter
        /// </summary>
        /// <param name="filterConditions"></param>
        /// <param name="AssemblyFullName"></param>
        /// <returns></returns>
        private bool AssemblyFilter(List<string> filterConditions, string AssemblyFullName)
        {
            if (filterConditions.Count.Equals(0)) return true;

            return filterConditions.Any(prefix => Path.GetFileName(AssemblyFullName).StartsWith(prefix));
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <returns>File path</returns>
        private string GetEntryAssemblyDirectory(string relatedPath)
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) assembly = Assembly.GetCallingAssembly();
            var codeBase = assembly.CodeBase;
            var finalPath = Path.Combine(Path.GetDirectoryName(new Uri(codeBase).LocalPath) + @"/", relatedPath);
            return finalPath;
        }
    }
}