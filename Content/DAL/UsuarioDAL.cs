using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SistemaCotizaciones.Models;

namespace SistemaCotizaciones.DAL
{
    public class UsuarioDAL
    {
        public bool RegistrarUsuario(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    // Verificar si ya existe
                    string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @Usuario";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Usuario", usuario.NombreUsuario);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0) return false; // Usuario ya existe

                    string insertQuery = @"INSERT INTO Usuarios (Usuario, Contraseña) 
                                         VALUES (@Usuario, @Contraseña)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Usuario", usuario.NombreUsuario);
                    insertCmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public Usuario ValidarLogin(string nombreUsuario, string contraseña)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT * FROM Usuarios 
                           WHERE Usuario = @Usuario AND Contraseña = @Contraseña AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Usuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseña);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Usuario usuario = new Usuario
                        {
                            ID = (int)reader["ID"],
                            NombreUsuario = reader["Usuario"].ToString(),
                            Contraseña = reader["Contraseña"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            UltimaActividad = (DateTime)reader["UltimaActividad"],
                            Estado = (bool)reader["Estado"]
                        };

                        reader.Close();

                        // Actualizar última actividad
                        string updateQuery = "UPDATE Usuarios SET UltimaActividad = GETDATE() WHERE Usuario = @Usuario";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@Usuario", nombreUsuario);
                        updateCmd.ExecuteNonQuery();

                        return usuario;
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public Usuario BuscarUsuarioPorNombre(string nombreUsuario)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Usuarios WHERE Usuario = @Usuario AND Estado = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Usuario", nombreUsuario);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            ID = (int)reader["ID"],
                            NombreUsuario = reader["Usuario"].ToString(),
                            Contraseña = reader["Contraseña"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            UltimaActividad = (DateTime)reader["UltimaActividad"],
                            Estado = (bool)reader["Estado"]
                        };
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool EditarUsuario(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE Usuarios SET 
                                   Contraseña = @Contraseña,
                                   UltimaActividad = GETDATE()
                                   WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                    cmd.Parameters.AddWithValue("@ID", usuario.ID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EliminarUsuario(int usuarioId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Usuarios SET Estado = 0 WHERE ID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", usuarioId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM Usuarios WHERE Estado = 1 ORDER BY Usuario";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            ID = (int)reader["ID"],
                            NombreUsuario = reader["Usuario"].ToString(),
                            Contraseña = reader["Contraseña"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            UltimaActividad = (DateTime)reader["UltimaActividad"],
                            Estado = (bool)reader["Estado"]
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return usuarios;
        }

        public bool CambiarContraseña(string nombreUsuario, string nuevaContraseña)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE Usuarios SET 
                                   Contraseña = @NuevaContraseña,
                                   UltimaActividad = GETDATE()
                                   WHERE Usuario = @Usuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NuevaContraseña", nuevaContraseña);
                    cmd.Parameters.AddWithValue("@Usuario", nombreUsuario);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Usuario> ObtenerUsuariosInactivos(int meses = 12)
        {
            List<Usuario> usuariosInactivos = new List<Usuario>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT * FROM Usuarios 
                                   WHERE Estado = 1 
                                   AND UltimaActividad < DATEADD(MONTH, -@Meses, GETDATE())
                                   ORDER BY UltimaActividad";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Meses", meses);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        usuariosInactivos.Add(new Usuario
                        {
                            ID = (int)reader["ID"],
                            NombreUsuario = reader["Usuario"].ToString(),
                            Contraseña = reader["Contraseña"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            UltimaActividad = (DateTime)reader["UltimaActividad"],
                            Estado = (bool)reader["Estado"]
                        });
                    }
                }
            }
            catch
            {
                // Log error
            }
            return usuariosInactivos;
        }
    }
}