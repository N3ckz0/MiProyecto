namespace MiProyecto.Aplicacion.Saludo;

public interface ISaludoService
{
    string ObtenerSaludo();
}

public class SaludoService : ISaludoService
{
    public string ObtenerSaludo()
    {
        return "Hola desde Aplicacion 🚀";
    }
}