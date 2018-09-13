using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                string query = string.Format(@"select Name from Users where Login = @Login AND Password = @Password AND Token != 'NULL'");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Login
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Password",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Password
                };
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return "$";
                }
                reader.Close();

                query = string.Format(@"select Name from Users where Login = @Login AND Password = @Password AND Token IS NULL");
                command = new SqlCommand(query, conn);

                param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Login
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Password",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Password
                };
                command.Parameters.Add(param);

                reader = command.ExecuteReader();
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

        public static int forgetPassword(UserDTO user)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select Id,Email from Users where Login = @Login");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = user.Login
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) == 0)
                {
                    reader.Close();
                    return 0;
                }
                user.Email = reader.GetString(1);
                reader.Close();

                string token = Guid.NewGuid().ToString();
                
                string url = "http://" + HttpContext.Current.Request.Url.Authority + "/User/ResetPassword/" + token;
                try
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("m.mianjazibali@gmail.com", "3g9&2C67rupp6TE@0T6e")
                    };

                    using (var message = new MailMessage("m.mianjazibali@gmail.com", user.Email))
                    {
                        message.Subject = "Reset Password";
                        message.Body = url;
                        smtp.Send(message);
                    }
                }
                catch (Exception)
                {
                    return -1;
                }
                
                query = string.Format("update Users set TokenPassword = @Token where Login = @Login");
                command = new SqlCommand(query, conn);
                param = new SqlParameter
                {
                    ParameterName = "Token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = token
                };
                command.Parameters.Add(param);
                param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = user.Login
                };
                command.Parameters.Add(param);
                int result = command.ExecuteNonQuery();
                return result;
            }
        }

        public static int resetPassword(string token)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("select Count(*) from Users where TokenPassword = @token");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = token
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(0);
                return result;
            }
        }

        public static string generateFileToken(int id)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("select Token from Shared where FileId = @Id AND OwnerLogin IS NULL");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = id
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                string token = "";
                if (reader.Read())
                {
                    token = reader.GetString(0);
                }
                reader.Close();
                if(token == "")
                {
                    token = Guid.NewGuid().ToString();
                    query = string.Format("insert into Shared(Token,FileId) values (@Token,@Id)");
                    command = new SqlCommand(query, conn);
                    param = new SqlParameter
                    {
                        ParameterName = "Token",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Value = token
                    };
                    command.Parameters.Add(param);
                    param = new SqlParameter
                    {
                        ParameterName = "Id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = id
                    };
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                }
                query = string.Format("delete from Shared where FileId = @Id AND OwnerLogin IS NOT NULL");
                command = new SqlCommand(query, conn);
                param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = id
                };
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
                return token;
            }
        }

        public static string generateSpecificFileToken(int Id, string Login)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("select Email from Users where Login = @Login");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Login
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                string Email = "";
                if (reader.Read())
                {
                    Email = reader.GetString(0);
                }
                reader.Close();
                if (Email != "")
                {
                    query = string.Format("select Count(*) from Files where Id = @Id");
                    command = new SqlCommand(query, conn);
                    param = new SqlParameter
                    {
                        ParameterName = "Id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = Id
                    };
                    command.Parameters.Add(param);
                    reader = command.ExecuteReader();
                    reader.Read(); 
                    if(reader.GetInt32(0) > 0)
                    {
                        reader.Close();
                        query = string.Format("select count(*) from Shared where FileId = @Id AND OwnerLogin = @Login");
                        command = new SqlCommand(query, conn);
                        param = new SqlParameter
                        {
                            ParameterName = "Id",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = Id
                        };
                        command.Parameters.Add(param);
                        param = new SqlParameter
                        {
                            ParameterName = "Login",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = Login
                        };
                        command.Parameters.Add(param);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            int found = reader.GetInt32(0);
                            if(found > 0)
                            {
                                return "UserAlreadyExist";
                            }
                        }
                        reader.Close();
                        string token = Guid.NewGuid().ToString();
                        query = string.Format("insert into Shared(Token,OwnerLogin,FileId) values(@Token,@Login,@Id)");
                        command = new SqlCommand(query, conn);
                        param = new SqlParameter
                        {
                            ParameterName = "Token",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = token
                        };
                        command.Parameters.Add(param);
                        param = new SqlParameter
                        {
                            ParameterName = "Login",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = Login
                        };
                        command.Parameters.Add(param);
                        param = new SqlParameter
                        {
                            ParameterName = "Id",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = Id
                        };
                        command.Parameters.Add(param);
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            query = string.Format("delete from Shared where FileId = @Id AND OwnerLogin IS NULL");
                            command = new SqlCommand(query, conn);
                            param = new SqlParameter
                            {
                                ParameterName = "Id",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Value = Id
                            };
                            command.Parameters.Add(param);
                            command.ExecuteNonQuery();
                        }
                        /*
                        string url = "http://" + HttpContext.Current.Request.Url.Authority + "/Download/File/" + token;
                        try
                        {
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential("m.mianjazibali@gmail.com", "3g9&2C67rupp6TE@0T6e");

                            using (var message = new MailMessage("m.mianjazibali@gmail.com", Email))
                            {
                                message.Subject = "File Sharing";
                                message.Body = url;
                                smtp.Send(message);
                            }
                        }
                        catch (Exception)
                        {
                            return "Error";
                        }
                        */
                        return token;
                    }
                    else
                    {
                        return "FileNotFound";
                    }
                }
                else
                {
                    return "UserNotFound";
                }
            }
        }

        public static FileDTO getFile(string token, string login)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"select FileId from Shared where Token = @Token AND OwnerLogin IS NULL OR OwnerLogin = @Login");
                SqlCommand command = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = token
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = login
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                int Fileid = 0;
                if (reader.Read())
                {
                    Fileid = reader.GetInt32(0);
                }
                reader.Close();
                FileDTO dto = new FileDTO();
                if ( Fileid == 0)
                {
                    return dto;
                }
                query = string.Format(@"select * from Files where Id = @Id");
                command = new SqlCommand(query, conn);

                param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = Fileid
                };
                command.Parameters.Add(param);

                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    dto.UniqueName = reader.GetString(1);
                    dto.Name = reader.GetString(2);
                    dto.FileExt = reader.GetString(4);
                    dto.FileSizeInKB = reader.GetInt32(5); 
                }
                return dto;
            }
        } 

        public static int resetPassword(UserDTO dto)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("update Users set Password = @Password, TokenPassword = 'NULL' where TokenPassword = @Token");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Password",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Password
                };
                command.Parameters.Add(param);
                param = new SqlParameter
                {
                    ParameterName = "Token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.TokenPassword
                };
                command.Parameters.Add(param);
                int result = command.ExecuteNonQuery();
                return result;
            }
        }

        public static int verifyUser(string token)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("update Users set Token = 'NULL' where Token = @token");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = token
                };
                command.Parameters.Add(param);
                int result = command.ExecuteNonQuery();
                return result;
            }
        }

        public static List<UserDTO> getFileUsers(int id)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format("select OwnerLogin from Shared where FileId = @Id");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = id
                };
                command.Parameters.Add(param);
                SqlDataReader reader = command.ExecuteReader();
                List<UserDTO> users = new List<UserDTO>();
                string Login = "";
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        Login = reader.GetString(0);
                        UserDTO user = new UserDTO
                        {
                            Login = Login
                        };
                        users.Add(user);
                    } 
                }
                return users;
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
                    FolderDTO folder = new FolderDTO
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ParentFolderId = reader.GetInt32(2),
                        CreatedBy = reader.GetInt32(3),
                        CreatedOn = reader.GetDateTime(4).ToString("dd/MM/yyyy hh:mm tt"),
                        ViewCount = reader.GetInt32(6)
                    };
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
                    FolderDTO folder = new FolderDTO
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ParentFolderId = reader.GetInt32(2),
                        CreatedBy = reader.GetInt32(3),
                        CreatedOn = reader.GetDateTime(4).ToString("dd/MM/yyyy hh:mm tt"),
                        ViewCount = reader.GetInt32(6)
                    };
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
                string query = string.Format(@"select * from Files FULL OUTER JOIN Shared ON Files.Id = Shared.FileId where Name like '%' + @Search + '%' AND CreatedBy = @UserId");
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
                    if (!reader.IsDBNull(10))
                    {
                        file.Token = reader.GetString(10);
                    }
                    if (!reader.IsDBNull(11))
                    {
                        file.Share = reader.GetString(11);
                    }
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
                string query = string.Format(@"select * from Files FULL OUTER JOIN Shared ON Files.Id = Shared.FileId where ParentFolderId = @parent AND CreatedBy = @UserId");
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
                    if (!reader.IsDBNull(10))
                    {
                        file.Token = reader.GetString(10);
                    }
                    if (!reader.IsDBNull(11))
                    {
                        file.Share = reader.GetString(11);
                    }
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

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = login
                };
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
                dto.Token = Guid.NewGuid().ToString();
                
                string url = "http://" + HttpContext.Current.Request.Url.Authority + "/User/Verify/" + dto.Token;
                try
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("m.mianjazibali@gmail.com", "3g9&2C67rupp6TE@0T6e")
                    };

                    using (var message = new MailMessage("m.mianjazibali@gmail.com",dto.Email))
                    {
                        message.Subject = "User Verification";
                        message.Body = url; 
                        smtp.Send(message);
                    }
                }
                catch (Exception)
                {
                    return -1;
                }
                
                query = string.Format(@"insert into Users(Name,Login,Email,Password,Token,CreatedOn) values(@Name,@Login,@Email,@Password,@Token,@CreatedOn)");
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

                param = new SqlParameter
                {
                    ParameterName = "Token",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = dto.Token
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "CreatedOn",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = DateTime.Now
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

        public static int deleteSharedUser(int Id, string Login)
        {
            string connString = @"Data Source=localhost;Initial Catalog=GoogleDrive;User ID=sa;Password=123";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = string.Format(@"delete from Shared where FileId = @Id AND OwnerLogin = @Login");
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = Id
                };
                command.Parameters.Add(param);

                param = new SqlParameter
                {
                    ParameterName = "Login",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = Login
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

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "Id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = id
                };
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