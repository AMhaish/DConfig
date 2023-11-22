using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Design;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SABFramework.ModulesUtilities
{
    /// <summary>
    /// Database Initializer to create tables only if they don't already exist.
    /// It will never drop the database.  Does not check the model for compatibility.
    /// </summary>
    /// <typeparam name="TContext">The data context</typeparam>
    public class CreateTablesOnlyIfTheyDontExist<TContext> : IDatabaseInitializer<TContext>
      where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {

                // get the object context
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var script = objectContext.CreateDatabaseScript() + '\n' + GenerateIndexesScript(context);
                // If the database doesn't exist at all then just create it like normal.
                if (!context.Database.Exists())
                {
                    context.Database.Create();
                    //objectContext.ExecuteStoreCommand(DropAllObjects);
                    //objectContext.ExecuteStoreCommand(script);
                    return;
                }

                // get the database creation script
                if (context.Database.Connection is SqlConnection)
                {
                    // for SQL Server, we'll just alter the script
                    // add existance checks to the table creation statements
                    script = Regex.Replace(script,
                      @"create table \[(\w+)\]\.\[(\w+)\]",
                      "if not exists (select * from INFORMATION_SCHEMA.TABLES " +
                      "where TABLE_SCHEMA='$1' and TABLE_NAME = '$2')\n$&");
                    // add existance checks to the table constraint creation statements
                    script = Regex.Replace(script,
                      @"alter table \[(\w+)\]\.\[(\w+)\] add constraint \[(\w+)\]",
                      "if not exists (select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS " +
                      "where TABLE_SCHEMA='$1' and TABLE_NAME = '$2' " +
                      "and CONSTRAINT_NAME = '$3')\n$&");

                    // run the modified script
                    objectContext.ExecuteStoreCommand(script);
                }
                //else if (context.Database.Connection is SqlCeConnection)
                //{
                //    // SQL CE doesn't let you use inline existance checks,
                //    // so we have to parse each statement out and check separately.

                //    var statements = script.Split(new[] { ";\r\n" },
                //                    StringSplitOptions.RemoveEmptyEntries);
                //    foreach (var statement in statements)
                //    {
                //        var quoteSplitStrings = statement.Split('"');
                //        if (statement.StartsWith("CREATE TABLE"))
                //        {
                //            // Create a table if it does not exist.
                //            var tableName = quoteSplitStrings[1];
                //            const string sql =
                //              "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                //              "WHERE TABLE_NAME='{0}'";
                //            var checkScript = string.Format(sql, tableName);
                //            if (objectContext.ExecuteStoreQuery<int>(checkScript).First() == 0)
                //                objectContext.ExecuteStoreCommand(statement);
                //        }
                //        else if (statement.Contains("ADD CONSTRAINT"))
                //        {
                //            // Add a table constraint if it does not exist.
                //            var tableName = quoteSplitStrings[1];
                //            var constraintName = quoteSplitStrings[3];
                //            const string sql =
                //              "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS " +
                //              "WHERE TABLE_NAME='{0}' AND CONSTRAINT_NAME='{1}'";
                //            var checkScript = string.Format(sql, tableName, constraintName);
                //            if (objectContext.ExecuteStoreQuery<int>(checkScript).First() == 0)
                //                objectContext.ExecuteStoreCommand(statement);
                //        }
                //        else
                //        {
                //            // Not sure what else it could be. Just run it.
                //            objectContext.ExecuteStoreCommand(statement);
                //        }
                //    }
                //}
                else
                {
                    throw new InvalidOperationException(
                      "This initializer is only compatible with SQL Server or SQL Compact Edition"
                      );
                }
            }
        }

        public string GenerateIndexesScript(TContext context)
        {
            const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
            string query = string.Empty;
            Dictionary<IndexAttribute, List<string>> indexes = new Dictionary<IndexAttribute, List<string>>();
            foreach (var dataSetProperty in typeof(TContext).GetProperties(PublicInstance).Where(p => p.PropertyType.Name == typeof(DbSet<>).Name))
            {
                var entityType = dataSetProperty.PropertyType.GetGenericArguments().Single();
                TableAttribute[] tableAttributes = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), false);

                indexes.Clear();
                //string tableName = tableAttributes.Length != 0 ? tableAttributes[0].Name : dataSetProperty.Name;
                string tableName = entityType.Name + 's';

                foreach (PropertyInfo property in entityType.GetProperties(PublicInstance))
                {
                    IndexAttribute[] indexAttributes = (IndexAttribute[])property.GetCustomAttributes(typeof(IndexAttribute), false);
                    NotMappedAttribute[] notMappedAttributes = (NotMappedAttribute[])property.GetCustomAttributes(typeof(NotMappedAttribute), false);
                    if (indexAttributes.Length > 0 && notMappedAttributes.Length == 0)
                    {
                        ColumnAttribute[] columnAttributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), false);

                        foreach (IndexAttribute indexAttribute in indexAttributes)
                        {
                            if (!indexes.ContainsKey(indexAttribute))
                            {
                                indexes.Add(indexAttribute, new List<string>());
                            }

                            if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                            {
                                string columnName = columnAttributes.Length != 0 ? columnAttributes[0].Name : property.Name;
                                indexes[indexAttribute].Add(columnName);
                            }
                            else
                            {
                                indexes[indexAttribute].Add(property.PropertyType.Name + "_" + GetKeyName(property.PropertyType));
                            }
                        }
                    }
                }

                foreach (IndexAttribute indexAttribute in indexes.Keys)
                {
                    query += CreateIndexQueryTemplate.Replace("{indexName}", "IX_" + tableName + "_" + string.Join("_", indexes[indexAttribute].ToArray() ))
                                .Replace("{tableName}", tableName)
                                .Replace("{columnName}", string.Join(", ", indexes[indexAttribute].ToArray()))
                                .Replace("{unique}", indexAttribute.IsUnique ? "UNIQUE" : string.Empty);
                }
            }

            return query;
        }
        private string GetKeyName(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) != null)
                    return propertyInfo.Name;
            }
            throw new Exception("No property was found with the attribute Key");
        }

        private const string CreateIndexQueryTemplate = "IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = '{indexName}') CREATE {unique} INDEX {indexName} ON {tableName} ({columnName});";

        private const string DropAllObjects = @"
    declare @n char(1)
    set @n = char(10)

    declare @stmt nvarchar(max)

    -- procedures
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop procedure [' + name + ']'
    from sys.procedures

    -- check constraints
    select @stmt = isnull( @stmt + @n, '' ) +
        'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']'
    from sys.check_constraints

    -- functions
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop function [' + name + ']'
    from sys.objects
    where type in ( 'FN', 'IF', 'TF' )

    -- views
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop view [' + name + ']'
    from sys.views

    -- foreign keys
    select @stmt = isnull( @stmt + @n, '' ) +
        'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']'
    from sys.foreign_keys

    -- tables
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop table [' + name + ']'
    from sys.tables

    -- user defined types
    select @stmt = isnull( @stmt + @n, '' ) +
        'drop type [' + name + ']'
    from sys.types
    where is_user_defined = 1

    exec sp_executesql @stmt";


    }
}
