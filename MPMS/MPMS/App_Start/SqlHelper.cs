using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MPMS.App_Start
{
    public static class SqlHelper
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;//获取web.config之中名称为OAConn的链接字符串

        /// <summary>
        /// 通过SQL语句查询数据
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSql)
        {
            using (SqlConnection conn = new SqlConnection(strConn)) //通过strConn字符串连接到数据库，如果链接完成则释放资源关闭链接
            {
                DataTable dt = new DataTable();
                if (conn.State == ConnectionState.Closed)//如果链接状态为关闭，则打开链接
                {
                    conn.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(strSql, conn);//通过SQL语句以及链接状态，从数据库之中读取数据
                    sda.Fill(dt);//填充数据
                }
                return dt;
            }
        }

        /// <summary>
        /// 根据sql查询首行首列
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static object GetScalar(string strSql)
        {
            using (SqlConnection conn = new SqlConnection(strConn))//声明一个连接对象，用完自动释放
            {
                object o = "";//声明一个object对象

                if (conn.State == ConnectionState.Closed)//判断状态，关闭则开启
                {
                    conn.Open();//打开连接
                    try
                    {
                        SqlCommand com = new SqlCommand(strSql, conn);//根据sql和连接对象获取一个查询对象
                        o = com.ExecuteScalar();
                    }
                    catch (Exception ex)//捕获异常
                    {
                        throw ex;//抛出异常
                    }
                }

                return o;//返回对象
            }

        }

        /// <summary>
        /// 执行新增，修改，删除语句
        /// </summary>
        /// <param name="strSql">需要执行的SQL语句</param>
        /// <returns></returns>
        public static int ExcueNoQuery(string strSql)
        {
            using (SqlConnection conn = new SqlConnection(strConn)) //通过strConn字符串连接到数据库，如果链接完成则释放资源关闭链接
            {
                int result = 0;
                if (conn.State == ConnectionState.Closed)//如果链接状态为关闭，则打开链接
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(strSql, conn);
                    result = comm.ExecuteNonQuery();
                }
                return result;
            }
        }

        /// <summary>
        /// 把DataRow直接转换成对应的实体对象Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToModel<T>(this DataRow dr)//我们给DataRow新增了一个扩展方法
        {
            T t = Activator.CreateInstance<T>();
            // 获得此模型的公共属性  
            string tempName = "";
            PropertyInfo[] propertys = t.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;  // 检查DataTable是否包含此列
                if (dr.Table.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter      
                    if (!pi.CanWrite)
                    {
                        continue;
                    }
                    object value = dr[tempName];
                    if (value != DBNull.Value)
                    {
                        pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }
    }
}