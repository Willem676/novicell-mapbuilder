﻿using System;
using System.Linq;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Web;

namespace MapBuilder.Library.Migration
{
    public class MigrationHelper
    {
        public void HandleMapBuilderMigration()
        {
            const string productName = "NcMapBuilder";
            var currentVersion = new SemVersion(0, 0, 0);

            var migrations = ApplicationContext.Current.Services.MigrationEntryService.GetAll(productName);
            var latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

            if (latestMigration != null)
            {
                currentVersion = latestMigration.Version;
            }

            var targetVersion = new SemVersion(1, 0, 0);
            if (targetVersion == currentVersion) return;

            var migrationsRunner = new MigrationRunner(
               ApplicationContext.Current.Services.MigrationEntryService,
               ApplicationContext.Current.ProfilingLogger.Logger,
               currentVersion,
               targetVersion,
               productName);

            try
            {
                migrationsRunner.Execute(UmbracoContext.Current.Application.DatabaseContext.Database);
            }
            catch (Exception e)
            {
                LogHelper.Error<MapBuilderMigration>("Error running NcMapBuilder migration", e);
            }
        }
    }
}
