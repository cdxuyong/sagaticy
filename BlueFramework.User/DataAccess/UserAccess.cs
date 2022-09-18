using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using BlueFramework.User.Models;
using BlueFramework.Data;

namespace BlueFramework.User.DataAccess
{
    public class UserAccess
    {
        public UserInfo GetUserInfo(int userId)
        {
            //DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            //Database database = dbFactory.CreateDefault();
            //string sql = "select * from t_s_user  where userid=@userid";
            //DbCommand dbCommand = database.GetSqlStringCommand(sql);
            //database.AddInParameter(dbCommand, "userid", DbType.Int32, userId);
            //DataSet dataSet = database.ExecuteDataSet(dbCommand);
            Models.UserInfo userInfo = new Models.UserInfo();
            userInfo.UserId = 1;
            userInfo.UserName = "admin";
            return userInfo;

        }

        public UserInfo GetUser(UserInfo user)
        {
            return null;
        }

        public List<UserInfo> GetUserList()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select t.*,p.ORG_NAME from T_S_USER t,T_S_ORGANIZATION p where t.username<>'admin' and t.ORG_ID=p.ORG_ID";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            List<UserInfo> users = new List<UserInfo>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UserInfo useri = new UserInfo();
                    useri.UserId = int.Parse(row["USERID"].ToString());
                    useri.UserName = row["USERNAME"].ToString();
                    useri.TrueName = row["TRUENAME"].ToString();
                    useri.OrgName = row["ORG_NAME"].ToString();
                    users.Add(useri);
                }
            }
            return users;
        }

        public bool AddAccount(UserInfo user)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            user.UserId = getMaxUserId() + 1;
            string sql = string.Format("insert into T_S_USER(USERID,ORG_ID,USERNAME,PASSWORD,TRUENAME,ISADMIN,STATE,CREATE_TIME)"
         + "values({0},{1},'{2}','{3}','{4}',{5},{6},'{7}')", user.UserId, user.OrgId, user.UserName, user.Password, user.TrueName, 0, 1, DateTime.Now.ToString("yyyy/MM/dd"));
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            int result = database.ExecuteNonQuery(dbCommand);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(UserInfo user)
        {
            Database database = new DatabaseProviderFactory().CreateDefault();
            try
            {
                using (DbConnection dbConnection = database.CreateConnection())
                {
                    dbConnection.Open();
                    using (DbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            string sql1 = "delete from T_S_USER where userid=" + user.UserId;
                            DbCommand dbCommand1 = database.GetSqlStringCommand(sql1);
                            database.ExecuteNonQuery(dbCommand1, dbTransaction);

                            string sql2 = "delete from T_S_USERROLE where userid=" + user.UserId;
                            DbCommand dbCommand2 = database.GetSqlStringCommand(sql2);
                            database.ExecuteNonQuery(dbCommand2, dbTransaction);
                            dbTransaction.Commit();
                        }
                        catch (Exception exception)
                        {
                            dbTransaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private int getMaxUserId()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select max(t.userid) id from t_s_user t";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            int maxId = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                maxId = int.Parse(dt.Rows[0]["ID"].ToString());
            }
            else
            {
                maxId = 1;
            }
            return maxId;
        }

        public UserInfo GetUserByName(string userName)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from t_s_user t  where username=@userName";
            DataSet dataSet = null;
            using (DbCommand dbCommand = database.GetSqlStringCommand(sql))
            {
                database.AddInParameter(dbCommand, "userName", DbType.String, userName);
                dataSet = database.ExecuteDataSet(dbCommand);
            }
            DataTable dt = dataSet.Tables[0];
            UserInfo user = new UserInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user.UserId = int.Parse(row["USERID"].ToString());
                user.UserName = row["USERNAME"].ToString();
                user.Password = row["PASSWORD"].ToString();
                user.OrgId = int.Parse(row["ORG_ID"].ToString());
                user.TrueName = row["TRUENAME"].ToString();
                user.CreateTime = DateTime.Parse(row["CREATE_TIME"].ToString()).ToShortDateString();
                user.IsAdmin = (row["IsAdmin"].ToString() == "1") ? true : false;
            }
            return user;
        }

        public UserInfo QueryUserById(int userId)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from t_s_user t  where userid=@userId";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            database.AddInParameter(dbCommand, "userId", DbType.Int32, userId);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            UserInfo user = new UserInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user.UserId = int.Parse(row["USERID"].ToString());
                user.UserName = row["USERNAME"].ToString();
                user.Password = row["PASSWORD"].ToString();
                user.OrgId = int.Parse(row["ORG_ID"].ToString());
                user.TrueName = row["TRUENAME"].ToString();
                user.CreateTime = DateTime.Parse(row["CREATE_TIME"].ToString()).ToShortDateString();
                user.IsAdmin = (row["IsAdmin"].ToString() == "1") ? true : false;
            }
            return user;
        }

        public UserInfo GetUser(string userName)
        {
            UserInfo user = GetUserByName(userName);
            string sql = @"select r.NAME,r.ROLEID from T_S_USERROLE t 
            inner join T_S_ROLE r on r.ROLEID=t.ROLEID
            where t.USERID="+user.UserId;
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            DataTable dataTable = database.ExecuteDataSet(CommandType.Text, sql).Tables[0];

            user.Roles = new List<RoleInfo>();
            foreach(DataRow row in dataTable.Rows)
            {
                RoleInfo role = new RoleInfo()
                {
                    RoleId = int.Parse(row["ROLEID"].ToString()),
                    RoleName = row["NAME"].ToString()
                };
                user.Roles.Add(role);
            }

            sql = @"select DISTINCT m.MENUID
            from T_S_USERROLE t 
            inner join T_S_ROLE r on r.ROLEID=t.ROLEID
            inner join T_S_MENURIGHT m on m.ROLEID=t.ROLEID
            where t.USERID="+user.UserId;
            dataTable = database.ExecuteDataSet(CommandType.Text, sql).Tables[0];
            user.MenuRights = new List<int>();
            foreach (DataRow row in dataTable.Rows)
            {
                int menuId = int.Parse(row["MENUID"].ToString());
                user.MenuRights.Add(menuId);
            }

            return user;
        }

        public bool InitPwd(int userID, string pwd)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "update T_S_USER set {0} where userid={1}";
            string column = @"password='" + pwd + "'";
            sql = string.Format(sql, column, userID);
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            int result = database.ExecuteNonQuery(dbCommand);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int UpdateUser(UserInfo user)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string now = DateTime.Now.ToString("yyyy/MM/dd");
            string sql = "update T_S_USER set {0} where userid={1}";
            string column = @" username='" + user.UserName + "',truename='" + user.TrueName + "',org_id=" + user.OrgId + ",update_time= '" + now + "'";
            sql = string.Format(sql, column, user.UserId);
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            int result = database.ExecuteNonQuery(dbCommand);
            return result;
        }

        public List<UserInfo> GetUsers(UserInfo user)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = @"SELECT T.*, A.ORG_NAME
                          FROM T_S_USER T
                           LEFT JOIN T_S_ORGANIZATION A
                           ON T.ORG_ID = A.ORG_ID
                           WHERE T.USERNAME <> 'ADMIN' ";
            string whereStr = "";
            if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.TrueName))
            {
                whereStr += "AND ( T.USERNAME LIKE '%" + user.UserName + "%' OR T.TRUENAME LIKE '%" + user.TrueName + "%') ";
            }
            whereStr += " order by userid";
            sql += whereStr;
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            //database.AddInParameter(dbCommand, "userName", DbType.String, user.TrueName);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            List<UserInfo> users = new List<UserInfo>();
            foreach (DataRow row in dt.Rows)
            {
                UserInfo ui = new UserInfo();
                ui.UserId = int.Parse(row["USERID"].ToString());
                ui.UserName = row["UserName"].ToString();
                ui.TrueName = row["TrueName"].ToString();
                ui.CreateTime = DateTime.Parse(row["CREATE_TIME"].ToString()).ToShortDateString();
                ui.OrgId = int.Parse(row["ORG_ID"].ToString());
                ui.OrgName = row["ORG_NAME"].ToString();
                users.Add(ui);
            }
            return users;
        }

        /// <summary>
        /// get orgniations
        /// </summary>
        /// <returns></returns>
        public List<OrgnizationInfo> GetOrgnizations()
        {
            DataTable dataTable = null;
            try
            {
                DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
                Database database = dbFactory.CreateDefault();
                string sql = "select * from T_S_ORGANIZATION";
                DataSet dataSet = database.ExecuteDataSet(CommandType.Text, sql);
                dataTable = dataSet.Tables[0];
            }
            catch
            {
                return null;
            }
            List<OrgnizationInfo> orgnizations = new List<OrgnizationInfo>();
            foreach (DataRow dr in dataTable.Rows)
            {
                OrgnizationInfo orgnization = new OrgnizationInfo()
                {
                    OrgId = int.Parse(dr["ORG_ID"].ToString()),
                    ParentId = int.Parse(dr["PARENT_ID"].ToString()),
                    OrgName = dr["ORG_NAME"].ToString(),
                    OrgCode = dr["ORG_CODE"].ToString()
                };
                orgnizations.Add(orgnization);
            }
            return orgnizations;

        }
    }
}
