using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Rol : ConsultaBase
{
    public int rol_id { get; set; }
    public string rol_nombre { get; set; } = "";
    public string rol_descripcion { get; set; } = "";
    public Activo? rol_activo { get; set; }

    public List<Rol> MostrarRoles()
    {
        string query = @"SELECT r.rol_id, r.rol_nombre, r.rol_descripcion, a.act_id, a.act_nombre  
                        FROM Rol r
                        INNER JOIN Activo a ON r.rol_activo = a.act_id";

        SqlDataReader reader = null!;
        List<Rol> lista = new List<Rol>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Rol rol = new Rol()
            {
                rol_id = reader.GetInt32(0),
                rol_nombre = reader.GetString(1),
                rol_descripcion = reader.GetString(2),
                rol_activo = new Activo()
                {
                    act_id = reader.GetInt32(3),
                    act_nombre = reader.GetString(4)
                }
            };
            lista.Add(rol);
        } 
        return lista;
    }

    public List<MenuPermisoVM> ObtenerPermisos(int rolId)
    {
        string query = @"SELECT m.men_id, m.men_nombre, m.men_icono, m.men_padre,
                                ISNULL(rm.romen_puedeVer, 0)
                        FROM Menu m
                        LEFT JOIN RolMenu rm ON rm.romen_menu = m.men_id AND rm.romen_rol = @RolId
                        WHERE m.men_activo = 1
                        ORDER BY m.men_orden";

        Dictionary<string, object> parametros = new Dictionary<string, object>() { { "@RolId", rolId } };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        var lista = new List<MenuPermisoVM>();
        var padres = new Dictionary<int, int?>();

        while (reader.Read())
        {
            var vm = new MenuPermisoVM
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Icono = reader.IsDBNull(2) ? null : reader.GetString(2),
                Marcado = Convert.ToBoolean(reader.GetValue(4))
            };
            lista.Add(vm);
            padres[vm.Id] = reader.IsDBNull(3) ? null : reader.GetInt32(3);
        }
        reader.Close();

        foreach (var hijo in lista.Where(x => padres[x.Id] != null))
            lista.First(p => p.Id == padres[hijo.Id]).Hijos.Add(hijo);

        return lista.Where(x => padres[x.Id] == null).ToList();
    }

    public bool GuardarPermisos(int rolId, int[] menus)
    {
        var sql = new System.Text.StringBuilder(
            "SET XACT_ABORT ON; BEGIN TRANSACTION; DELETE FROM RolMenu WHERE romen_rol = @Rol;");
        var parametros = new Dictionary<string, object?>() { { "@Rol", rolId } };

        for (int i = 0; i < menus.Length; i++)
        {
            sql.Append($" INSERT INTO RolMenu (romen_rol, romen_menu, romen_puedeVer) VALUES (@Rol, @M{i}, 1);");
            parametros[$"@M{i}"] = menus[i];
        }
        sql.Append(" COMMIT TRANSACTION;");

        return ExecutedCommand(sql.ToString(), parametros);
    }
}