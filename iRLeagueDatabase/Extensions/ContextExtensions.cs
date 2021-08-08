using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Extensions
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Get the table name from a <see cref="DbSet"/>
        /// </summary>
        /// <param name="dbSet">set to provide the table name</param>
        /// <returns>Name of the table linked to the <see cref="DbSet"/> entity type</returns>
        public static string GetTableName(this DbSet dbSet)
        {
            string sql = dbSet.Sql;
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        /// <summary>
        /// Add a condition to the standard sql query of a <see cref="DbSet"/>
        /// </summary>
        /// <param name="dbSet">set to perform the query</param>
        /// <param name="condition">condition in valid sql syntax e.g.: "Column1 = Value"</param>
        /// <returns>Altered sql query with the added condition</returns>
        public static string AddConditionToSql(this DbSet dbSet, string condition)
        {
            string sql = dbSet.Sql;
            Regex regex = new Regex("WHERE (?<params>.*)");
            Match match = regex.Match(sql);

            if (match.Success)
            {
                sql = regex.Replace(sql, match.ToString() + " AND " + condition);
            }
            else
            {
                sql = sql + "\n    WHERE " + condition;
            }
            
            return sql;
        }
    }
}
