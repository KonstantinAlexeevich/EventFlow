// The MIT License (MIT)
// 
// Copyright (c) 2015-2020 Rasmus Mikkelsen
// Copyright (c) 2015-2020 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Configuration;
using EventFlow.Core;
using EventFlow.Extensions;
using EventFlow.MetadataProviders;
using EventFlow.SQLite.Connections;
using EventFlow.SQLite.EventStores;
using EventFlow.SQLite.Extensions;
using EventFlow.SQLite.Tests.TestHelpers;
using EventFlow.TestHelpers;
using EventFlow.TestHelpers.Suites;
using NUnit.Framework;

namespace EventFlow.SQLite.Tests.IntegrationTests.EventStores
{
    [Category(Categories.Integration)]
    public class SQLiteEventStoreTests : TestSuiteForEventStore
    {
        private ISQLiteDatabase _testDatabase;

        protected override IRootResolver CreateRootResolver(IEventFlowOptions eventFlowOptions)
        {
            _testDatabase = SQLiteHelpz.CreateDatabase("eventflow");

            var resolver = eventFlowOptions
                .ConfigureSQLite(SQLiteConfiguration.New.SetConnectionString(_testDatabase.ConnectionString.Value))
                .UseEventStore<SQLiteEventPersistence>()
                .CreateResolver();

            var databaseMigrator = resolver.Resolve<ISQLiteDatabaseMigrator>();
            EventFlowEventStoresSQLite.MigrateDatabase(databaseMigrator);
            databaseMigrator.MigrateDatabaseUsingEmbeddedScripts(GetType().Assembly);

            return resolver;
        }

        [TearDown]
        public void TearDown()
        {
            _testDatabase.Dispose();
        }
    }
}