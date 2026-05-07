using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProiectAbonamente.Modele;
using ProiectAbonamente.Servicii;

namespace ProiectAbonamente.Logica;

public class SistemCentral
{
    private readonly IServiciuStocare _stocare;
    private readonly ILogger<SistemCentral> _logger;
    
    private List<Utilizator> _utilizatori = new();
    private List<Serviciu> _servicii = new();
    private List<Abonament> _abonamente = new();

    public Utilizator UtilizatorCurent { get; private set; }

    public SistemCentral(IServiciuStocare stocare, ILogger<SistemCentral> logger)
    {
        _stocare = stocare;
        _logger = logger;
    }

    public async Task InitializeazaSistem()
    {
        _utilizatori = await _stocare.IncarcaDate<List<Utilizator>>("utilizatori.json") ?? new List<Utilizator>();
        _servicii = await _stocare.IncarcaDate<List<Serviciu>>("servicii.json") ?? new List<Serviciu>();
        _abonamente = await _stocare.IncarcaDate<List<Abonament>>("abonamente.json") ?? new List<Abonament>();
        
        if (!_utilizatori.Any())
        {
            _utilizatori.Add(new Utilizator(Guid.NewGuid(), "admin", "admin", TipUtilizator.Administrator));
            _utilizatori.Add(new Utilizator(Guid.NewGuid(), "client", "client", TipUtilizator.Client));
            await SalveazaTot();
        }
        _logger.LogInformation("Sistem initializat si date incarcate.");
    }

    private async Task SalveazaTot()
    {
        await _stocare.SalveazaDate("utilizatori.json", _utilizatori);
        await _stocare.SalveazaDate("servicii.json", _servicii);
        await _stocare.SalveazaDate("abonamente.json", _abonamente);
    }

    public bool Autentificare(string nume, string parola)
    {
        var user = _utilizatori.FirstOrDefault(u => u.NumeUtilizator == nume && u.Parola == parola);
        if (user != null)
        {
            UtilizatorCurent = user;
            _logger.LogInformation("Utilizatorul {Nume} s-a autentificat.", nume);
            return true;
        }
        _logger.LogWarning("Tentativa de autentificare esuata pentru: {Nume}", nume);
        return false;
    }

    public void Deconectare()
    {
        _logger.LogInformation("Utilizatorul {Nume} s-a deconectat.", UtilizatorCurent?.NumeUtilizator);
        UtilizatorCurent = null;
    }

