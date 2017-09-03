using System;
using System.Collections.Generic;

namespace GutenTag
{
    internal class TagPropertyFactory
    {
        private readonly Dictionary<string, Func<TagProperty>> factories = new Dictionary<string, Func<TagProperty>>();

        public void Default<T>() where T : TagProperty, new()
        {
            Register<T>(string.Empty);
        }

        public void Default(Func<TagProperty> factory)
        {
            Register(string.Empty, factory);
        }

        public void Register(string name, Func<TagProperty> factory)
        {
            if (factory == null)
            {
                factories.Remove(name);
            }
            else
            {
                factories.Add(name, factory);
            }
        }

        public void Register<T>(string name) where T : TagProperty, new()
        {
            Register(name, () => new T());
        }

        public void Register<T, TModifier>(string name) where T : TagProperty, new()
            where TModifier : TagPropertyModifier, new()
        {
            Register<T>(name);
            RegisterModifier<TModifier>(name);
        }

        public void Register<T, TModifier, TModifier2>(string name)
            where T : TagProperty, new()
            where TModifier : TagPropertyModifier, new()
            where TModifier2 : TagPropertyModifier, new()
        {
            Register<T>(name);
            RegisterModifier<TModifier>(name);
            RegisterModifier<TModifier2>(name);
        }

        public void Register<T, TModifier, TModifier2, TModifier3>(string name)
            where T : TagProperty, new()
            where TModifier : TagPropertyModifier, new()
            where TModifier2 : TagPropertyModifier, new()
            where TModifier3 : TagPropertyModifier, new()
        {
            Register<T>(name);
            RegisterModifier<TModifier>(name);
            RegisterModifier<TModifier2>(name);
            RegisterModifier<TModifier3>(name);
        }

        public void Register<T, TModifier, TModifier2, TModifier3, TModifier4>(string name)
            where T : TagProperty, new()
            where TModifier : TagPropertyModifier, new()
            where TModifier2 : TagPropertyModifier, new()
            where TModifier3 : TagPropertyModifier, new()
            where TModifier4 : TagPropertyModifier, new()
        {
            Register<T>(name);
            RegisterModifier<TModifier>(name);
            RegisterModifier<TModifier2>(name);
            RegisterModifier<TModifier3>(name);
            RegisterModifier<TModifier4>(name);
        }


        public void Register<T, TModifier, TModifier2, TModifier3, TModifier4, TModifier5>(string name)
            where T : TagProperty, new()
            where TModifier : TagPropertyModifier, new()
            where TModifier2 : TagPropertyModifier, new()
            where TModifier3 : TagPropertyModifier, new()
            where TModifier4 : TagPropertyModifier, new()
            where TModifier5 : TagPropertyModifier, new()
        {
            Register<T>(name);
            RegisterModifier<TModifier>(name);
            RegisterModifier<TModifier2>(name);
            RegisterModifier<TModifier3>(name);
            RegisterModifier<TModifier4>(name);
            RegisterModifier<TModifier5>(name);
        }


        public void RegisterModifier<T>(string name) where T : TagPropertyModifier, new()
        {
            if (factories.ContainsKey(name))
            {
                var factory = factories[name];
                factories[name] = () =>
                {
                    var property = factory();
                    property.AddModifier(new T());
                    return property;
                };
            }
        }

        public Func<TagProperty> Unregister(string name)
        {
            var result = factories.ContainsKey(name) ? factories[name] : null;
            factories.Remove(name);
            return result;
        }

        public TagProperty Create(string name)
        {
            name = name ?? string.Empty;
            if (factories.ContainsKey(name))
            {
                return factories[name]();
            }
            if (factories.ContainsKey(string.Empty))
            {
                return factories[string.Empty]();
            }
            return null;
        }
    }
}