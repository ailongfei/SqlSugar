﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SqlSugar
{
    public class SqlHelper : IDisposable
    {
        SqlConnection _sqlConnection;
        SqlTransaction _tran = null;
        public SqlHelper(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }
        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }
        public void CommitTran()
        {
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }
        public string GetString(string sql, params SqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        public int GetInt(string sql, params SqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }
        public object GetScalar(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            sqlCommand.Parameters.Clear();
            return scalar;
        }

        public int ExecuteCommand(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            int count = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return count;
        }

        public SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.Parameters.AddRange(pars);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqlCommand.Parameters.Clear();
            return sqlDataReader;
        }

        public DataTable GetDataTable(string sql, params SqlParameter[] pars)
        {
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return dt;
        }

        public DataSet GetDataSetAll(string sql, params SqlParameter[] pars)
        {
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return ds;
        }

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _sqlConnection.Close();
                }
            }
        }
    }
}
