using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public static class GenericDataProvider<TModelStore, TKey>
    {
        private static IDictionary<Type, Type> DataProviders { get; set; }

        static GenericDataProvider()
        {
            DataProviders = new Dictionary<Type, Type>();
            RegisterDefaultProviders();
        }

        /// <summary>
        /// Register all providers in this assembly that implement IGenericDataProvider
        /// </summary>
        private static void RegisterDefaultProviders()
        {
            // Get types in assembly
            var providerTypes = Assembly
                .GetAssembly(typeof(GenericDataProvider<TModelStore, TKey>))
                .GetTypes()
                .Where(x => x.IsClass && x.GetInterfaces()
                    .Any(y => y.IsGenericType 
                        && y.GetGenericTypeDefinition() == typeof(IDataProvider<,>)
                        && y.GetGenericArguments()[1] == typeof(TKey)));

            foreach(var providerType in providerTypes)
            {
                var interfaceType = providerType
                    .GetInterfaces()
                    .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDataProvider<,>));
                var dataType = interfaceType.GetGenericArguments()[0];
                Register(dataType, providerType);
            }
        }

        private static ConstructorInfo GetProviderConstructor(Type providerType)
        {
            var constructor = providerType.GetConstructor(new Type[] { typeof(IProviderContext<TModelStore>) });
            return constructor;
        }

        public static void Register(Type dataType, Type dataProviderType)
        {
            var key = dataType;
            if (DataProviders.ContainsKey(key) == false)
            {
                var constructor = GetProviderConstructor(dataProviderType);                
                if (constructor == null)
                {
                    throw new Exception($"Error while registering GenericDataProvider: Data provider does not have a constructor with the required type: {typeof(TModelStore)}");
                }
                DataProviders.Add(key, dataProviderType);
            }
            else
            {
                throw new Exception($"Provider IGenericDataProvider<{dataType.Name}> already registered.");
            }
        }

        public static IDataProvider<TData, TKey> GetGenericProvider<TData>(TModelStore store, string userName = "", string userId = "", LeagueRoleEnum roles = LeagueRoleEnum.None)
        {
            var key = typeof(TData);
            if (DataProviders.ContainsKey(key))
            {
                var dataProviderType = DataProviders[key];
                var constructor = GetProviderConstructor(dataProviderType);
                var dataProvider = constructor?.Invoke(new object[] { store });
                return (IDataProvider<TData, TKey>)dataProvider;
            }
            return null;
        }

        public static IDataProvider<TKey> GetProvider(Type type, IProviderContext<TModelStore> context)
        {
            var key = type;
            if (DataProviders.ContainsKey(key))
            {
                var dataProviderType = DataProviders[key];
                var constructor = GetProviderConstructor(dataProviderType);
                var dataProvider = constructor?.Invoke(new object[] { context });
                return (IDataProvider<TKey>)dataProvider;
            }
            return null;
        }
    }
}
