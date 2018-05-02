using System;
using Bizpulse.Admin.Domain.NHibernate.Mappings;
using Bizpulse.Infrastructure.FluentNHibernate;
using ERP.Infrastructure.NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Bizpulse.Admin.UI.Web
{
    public class FluentInitializer : INHibernateSingleDBInitializer
    {
        public FluentInitializer()
        {
            // в DebugMode генерим схему БД
            if (AppSettings.DebugMode)
            {
                Configuration cfg = GetConfiguration();
                
                var schemaExport = new SchemaExport(cfg);
                schemaExport.SetOutputFile(AppDomain.CurrentDomain.BaseDirectory + "\\Admin.sql")
                    .Execute(true, false, false);
            }
        }

        public Configuration GetConfiguration()
        {
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString("Data Source=" + AppSettings.DBServerName + ";Initial Catalog=" + AppSettings.DBName + ";Integrated Security=True")
                    .DefaultSchema("dbo")
                    .AdoNetBatchSize(100))
                .ProxyFactoryFactory<ProxyFactoryFactory>()
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ClientMap>()
                    .Conventions.AddFromAssemblyOf<AnsiStringConvention>()); // подключаем конвенции

            return configuration.BuildConfiguration();
        }
        
    }
}

