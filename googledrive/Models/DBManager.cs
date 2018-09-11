﻿using System;
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

        public static List<FolderDTO> getFoldersList(int Id, int UserId)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("update Folders SET ViewCount = ViewCount + 1 where Id = @Id");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Id
                };
                command.Parameters.Add(param);
                command.ExecuteNonQuery();

                query = string.Format(@"select * from Folders where ParentFolderId = @parent AND CreatedBy = @UserId");
                command = new SqlCommand(query, conn);
                param = new SqlParameter
                {
                    ParameterName = "parent",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Id
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "UserId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = UserId
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
                    folder.ViewCount = reader.GetInt32(6);
                    folders.Add(folder);
                }
                return folders;
            }
        }

        public static List<FolderDTO> searchFolders(string search, int UserId)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Folders where Name like '%' + @Search + '%' AND CreatedBy = @UserId");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Search",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = search
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "UserId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = UserId
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
                    folder.ViewCount = reader.GetInt32(6);
                    folders.Add(folder);
                }
                return folders;
            }
        }

        public static List<FileDTO> searchFiles(string search, int UserId)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Files where Name like '%' + @Search + '%' AND CreatedBy = @UserId");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Search",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = search
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "UserId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = UserId
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

        public static List<FileDTO> getFilesList(int Id, int UserId)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Files where ParentFolderId = @parent AND CreatedBy = @UserId");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "parent",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Id
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "UserId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = UserId
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
                string query = string.Format("update Folders SET ViewCount = ViewCount - 1 where Id = @Id");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.ParentFolderId
                };
                command.Parameters.Add(param);
                command.ExecuteNonQuery();

                query = string.Format(@"select count(*) from Folders where Name = @Name AND ParentFolderId = @ParentFolderId AND CreatedBy = @CreatedBy");
                command = new SqlCommand(query, conn);
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
                    ParameterName = "CreatedBy",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.CreatedBy
                };
                command.Parameters.Add(param);

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if(reader.GetInt32(0) > 0)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                if (dto.Id > 0)
                {
                    query = string.Format(@"update Folders set Name = @Name where Id = @Id");
                }
                else
                {
                    query = string.Format(@"insert into Folders(Name,ParentFolderId,CreatedBy,CreatedOn) values(@Name,@ParentFolderId,@CreatedBy,@CreatedOn)");
                }
                command = new SqlCommand(query, conn);

                param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.Id
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
                    ParameterName = "CreatedBy",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.CreatedBy
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

        public static int registerUser(UserDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select count(*) from Users where Login = @Login");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Login
                };
                command.Parameters.Add(param);

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) > 0)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                query = string.Format(@"insert into Users(Name,Login,Email,Password) values(@Name,@Login,@Email,@Password)");
                command = new SqlCommand(query, conn);

                param = new SqlParameter
                {
                    ParameterName = "Name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Name
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Login
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Email",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Email
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Password",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Password
                };
                command.Parameters.Add(param);

                int result = command.ExecuteNonQuery();

                return result;
            }
        }

        public static int deleteFolder(FolderDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"delete from Folders where Id = @Id OR ParentFolderId = @Id; delete from Files where ParentFolderId = @Id");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.Id
                };
                command.Parameters.Add(param);

                int result = command.ExecuteNonQuery();

                return result;
            }
        }

        public static int deleteFile(FileDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"delete from Files where Id = @Id");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.Id
                };
                command.Parameters.Add(param);

                int result = command.ExecuteNonQuery();

                if(result > 0)
                {
                    query = string.Format(@"select * from Files where Id = @Id");
                    command = new SqlCommand(query, conn);
                    param = new SqlParameter
                    {
                        ParameterName = "Id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = dto.Id
                    };
                    command.Parameters.Add(param);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dto.UniqueName = reader.GetString(1);
                        dto.FileExt = reader.GetString(4);
                    }

                    string path = HttpContext.Current.Server.MapPath("~/Uploads/" + dto.UniqueName + dto.FileExt);

                    if ((System.IO.File.Exists(path)))
                    {
                        System.IO.File.Delete(path);
                    }
                    reader.Close();
                }
                return result;
            }
        }

        public static int editFile(FileDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"update Files set Name = @Name where Id = @Id");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = dto.Id
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Name
                };

                command.Parameters.Add(param);

                int result = command.ExecuteNonQuery();

                return result;
            }
        }

        public static FileDTO downloadFile(int id)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select * from Files where Id = @id");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "Id";
                param.SqlDbType = System.Data.SqlDbType.VarChar;
                param.Value = id;
                command.Parameters.Add(param);

                SqlDataReader reader = command.ExecuteReader();
                FileDTO dto = new FileDTO();
                if (reader.Read())
                {
                    dto.UniqueName = reader.GetString(1);
                    dto.Name = reader.GetString(2);
                    dto.FileExt = reader.GetString(4);
                }
                return dto;
            }
        }

        public static int uploadFiles(List<FileDTO> dtos)
        {
            int result = 0;
            int exist = 0;
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                foreach (var dto in dtos)
                {
                    result++;
                    string query = string.Format(@"select count(*) from Files where Name = @Name AND ParentFolderId = @ParentFolderId AND FileExt = @FileExt AND FileSizeInKB = @FileSizeInKB AND CreatedBy = @CreatedBy");
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
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetInt32(0) > 0)
                    {
                        exist++;
                        reader.Close();
                        continue;
                    }
                    reader.Close();
                    query = string.Format(@"insert into Files(UniqueName,Name,ParentFolderId,FileExt,FileSizeInKB,CreatedBy,UploadedOn) values(@UniqueName,@Name,@ParentFolderId,@FileExt,@FileSizeInKB,@CreatedBy,@UploadedOn)");
                    command = new SqlCommand(query, conn);

                    param = new SqlParameter
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
                }
                return result - exist;
            }
        }
    }
}