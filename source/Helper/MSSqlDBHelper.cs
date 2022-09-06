using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Helper
{
    public class MSSqlDBHelper
    {

        private static string _connectionString = string.Empty;
        //private static string _tempConnectionString = "Data Source=localhost;Persist Security Info=True;User ID=sa1;Password=P@ssword;Initial Catalog=spares_warehouse";

        public MSSqlDBHelper(IConfiguration config)
        {
            _connectionString = config.GetSection("MSSql").GetValue<string>("ConnectionString");
        }

        public MSSqlDBHelper()
        {

        }

        /// <summary>
        /// 返回受影響行數
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return conn.Execute(sql, param, transaction, null, commandType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return conn.ExecuteScalar<T>(sql, param, transaction, null, commandType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<T> ExecuteQuery<T>(string sql, object param = null, CommandType? commandType = null)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return conn.Query<T>(sql, param, null, true, null, commandType).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
