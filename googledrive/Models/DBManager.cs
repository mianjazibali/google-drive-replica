using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GoogleDrive.Models
{
    public static class DBManager
    {
        public static string GetUser(UserDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select Name from Users where Login = @Login AND Password = @Password");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "Login";
                param.SqlDbType = System.Data.SqlDbType.VarChar;
                param.Value = dto.Login;
                command.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "Password";
                param.SqlDbType = System.Data.SqlDbType.VarChar;
                param.Value = dto.Password;
                command.Parameters.Add(param);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
                else
                {
                    return "";
                }
            }
        }

        public static List<FolderDTO> getFoldersList(int Id)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Folders where ParentFolderId = @parent");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "parent",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Id
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                List<FolderDTO> folders = new List<FolderDTO>();
                while (reader.Read())
                {
                    FolderDTO folder = new FolderDTO();
                    folder.Id = reader.GetInt32(0);
                    folder.Name = reader.GetString(1);
                    folder.ParentFolderId = reader.GetInt32(2);
                    folder.CreatedBy = reader.GetInt32(3);
                    folder.CreatedOn = reader.GetDateTime(4).ToString("dd/MM/yyyy hh:mm tt");
                    folders.Add(folder);
                }
                return folders;
            }
        }

        public static List<FileDTO> getFilesList(int Id)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Files where ParentFolderId = @parent");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "parent",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Id
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                List<FileDTO> files = new List<FileDTO>();
                while (reader.Read())
                {
                    FileDTO file = new FileDTO();
                    file.Id = reader.GetInt32(0);
                    file.Name = reader.GetString(2);
                    file.ParentFolderId = reader.GetInt32(3);
                    file.FileExt = reader.GetString(4);
                    file.FileSizeInKB = reader.GetInt32(5);
                    file.CreatedBy = reader.GetInt32(6);
                    file.UploadedOn = reader.GetDateTime(7).ToString("dd/MM/yyyy hh:mm tt");
                    files.Add(file);
                }
                return files;
            }
        }

        public static int getUserIdByLogin(string login)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select Id from Users where Login = @Login");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "Login";
                param.SqlDbType = System.Data.SqlDbType.VarChar;
                param.Value = login;
                command.Parameters.Add(param);

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return reader.GetInt32(0);
            }
        }

        public static int createFolder(FolderDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"insert into Folders(Name,ParentFolderId,CreatedBy,CreatedOn) values(@Name,@ParentFolderId,@CreatedBy,@CreatedOn)");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Name
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "ParentFolderId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.ParentFolderId
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "CreatedBy",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.Id
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "CreatedOn",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Value = System.DateTime.Now
                };
                command.Parameters.Add(param);

                int result = command.ExecuteNonQuery();

                return result;
            }
        }

        public static int uploadFiles(List<FileDTO> dtos)
        {
            int result = 0;
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                foreach (var dto in dtos)
                {
                    string query = string.Format(@"insert into Files(UniqueName,Name,ParentFolderId,FileExt,FileSizeInKB,CreatedBy,UploadedOn) values(@UniqueName,@Name,@ParentFolderId,@FileExt,@FileSizeInKB,@CreatedBy,@UploadedOn)");
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlParameter param = new SqlParameter
                    {
                        ParameterName = "UniqueName",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Value = dto.UniqueName
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "Name",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Value = dto.Name
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "ParentFolderId",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = dto.ParentFolderId
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "FileExt",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Value = dto.FileExt
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "FileSizeInKB",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = dto.FileSizeInKB
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "CreatedBy",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = dto.CreatedBy
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "UploadedOn",
                        SqlDbType = System.Data.SqlDbType.DateTime,
                        Value = System.DateTime.Now
                    };
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    result++;
                }
                return result;
            }
        }
    }
}