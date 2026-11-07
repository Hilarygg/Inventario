using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Proveedor : ConsultaBase
{
    public int pro_id { get; set; }
    public string pro_nombre { get; set; } = "";
    public string pro_telefono  { get; set; } = "";
    public string pro_correo { get; set; } = "";
    public Activo? pro_activo { get; set; }


    public List<Proveedor> MostrarProveedores()
    {
        string query = @"SELECT pro_id, pro_nombre FROM Proveedor";

        SqlDataReader reader = null!;
        List<Proveedor> lista = new List<Proveedor>();

        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Proveedor proveedor = new Proveedor()
            {
                pro_id = reader.GetInt32(0),
                pro_nombre = reader.GetString(1)
            };
            lista.Add(proveedor);
        }
        return lista;
    }

    public List<Proveedor> MostrarProveedoresAdmin()
    {
        string query = @"SELECT p.pro_id, p.pro_nombre, p.pro_telefono, p.pro_correo, a.act_id, a.act_nombre
                        FROM Proveedor p
                        INNER JOIN Activo a ON p.pro_activo = a.act_id";

        SqlDataReader reader = null!;
        List<Proveedor> lista = new List<Proveedor>();

        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Proveedor proveedor = new Proveedor()
            {
                pro_id = reader.GetInt32(0),
                pro_nombre = reader.GetString(1),
                pro_telefono = reader.GetString(2),
                pro_correo = reader.GetString(3),
                pro_activo = new Activo()
                {
                    act_id = reader.GetInt32(4),
                    act_nombre = reader.GetString(5)
                }
            };
            lista.Add(proveedor);
        }
        return lista;
    }

    public bool InsertProveedor()
    {
        string query = @"INSERT INTO Proveedor (pro_nombre, pro_telefono, pro_correo, pro_activo) 
                        VALUES (@pro_nombre, @pro_telefono, @pro_correo, @pro_activo)";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@pro_nombre", pro_nombre },
            { "@pro_telefono", pro_telefono },
            { "@pro_correo", pro_correo },
            { "@pro_activo", pro_activo?.act_id ?? 1 }
        };

        return ExecutedCommand(query, parameters);
    }

    public Proveedor ObtenerProveedor(int pro_id)
    {
        string query = @"SELECT p.pro_id, p.pro_nombre, p.pro_telefono, p.pro_correo, a.act_id, a.act_nombre
                        FROM Proveedor p
                        INNER JOIN Activo a ON p.pro_activo = a.act_id
                        WHERE p.pro_id = @pro_id";

        SqlDataReader reader = null!;
        Proveedor proveedor = new Proveedor();

        Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
        {
            { "@pro_id", pro_id }
        };

        ConsultaDB(ref reader, query, parameters);

        if (reader.Read())
        {
            proveedor.pro_id = reader.GetInt32(0);
            proveedor.pro_nombre = reader.GetString(1);
            proveedor.pro_telefono = reader.GetString(2);
            proveedor.pro_correo = reader.GetString(3);
            proveedor.pro_activo = new Activo()
            {
                act_id = reader.GetInt32(4),
                act_nombre = reader.GetString(5)
            };
        }
        return proveedor;
    }

    public bool ActualizarProveedor()
    {
        string query = @"UPDATE Proveedor 
                        SET pro_nombre = @pro_nombre, pro_telefono = @pro_telefono, pro_correo = @pro_correo, pro_activo = @pro_activo
                        WHERE pro_id = @pro_id";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@pro_id", pro_id },
            { "@pro_nombre", pro_nombre },
            { "@pro_telefono", pro_telefono },
            { "@pro_correo", pro_correo },
            { "@pro_activo", pro_activo?.act_id ?? 1 }
        };

        return ExecutedCommand(query, parameters);
    }

    public bool EliminarProveedor(int pro_id)
    {
        string query = @"DELETE FROM Proveedor WHERE pro_id = @pro_id";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@pro_id", pro_id }
        };

        return ExecutedCommand(query, parameters);
    }

    public bool ValidarProveedorInactivo(int pro_id)
    {
        string query = @"SELECT COUNT(*)
                        FROM Proveedor p
                        WHERE P.pro_id = @Id
                        AND p.pro_activo = (SELECT act_id FROM Activo WHERE act_nombre = 'Inactivo')";
                        
        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Id", pro_id }
        };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        bool validar = reader.Read() && reader.GetInt32(0) > 0;
        reader.Close();

        return validar;
    }

    public bool ValidarProveedorProductos(int pro_id)
    {
        string query = @"SELECT COUNT(*)
                        FROM Producto p
                        WHERE p.prod_proveedor = @Id";

        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Id", pro_id }
        };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        bool validar = reader.Read() && reader.GetInt32(0) == 0;
        reader.Close();

        return validar;
    }
}