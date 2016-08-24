﻿using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace WebCMS.Entities.Models.Initializers
{
    public class DropAndMigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext, new() where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly TMigrationsConfiguration config;

        public DropAndMigrateDatabaseToLatestVersion()
        {
            this.config = new TMigrationsConfiguration();
        }

        public void InitializeDatabase(TContext context)
        {
            context.Database.Delete();

            var migrator = new DbMigrator(this.config);
            migrator.Update();

            this.Seed(context);
        }

        public virtual void Seed(TContext context)
        {

        }
    }
}
