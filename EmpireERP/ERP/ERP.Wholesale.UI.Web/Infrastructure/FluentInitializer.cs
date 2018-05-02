using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Bizpulse.Infrastructure.FluentNHibernate;
using ERP.Infrastructure.NHibernate;
using ERP.Wholesale.Domain.NHibernate.Mappings;
using ERP.Wholesale.Settings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ERP.Wholesale.UI.Web.Infrastructure
{
    public class FluentInitializer : INHibernateInitializer
    {
        private static string serializedConfigurationPath = 
            AppDomain.CurrentDomain.BaseDirectory + "\\Configuration.serialized";

        private static string webConfigPath =
            AppDomain.CurrentDomain.BaseDirectory + "\\web.config";

        public FluentInitializer()
        {
            string dbServer = "dbServer", dbName = "dbName";

            Configuration cfg = GetConfiguration(dbServer, dbName);

            // в DebugMode генерим схему БД
            if (AppSettings.DebugMode)
            {
                var schemaExport = new SchemaExport(cfg);
                schemaExport.SetOutputFile(AppDomain.CurrentDomain.BaseDirectory + "\\Wholesale.sql")
                    .Execute(true, false, false);
            }

            SaveConfigurationToFile(cfg);
        }

        /// <summary>
        /// Получение конфигурации
        /// </summary>
        /// <param name="dbServer">Имя сервера БД</param>
        /// <param name="dbName">Имя БД</param>
        /// <returns></returns>
        public Configuration GetConfiguration(string dbServer, string dbName)
        {
            Configuration cfg = LoadConfigurationFromFile();

            if (cfg == null)
            {
                cfg = BuildConfiguration(dbServer, dbName);
            }

            // устанавливаем необходимые параметры строки соединения с БД
            cfg.Properties["connection.connection_string"] = "Data Source=" + dbServer + ";Initial Catalog=" + dbName + ";Integrated Security=True";
            cfg.Properties["cache.region_prefix"] = dbServer + "/" + dbName;

            return cfg;
        }

        private static Configuration BuildConfiguration(string dbServer, string dbName)
        {
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString("Data Source=" + dbServer + ";Initial Catalog=" + dbName + ";Integrated Security=True")
                    .DefaultSchema("dbo")
                    .AdoNetBatchSize(100))
                .ProxyFactoryFactory<ProxyFactoryFactory>()
                .Cache(c => c
                    .ProviderClass<NHibernate.Caches.SysCache2.SysCacheProvider>()
                    .UseQueryCache()
                    .UseMinimalPuts()
                    .UseSecondLevelCache()
                    .RegionPrefix(dbServer + "/" + dbName));

            // в DebugMode экспортируем маппинги в XML
            if (AppSettings.DebugMode)
            {
                /* Этот код требуется для экспорта маппингов в xml файлы. 
                 * Олег просил оставить этот код.
                 * 
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\_ExportedMappings";

                Directory.CreateDirectory(directoryPath);

                try
                {
                    string[] fileList = Directory.GetFiles(directoryPath, "*.xml");
                    foreach (string file in fileList)
                    {
                        File.Delete(file);
                    }
                }
                catch { }
                */

                configuration = configuration
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<OrganizationMap>()
                        .Conventions.AddFromAssemblyOf<AnsiStringConvention>())//; // подключаем конвенции
                    .Mappings(m => m.HbmMappings.AddFromAssemblyOf<OrganizationMap>());
                //.ExportTo(directoryPath));    //Использование экспорта приводит к тому, что отваливаются конвенции. Это справедливо для NH 3.1(3.2)
            }
            else
            {
                configuration = configuration
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<OrganizationMap>()
                        .Conventions.AddFromAssemblyOf<AnsiStringConvention>())//; // подключаем конвенции
                    .Mappings(m => m.HbmMappings.AddFromAssemblyOf<OrganizationMap>());
            }

            return configuration.BuildConfiguration();
        }

        private static bool IsConfigurationFileValid
        {
            get
            {
                var ass = Assembly.GetCallingAssembly();
                if (ass.Location == null)
                    return false;
                var configInfo = new FileInfo(serializedConfigurationPath);
                var assInfo = new FileInfo(ass.Location);
                var configFileInfo = new FileInfo(webConfigPath);
                if (configInfo.LastWriteTime < assInfo.LastWriteTime)
                    return false;
                if (configInfo.LastWriteTime < configFileInfo.LastWriteTime)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Сохранение конфигурации в файл
        /// </summary>
        private static void SaveConfigurationToFile(Configuration configuration)
        {
            using (var file = File.Open(serializedConfigurationPath, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, configuration);
            }
        }

        /// <summary>
        /// Загрузка конфигурации из файла
        /// </summary>
        private static Configuration LoadConfigurationFromFile()
        {
            if (IsConfigurationFileValid == false)
                return null;
            try
            {
                using (var file = File.Open(serializedConfigurationPath, FileMode.Open))
                {
                    var bf = new BinaryFormatter();

                    return bf.Deserialize(file) as Configuration;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

