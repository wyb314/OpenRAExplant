using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Primitives;
using System.Reflection;
using Engine.Network;

namespace Engine
{
    public sealed class ObjectCreator : IDisposable
    {
        readonly Cache<string, Type> typeCache;
        readonly Cache<Type, ConstructorInfo> ctorCache;
        readonly Pair<Assembly, string>[] assemblies;

        public ObjectCreator(Manifest manifest)
        {
            typeCache = new Cache<string, Type>(FindType);
            ctorCache = new Cache<Type, ConstructorInfo>(GetCtor);
        }

        Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.FullName == e.Name)
                    return a;

            if (assemblies == null)
                return null;

            return assemblies.Select(a => a.First).FirstOrDefault(a => a.FullName == e.Name);
        }

        public static Action<string> MissingTypeAction =
            s => { throw new InvalidOperationException(string.Format("Cannot locate type: {0}",s)); };

        public T CreateObject<T>(string className)
        {
            return CreateObject<T>(className, new Dictionary<string, object>());
        }

        public T CreateObject<T>(string className, Dictionary<string, object> args)
        {
            var type = typeCache[className];
            if (type == null)
            {
                MissingTypeAction(className);
                return default(T);
            }

            var ctor = ctorCache[type];
            if (ctor == null)
                return (T)CreateBasic(type);
            else
                return (T)CreateUsingArgs(ctor, args);
        }

        public Type FindType(string className)
        {
            return assemblies
                .Select(pair => pair.First.GetType(pair.Second + "." + className, false))
                .FirstOrDefault(t => t != null);
        }

        public ConstructorInfo GetCtor(Type type)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var ctors = type.GetConstructors(flags).Where(x => x.HasAttribute<UseCtorAttribute>());
            if (ctors.Count() > 1)
                throw new InvalidOperationException("ObjectCreator: UseCtor on multiple constructors; invalid.");
            return ctors.FirstOrDefault();
        }

        public object CreateBasic(Type type)
        {
            return type.GetConstructor(new Type[0]).Invoke(new object[0]);
        }

        public object CreateUsingArgs(ConstructorInfo ctor, Dictionary<string, object> args)
        {
            var p = ctor.GetParameters();
            var a = new object[p.Length];
            for (var i = 0; i < p.Length; i++)
            {
                var key = p[i].Name;
                if (!args.ContainsKey(key)) throw new InvalidOperationException("ObjectCreator: key `{0}' not found".F(key));
                a[i] = args[key];
            }

            return ctor.Invoke(a);
        }

        public IEnumerable<Type> GetTypesImplementing<T>()
        {
            var it = typeof(T);
            return GetTypes().Where(t => t != it && it.IsAssignableFrom(t));
        }

        public IEnumerable<Type> GetTypes()
        {
            return assemblies.Select(ma => ma.First).Distinct()
                .SelectMany(ma => ma.GetTypes());
        }

        public TLoader[] GetLoaders<TLoader>(IEnumerable<string> formats, string name)
        {
            var loaders = new List<TLoader>();
            foreach (var format in formats)
            {
                var loader = FindType(format + "Loader");
                if (loader == null || !loader.GetInterfaces().Contains(typeof(TLoader)))
                    throw new InvalidOperationException("Unable to find a {0} loader for type '{1}'.".F(name, format));

                loaders.Add((TLoader)CreateBasic(loader));
            }

            return loaders.ToArray();
        }

        ~ObjectCreator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
                AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
        }

        [AttributeUsage(AttributeTargets.Constructor)]
        public sealed class UseCtorAttribute : Attribute { }
    }
}
