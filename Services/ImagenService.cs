namespace Inventario.Services;

public class ImagenService
{
    private readonly IWebHostEnvironment _env;

    public ImagenService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string Guardar(IFormFile? imagen, string carpeta, string imagenActual = "")
    {
        if (imagen == null || imagen.Length == 0)
            return imagenActual;

        string nombre = Path.GetFileName(imagen.FileName);
        string ruta = Path.Combine(_env.ContentRootPath, "Images", carpeta, nombre);

        Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);

        if (!File.Exists(ruta))
        {
            using var stream = new FileStream(ruta, FileMode.Create);
            imagen.CopyTo(stream);
        }

        return nombre;
    }
}