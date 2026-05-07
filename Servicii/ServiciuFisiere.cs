using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProiectAbonamente.Servicii;

public class ServiciuFisiere : IServiciuStocare
{
    public async Task SalveazaDate<T>(string numeFisier, T date)
    {
        try
        {
            var optiuni = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(date, optiuni);
            await File.WriteAllTextAsync(numeFisier, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la salvarea datelor: {ex.Message}");
        }
    }

    public async Task<T> IncarcaDate<T>(string numeFisier)
    {
        if (!File.Exists(numeFisier)) return default;
        try
        {
            string json = await File.ReadAllTextAsync(numeFisier);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}