    public async Task CreazaServiciu(string nume, string descriere, string categorie)
    {
        if (UtilizatorCurent?.Tip != TipUtilizator.Administrator) return;
        
        if (_servicii.Any(s => s.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Exista deja un serviciu cu acest nume.");
            return;
        }

        _servicii.Add(new Serviciu(Guid.NewGuid(), nume, descriere, categorie, new List<PlanAbonament>()));
        await SalveazaTot();
        _logger.LogInformation("Admin creat serviciu: {Nume}", nume);
    }

    public async Task ModificaServiciu(string numeServiciu, string descriereNoua, string categorieNoua)
    {
        if (UtilizatorCurent?.Tip != TipUtilizator.Administrator) return;

        var serviciuExistent = _servicii.FirstOrDefault(s => s.Nume.Equals(numeServiciu, StringComparison.OrdinalIgnoreCase));
        if (serviciuExistent != null)
        {
            var serviciuActualizat = serviciuExistent with 
            { 
                Descriere = string.IsNullOrWhiteSpace(descriereNoua) ? serviciuExistent.Descriere : descriereNoua, 
                Categorie = string.IsNullOrWhiteSpace(categorieNoua) ? serviciuExistent.Categorie : categorieNoua 
            };

            _servicii.Remove(serviciuExistent);
            _servicii.Add(serviciuActualizat);
            await SalveazaTot();
            _logger.LogInformation("Admin modificat serviciu: {Nume}", numeServiciu);
        }
        else Console.WriteLine("Serviciul nu a fost gasit.");
    }

    public async Task StergeServiciu(string numeServiciu)
    {
        if (UtilizatorCurent?.Tip != TipUtilizator.Administrator) return;

        var serviciu = _servicii.FirstOrDefault(s => s.Nume.Equals(numeServiciu, StringComparison.OrdinalIgnoreCase));
        if (serviciu != null)
        {
            _servicii.Remove(serviciu);
            _abonamente.RemoveAll(a => a.NumeServiciu == serviciu.Nume);
            
            await SalveazaTot();
            _logger.LogWarning("Admin sters serviciu: {Nume}", numeServiciu);
        }
        else Console.WriteLine("Serviciul nu a fost gasit.");
    }

    public async Task AdaugaPlan(string numeServiciu, string numePlan, decimal pret, int zile)
    {
        if (UtilizatorCurent?.Tip != TipUtilizator.Administrator) return;
        
        var s = _servicii.FirstOrDefault(x => x.Nume.Equals(numeServiciu, StringComparison.OrdinalIgnoreCase));
        if (s != null)
        {
            s.Planuri.Add(new PlanAbonament(numePlan, pret, zile, "Acces Standard"));
            await SalveazaTot();
            _logger.LogInformation("Adaugat plan {Plan} la serviciul {Serv}", numePlan, numeServiciu);
        }
        else Console.WriteLine("Serviciul nu exista.");
    }

    public async Task ModificaPretPlan(string numeServiciu, string numePlan, decimal pretNou)
    {
         if (UtilizatorCurent?.Tip != TipUtilizator.Administrator) return;

         var serviciu = _servicii.FirstOrDefault(s => s.Nume.Equals(numeServiciu, StringComparison.OrdinalIgnoreCase));
         if (serviciu != null)
         {
             var plan = serviciu.Planuri.FirstOrDefault(p => p.Nume.Equals(numePlan, StringComparison.OrdinalIgnoreCase));
             if (plan != null)
             {
                 serviciu.Planuri.Remove(plan);
                 serviciu.Planuri.Add(plan with { Pret = pretNou });
                 await SalveazaTot();
                 _logger.LogInformation("Pret modificat la {Pret} pentru {Plan}", pretNou, numePlan);
             }
             else Console.WriteLine("Planul nu exista.");
         }
         else Console.WriteLine("Serviciul nu exista.");
    }

    public List<Abonament> ObțineToateAbonamentele() => 
        UtilizatorCurent?.Tip == TipUtilizator.Administrator ? _abonamente.ToList() : new List<Abonament>();
    
    public List<Serviciu> CautaServiciiAvansat(string keyword, decimal? pretMaxim = null)
    {
        return _servicii.Where(s => 
            (string.IsNullOrEmpty(keyword) || 
             s.Nume.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
             s.Categorie.Contains(keyword, StringComparison.OrdinalIgnoreCase)) &&
            (!pretMaxim.HasValue || s.Planuri.Any(p => p.Pret <= pretMaxim))
        ).ToList();
    }

    public async Task Aboneaza(Serviciu s, PlanAbonament p)
    {
        if (UtilizatorCurent?.Tip != TipUtilizator.Client) return;
        
        var ab = new Abonament(Guid.NewGuid(), UtilizatorCurent.Id, s.Nume, p.Nume, DateTime.Now, DateTime.Now.AddDays(p.DurataZile), true);
        _abonamente.Add(ab);
        await SalveazaTot();
        _logger.LogInformation("Clientul {User} s-a abonat la {Serviciu}.", UtilizatorCurent.NumeUtilizator, s.Nume);
    }

    public List<Abonament> VeziAbonamenteleMele() => 
        _abonamente.Where(a => a.ClientId == UtilizatorCurent?.Id).ToList();

    public async Task AnuleazaAbonament(Guid id)
    {
        var ab = _abonamente.FirstOrDefault(a => a.Id == id);
        if (ab != null && (ab.ClientId == UtilizatorCurent?.Id || UtilizatorCurent?.Tip == TipUtilizator.Administrator))
        {
            var anulat = ab with { EsteActiv = false };
            _abonamente.Remove(ab);
            _abonamente.Add(anulat);
            await SalveazaTot();
            _logger.LogInformation("Abonament {Id} anulat.", id);
        }
    }

    public async Task ReinnoiesteAbonament(Guid abonamentId)
    {
        var ab = _abonamente.FirstOrDefault(a => a.Id == abonamentId);
        
        if (ab != null && ab.ClientId == UtilizatorCurent?.Id)
        {
            var serviciu = _servicii.FirstOrDefault(s => s.Nume == ab.NumeServiciu);
            var plan = serviciu?.Planuri.FirstOrDefault(p => p.Nume == ab.NumePlan);
            
            int zileAdaugate = plan?.DurataZile ?? 30; 
            
            DateTime nouaDataExpirare = ab.DataExpirare > DateTime.Now 
                ? ab.DataExpirare.AddDays(zileAdaugate) 
                : DateTime.Now.AddDays(zileAdaugate);

            var abReinnoit = ab with { DataExpirare = nouaDataExpirare, EsteActiv = true };

            _abonamente.Remove(ab);
            _abonamente.Add(abReinnoit);
            
            await SalveazaTot();
            _logger.LogInformation("Abonament {Id} reinnoit pana la {Data}.", abonamentId, nouaDataExpirare);
        }
        else Console.WriteLine("Abonamentul nu a fost gasit sau nu va apartine.");
    }
}