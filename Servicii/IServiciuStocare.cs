using System.Threading.Tasks;

namespace ProiectAbonamente.Servicii;

public interface IServiciuStocare
{
    Task SalveazaDate<T>(string numeFisier, T date);
    Task<T> IncarcaDate<T>(string numeFisier);
}