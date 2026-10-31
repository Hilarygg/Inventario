using System.Data;
using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class ConsultaBase
{
    private static readonly string _cadena = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build()
        .GetConnectionString("DefaultConnection")!;

    protected void ConsultaDB(ref SqlDataReader reader, string query,
                              Dictionary<string, dynamic>? parametros = null)
    {
        SqlConnection conn = new SqlConnection(_cadena);
        SqlCommand cmd = new SqlCommand(query, conn);

        if (parametros != null)
            foreach (var p in parametros)
                cmd.Parameters.AddWithValue(p.Key, (object?)p.Value ?? DBNull.Value);

        conn.Open();
        reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    public bool ExecutedCommand(string query, Dictionary<string, object?> parametros)
    {
        try
        {
            using SqlConnection conexion = new SqlConnection(_cadena);
            using SqlCommand comando = new SqlCommand(query, conexion);

            foreach (var p in parametros)
                comando.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            conexion.Open();
            return comando.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    
};


