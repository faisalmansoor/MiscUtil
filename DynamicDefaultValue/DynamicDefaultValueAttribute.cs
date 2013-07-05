using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace DynamicDefaultValue
{
    public class DynamicDefaultValueAttribute : DefaultValueAttribute
    {
        static DynamicDefaultValueAttribute()
        {
            try
            {
                foreach (DictionaryEntry v in Environment.GetEnvironmentVariables())
                {
                    DictionaryEntry v1 = v;
                    AddOrUpdate(v.Key.ToString(), key => v1.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize DynamicDefaultValueAttribute class. {0}", ex.Message);
            }
        }

        private static readonly ConcurrentDictionary<string, Func<string, object>> KeyValueMap =
            new ConcurrentDictionary<string, Func<string, object>>();

        public DynamicDefaultValueAttribute(string key)
            : base(null)
        {
            try
            {
                Func<string, object> mapper;
                if (KeyValueMap.TryGetValue(key, out mapper))
                {
                    object mapped = mapper(key);
                    SetValue(mapped);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to inititalize DynamicDefaultValueAttribute instance for key: {0}. {1}", key, ex);
            }
        }

        public static void AddOrUpdate(string key, Func<string, object> mapper)
        {
            KeyValueMap.AddOrUpdate(key, mapper, (k, v) => mapper);
        }

        public static void AddOrUpdate(string key, object value)
        {
            KeyValueMap.AddOrUpdate(key, k => (k2 => value), (k, v) => (k2 => value));
        }

        public static void Remove(string key)
        {
            Func<string, object> mapper;
            KeyValueMap.TryRemove(key, out mapper);
        }
    }
}