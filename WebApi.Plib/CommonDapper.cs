using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using WebApi.Plib;
//using Common;

namespace WebApi.Plib
{
    public class CommonDapper : IDisposable
    {


        //private string _sqlconn;

        //public CommonDapper(DataConnectType dct, string sqlconnstring)
        //{
        //    _clk = Plib.ep.GetSQLLink(dct, sqlconnstring);
        //}

        //public CommonDapper(Customerlink clk)
        //{
        //    _clk = clk;
        //}

        //public CommonDapper(DataConnectType dct)
        //{
        //    if (dct == DataConnectType.UserDBDataService)
        //    {
        //        _clk = Plib.ep.GetSQLLink(dct, Plib.ep.ServerDBInfo);
        //    }
        //    else
        //    {
        //        _clk = Plib.ep.GetSQLLink(dct, Plib.ep.TokenDBServerInfo);
        //    }
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
        }


        public static SqlConnection GetOpenConnection(DataConnectType dct, bool mars = false)
        {
            Customerlink _clk = new Customerlink();

            _clk = Plib.ep.GetSQLLink(dct);

            string cs = ConnString(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);
            if (mars)
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(cs);
                scsb.MultipleActiveResultSets = true;
                cs = scsb.ConnectionString;
            }
            var connection = new SqlConnection(cs);

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return connection;
        }


        static string ConnString(string Serverip, string DBname, string ServerName, string Password)
        {
            return "server=" + Serverip + ";Initial Catalog=" + DBname + "; User ID= " + ServerName + ";Password=" + Password + ";Connect Timeout=200";
        }


        public static string GetSearchStrByPage(string TableName, string FieldList, string SearchCondition, string orderStr,
            int PageIndex, int PageSize)
        {
            return
                "  BEGIN  "
                + " select " + FieldList + " from (select *,ROW_NUMBER() over(" + orderStr
                + ") as num from " + TableName + " " + SearchCondition + ")"
                + " as t where num between cast(((" + PageIndex.ToString() + "-1)*" + PageSize.ToString()
                + " + 1) as varchar(20)) and cast(" + PageIndex + "*" + PageSize + " as varchar(20))"
                + ";"
                + "SELECT COUNT(1)  as TotalNumber FROM " + TableName + " " + SearchCondition
                + ";"
                + "END;";

        }
        public static string GetSearchStrByAll(string TableName, string FieldList, string SearchCondition, string orderStr="")
        {
            return " select " + FieldList + " from (" + TableName + ") NT " + SearchCondition + orderStr;
        }



        /*
        /// <summary>   
        /// 根据参数化sql语句获取结果
        /// </summary>
        /// <param name="sql">参数化sql语句</param>
        /// <param name="paramList">参数对象列表</param>
        /// <returns></returns>
        public DataSet GetDataByParameterizedSql(string sql, List<CustomSqlParam> paramList)
        {

            SqlCommand command = new SqlCommand(sql);
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 200;
            SqlParameterCollection sqlParams = command.Parameters;
            foreach (var sqlParam in paramList)
            {
                sqlParams.Add(sqlParam.Name, sqlParam.Type);
                sqlParams[sqlParam.Name].Value = sqlParam.Value;
            }

            DataSet ds = new DataSet();
            using (DataAccess Access = new DataAccess())
            {
                Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);
                return Access.Fill(command, ds);
            }
        }

        public DataSet GetPageDataByParameterizedSql(List<CustomSqlParam> paramList,
            string TableName, string FieldList, string Condition,
             string OrderField, int PageIndex, int PageSize)
        {

            string strSQL =
                "  BEGIN  "
                + " select " + FieldList + " from (select *,ROW_NUMBER() over(" + OrderField
                + ") as num from " + TableName + " " + Condition + ")"
                + " as t where num between cast(((" + PageIndex.ToString() + "-1)*" + PageSize.ToString()
                + " + 1) as varchar(20)) and cast(" + PageIndex + "*" + PageSize + " as varchar(20))"
                + ";"
                + "SELECT COUNT(1) as TotalNumber FROM " + TableName + " " + Condition
                + ";"
                + "END;";

            SqlCommand command = new SqlCommand(strSQL);
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 200;
            SqlParameterCollection sqlParams = command.Parameters;

            foreach (var sqlParam in paramList)
            {
                sqlParams.Add(sqlParam.Name, sqlParam.Type);
                sqlParams[sqlParam.Name].Value = sqlParam.Value;
            }

            DataSet ds = new DataSet();
            using (DataAccess Access = new DataAccess())
            {
                Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);
                return Access.Fill(command, ds);
            }
        }

        /// <summary>
        /// 多表连接分页查询，多个表用重复的字段，不能用SELECT *,所以重载上面的方法，传递要查询的字段
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldList"></param>
        /// <param name="Condition"></param>
        /// <param name="OrderField"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public DataSet GetPageDataByParameterizedSql(List<CustomSqlParam> paramList,
          string TableName, string FieldList, string TableFiled, string Condition,
           string OrderField, int PageIndex, int PageSize)
        {

            string strSQL =
                "  BEGIN  "
                + " select " + FieldList + " from (select " + TableFiled + ",ROW_NUMBER() over(" + OrderField
                + ") as num from " + TableName + " " + Condition + ")"
                + " as t where num between cast(((" + PageIndex.ToString() + "-1)*" + PageSize.ToString()
                + " + 1) as varchar(20)) and cast(" + PageIndex + "*" + PageSize + " as varchar(20))"
                + ";"
                + "SELECT COUNT(1) as TotalNumber FROM " + TableName + " " + Condition
                + ";"
                + "END;";

            SqlCommand command = new SqlCommand(strSQL);
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 200;
            SqlParameterCollection sqlParams = command.Parameters;

            foreach (var sqlParam in paramList)
            {
                sqlParams.Add(sqlParam.Name, sqlParam.Type);
                sqlParams[sqlParam.Name].Value = sqlParam.Value;
            }

            DataSet ds = new DataSet();
            using (DataAccess Access = new DataAccess())
            {
                Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);
                return Access.Fill(command, ds);
            }
        }


        /// <summary>
        /// 执行参数化sql语句
        /// </summary>
        /// <param name="sql">参数化sql语句</param>
        /// <param name="paramList">参数对象列表</param>
        /// <returns></returns>
        public int ExecuteParameterizedSql(string sql, List<CustomSqlParam> paramList)
        {

            SqlCommand command = new SqlCommand(sql);
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 200;

            SqlParameterCollection sqlParams = command.Parameters;
            foreach (var sqlParam in paramList)
            {
                sqlParams.Add(sqlParam.Name, sqlParam.Type);
                sqlParams[sqlParam.Name].Value = sqlParam.Value;
            }

            DataSet ds = new DataSet();
            using (DataAccess Access = new DataAccess())
            {
                Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);
                return Access.ExecuteNonQuery(command);
            }
        }


        /// <summary>
        /// 执行参数化事务语句
        /// </summary>
        /// <param name="sqlDirectionary"></param>
        /// <returns></returns>
        public int ExecuteTransSql(Dictionary<String, List<CustomSqlParam>> sqlDirectionary)
        {

            SqlConnection conn = null;

            SqlTransaction trans = null;

            int status = 1;

            try
            {
                DateTime dtnow = DateTime.Now;

                using (DataAccess Access = new DataAccess())
                {
                    Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);

                    conn = Access.Connection(true);

                    trans = conn.BeginTransaction();

                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.Transaction = trans;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;

                    foreach (var item in sqlDirectionary)
                    {

                        string sqlstr = item.Key;
                        command.CommandText = sqlstr;
                        SqlParameterCollection sqlParams = command.Parameters;
                        sqlParams.Clear();
                        foreach (var sqlParam in item.Value)
                        {

                            sqlParams.Add(sqlParam.Name, sqlParam.Type);
                            sqlParams[sqlParam.Name].Value = sqlParam.Value;
                        }
                        int count = command.ExecuteNonQuery();
                    }

                    trans.Commit();

                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                status = 0;
            }
            trans.Dispose();

            conn.Close();
            return status;
        }


        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="mappingname"></param>
        /// <param name="OnRowsCopied"></param>
        public bool BulkInsert(DataSet model)
        {
            SqlConnection conn = null;
            SqlTransaction sqlTran = null;
            bool result = false;
            try
            {
                using (DataAccess Access = new DataAccess())
                {
                    Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);

                    conn = Access.Connection(true);
                    sqlTran = conn.BeginTransaction(); //开始事务

                    using (SqlBulkCopy sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, sqlTran))
                    {
                        sqlBulk.BulkCopyTimeout = 5000;
                        sqlBulk.DestinationTableName = model.Tables[0].TableName;
                        sqlBulk.WriteToServer(model.Tables[0]);
                        sqlTran.Commit();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();

            }
            finally
            {
                sqlTran.Dispose();
                conn.Close();

            }
            return result;
        }

        /// <summary>
        /// 事务执行sql后批量插入
        /// </summary>
        /// <param name="sqlDirectionary"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ExecuteThenInsertTrans(Dictionary<String, List<CustomSqlParam>> sqlDirectionary, DataSet model)
        {

            SqlConnection conn = null;

            SqlTransaction trans = null;

            SqlBulkCopy sqlBulk = null;

            int status = 1;

            try
            {
                DateTime dtnow = DateTime.Now;

                using (DataAccess Access = new DataAccess())
                {
                    Access.SetSQLLinkParam(_clk.m_ServerIP, _clk.m_DbName, _clk.m_ServerID, _clk.m_PassWord);

                    conn = Access.Connection(true);

                    trans = conn.BeginTransaction();

                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.Transaction = trans;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;

                    foreach (var item in sqlDirectionary)
                    {

                        string sqlstr = item.Key;
                        command.CommandText = sqlstr;
                        SqlParameterCollection sqlParams = command.Parameters;
                        sqlParams.Clear();
                        foreach (var sqlParam in item.Value)
                        {

                            sqlParams.Add(sqlParam.Name, sqlParam.Type);
                            sqlParams[sqlParam.Name].Value = sqlParam.Value;
                        }
                        int count = command.ExecuteNonQuery();
                    }

                    ///最终执行插入
                    sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans);
                    sqlBulk.BulkCopyTimeout = 5000;
                    sqlBulk.DestinationTableName = model.Tables[0].TableName;
                    sqlBulk.WriteToServer(model.Tables[0]);

                    trans.Commit();

                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                status = 0;
            }
            finally
            {
                trans.Dispose();
                sqlBulk.Close();
                conn.Close();
            }
            return status;
        }
         * */


    }
}
