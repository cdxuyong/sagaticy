using BlueFramework.Data;
using BlueFramework.User.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.DataAccess
{
    public class SysAccess
    {
        public int[] GetMenuRights(int roleId)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from T_S_MENURIGHT t where t.roleid=" + roleId;
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                int[] str = new int[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    str[i] = int.Parse(dt.Rows[i]["MENUID"].ToString());
                }
                return str;
            }
            else
                return null;
            
        }

        public int[] GetDataRights(int roleId)
        {
            return null;
        }

        public RoleInfo GetRoleByRoleId(int roleId)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from T_S_ROLE t where t.roleid=" + roleId;
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            RoleInfo role = new RoleInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                role.RoleId = int.Parse(dt.Rows[0]["ROLEID"].ToString());
                role.RoleName = dt.Rows[0]["NAME"].ToString();
                role.Description = dt.Rows[0]["DESCRIPTION"].ToString();
            }
            return role;
        }

        public RoleInfo GetRoleByRoleName(string roleName)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from T_S_ROLE t where t.name='" + roleName + "'";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            RoleInfo role = new RoleInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                role.RoleId = int.Parse(dt.Rows[0]["ROLEID"].ToString());
                role.RoleName = dt.Rows[0]["NAME"].ToString();
                role.Description = dt.Rows[0]["DESCRIPTION"].ToString();
            }
            return role;
        }

        public List<RoleInfo> GetRoles(RoleInfo roleinfo)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from T_S_ROLE t where 1=1 ";
            string whereStr = "";
            if (!string.IsNullOrEmpty(roleinfo.RoleName))
            {
                whereStr += " and t.name like'%" + roleinfo.RoleName + "%'";
            }
            whereStr += " order by roleid";
            sql += whereStr;
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            List<RoleInfo> roles = new List<RoleInfo>();
            foreach (DataRow row in dt.Rows)
            {
                RoleInfo role = new RoleInfo();
                role.RoleId = int.Parse(row["ROLEID"].ToString());
                role.RoleName = row["NAME"].ToString();
                role.Description = row["DESCRIPTION"].ToString();
                roles.Add(role);
            }
            return roles;
        }

        public bool SaveMenuRights(int roleId, int[] item)
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
                            string sql1 = "delete from T_S_MENURIGHT where ROLEID = '" + roleId + "'";
                            DbCommand dbCommand1 = database.GetSqlStringCommand(sql1);
                            database.ExecuteNonQuery(dbCommand1, dbTransaction);

                            if (item != null)
                            {
                                string[] menu = item.Select(i => i.ToString()).ToArray();
                                int Id = GetMaxMenuRightId() + 1;
                                for (int i = 0; i < menu.Length; i++)
                                {
                                    string clown = menu[i];
                                    string sql = "insert into T_S_MENURIGHT values(" + Id + "," + roleId + "," + clown + ")";
                                    DbCommand dbCommand2 = database.GetSqlStringCommand(sql);
                                    database.ExecuteNonQuery(dbCommand2, dbTransaction);
                                    Id++;
                                }
                            }
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

        public int GetMaxMenuRightId()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select max(ID) ID from T_S_MENURIGHT";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            int maxId = 0;
            if (dt != null && !string.IsNullOrEmpty(dt.Rows[0]["ID"].ToString()))
            {
                maxId = int.Parse(dt.Rows[0]["ID"].ToString());
            }
            return maxId;
        }

        public int AddRole(RoleInfo role)
        {
            return 0;
        }

        public int AddOnlyRole(RoleInfo role)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            int roleId = GetMaxRoleId() + 1;
            role.RoleId = roleId;
            string sql = "insert into T_S_ROLE(roleid,name,description) values('{0}','{1}','{2}')";
            sql = string.Format(sql, role.RoleId, role.RoleName, role.Description);
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            int result = database.ExecuteNonQuery(dbCommand);
            return result;
        }

        public int GetMaxRoleId()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select max(t.ROLEID) id from t_s_role t";
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            int maxId = 0;
            if (dt != null && !string.IsNullOrEmpty(dt.Rows[0]["ID"].ToString()))
            {
                maxId = int.Parse(dt.Rows[0]["ID"].ToString());
            }
            return maxId;
        }

        public bool DeleteRole(RoleInfo role)
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
                            string sql1 = "delete from T_S_MENURIGHT where roleId=" + role.RoleId;
                            DbCommand dbCommand1 = database.GetSqlStringCommand(sql1);
                            database.ExecuteNonQuery(dbCommand1, dbTransaction);

                            string sql2 = "delete from T_S_DATARIGHT where roleId=" + role.RoleId;
                            DbCommand dbCommand2 = database.GetSqlStringCommand(sql2);
                            database.ExecuteNonQuery(dbCommand2, dbTransaction);

                            string sql3 = "delete from T_S_USERROLE where roleId=" + role.RoleId;
                            DbCommand dbCommand3 = database.GetSqlStringCommand(sql3);
                            database.ExecuteNonQuery(dbCommand3, dbTransaction);

                            string sql4 = "delete from T_S_ROLE where roleId=" + role.RoleId;
                            DbCommand dbCommand4 = database.GetSqlStringCommand(sql4);
                            database.ExecuteNonQuery(dbCommand4, dbTransaction);

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

        public int[] GetGrouping(int roleId)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "select * from T_S_USERROLE t where t.roleid=" + roleId;
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            DataSet dataSet = database.ExecuteDataSet(dbCommand);
            DataTable dt = dataSet.Tables[0];
            int[] str = new int[dt.Rows.Count];
            if (dt != null || dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    str[i] = int.Parse(dt.Rows[i]["USERID"].ToString());
                }
            }
            return str;
        }

        public int UpdateOnlyRole(RoleInfo role)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database database = dbFactory.CreateDefault();
            string sql = "update T_S_ROLE set {0} where roleid={1}";
            string column = @" name='" + role.RoleName + "',description='" + role.Description + "'";
            sql = string.Format(sql, column, role.RoleId);
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            int result = database.ExecuteNonQuery(dbCommand);
            return result;
        }

        public bool UpdateRoleUsers(RoleInfo role)
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
                            //删除角色对应的用户
                            string sql1 = "delete from T_S_USERROLE where ROLEID = '" + role.RoleId + "'";
                            DbCommand dbCommand1 = database.GetSqlStringCommand(sql1);
                            database.ExecuteNonQuery(dbCommand1, dbTransaction);
                            //增加角色对应的用户
                            if (role.Users.Count > 0)
                            {
                                string[] group = role.Users.Select(i => i.ToString()).ToArray();

                                for (int i = 0; i < group.Length; i++)
                                {
                                    string clown = group[i];
                                    string sql2 = "insert into T_S_USERROLE values('" + role.RoleId + "','" + clown + "')";
                                    DbCommand dbCommand2 = database.GetSqlStringCommand(sql2);
                                    database.ExecuteNonQuery(dbCommand2, dbTransaction);
                                }
                            }

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
    }
}
