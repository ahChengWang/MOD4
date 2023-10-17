//using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Utility.Helper
{
    public class OracleDBHelper
    {

        private static string _connectionString = string.Empty;

        public OracleDBHelper(IConfiguration config)
        {
            _connectionString = config.GetSection("Oracle").GetValue<string>("ConnectionString");
        }

        public OracleDBHelper()
        {

        }

        /// <summary>
        /// 返回受影響行數
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, List<(string, string)> paraList = null)
        {
            try
            {
                int _out = 0;

                using (OracleConnection conn = new OracleConnection(_connectionString))
                {
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();

                        cmd.BindByName = true;

                        cmd.CommandText = sql;

                        foreach (var para in paraList)
                            cmd.Parameters.Add(new OracleParameter(para.Item1, para.Item2));

                        _out = cmd.ExecuteNonQuery();
                    }
                }

                return _out;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T ExecuteScalar<T>(string sql, List<(string, string)> paraList = null) where T : new()
        {
            try
            {
                T _out = new T();

                using (OracleConnection conn = new OracleConnection(_connectionString))
                {
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();

                        cmd.BindByName = true;

                        cmd.CommandText = sql;

                        foreach (var para in paraList)
                            cmd.Parameters.Add(new OracleParameter(para.Item1, para.Item2));

                        _out = (T)cmd.ExecuteScalar();
                    }
                }

                return _out;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> ExecuteQuery<T>(string sql, List<(string, string)> paraList = null) where T : new()
        {
            try
            {
                DataSet dataSet = new DataSet();
                List<T> _outList = new List<T>();

                using (OracleConnection conn = new OracleConnection(_connectionString))
                {
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();

                        cmd.BindByName = true;

                        cmd.CommandText = sql;

                        if (paraList != null && paraList.Any())
                            foreach (var para in paraList)
                                cmd.Parameters.Add(new OracleParameter(para.Item1, para.Item2));

                        using (OracleDataAdapter sqlAdapter = new OracleDataAdapter(cmd))
                        {
                            sqlAdapter.SelectCommand = cmd;
                            sqlAdapter.Fill(dataSet);
                        }

                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            T _tmp = new T();
                            foreach (DataColumn col in dataSet.Tables[0].Columns)
                            {
                                var _rmp = _tmp.GetType().GetProperty(col.ColumnName);
                                if (_rmp == null)
                                    continue;
                                _rmp.SetValue(_tmp, row[col]);
                            }
                            _outList.Add(_tmp);
                        }
                    }
                }

                return _outList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
