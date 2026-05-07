using System;
using System.Linq;
using System.Threading.Tasks;
using ProiectAbonamente.Logica;
using ProiectAbonamente.Modele;

namespace ProiectAbonamente;

public class AplicatieConsola
{
    private readonly SistemCentral _sistem;

    public AplicatieConsola(SistemCentral sistem) => _sistem = sistem;

    public async Task Ruleaza()
    {
        await _sistem.InitializeazaSistem();
        while (true)
        {
            if (_sistem.UtilizatorCurent == null)
            {
                Console.WriteLine("\n================ LOGIN ================");
                Console.Write("User (default: admin / client): "); string u = Console.ReadLine();
                Console.Write("Pass (default: admin / client): "); string p = Console.ReadLine();
                
                if (!_sistem.Autentificare(u, p))
                {
                    Console.WriteLine("Date incorecte! Incearca din nou.");
                }
            }
            else
            {
                Console.WriteLine($"\nSalut, {_sistem.UtilizatorCurent.NumeUtilizator}!");
                if (_sistem.UtilizatorCurent.Tip == TipUtilizator.Administrator) 
                    await MeniuAdmin();
                else 
                    await MeniuClient();
            }
        }
    }

    private async Task MeniuAdmin()
    {
        Console.WriteLine("\n--- PANOU ADMIN ---");
        Console.WriteLine("1. Adauga Serviciu Nou");
        Console.WriteLine("2. Modifica Serviciu Existent (Descriere/Categ)");
        Console.WriteLine("3. Sterge Serviciu");
        Console.WriteLine("4. Adauga Plan la Serviciu");
        Console.WriteLine("5. Modifica Pret Plan");
        Console.WriteLine("6. Vezi Toate Abonamentele");
        Console.WriteLine("7. Logout");
        Console.Write("Alege optiunea: ");
        string op = Console.ReadLine();

        try 
        {
            switch (op)
            {
                case "1":
                    Console.Write("Nume Serviciu: "); string n = Console.ReadLine();
                    Console.Write("Descriere: "); string d = Console.ReadLine();
                    Console.Write("Categorie: "); string c = Console.ReadLine();
                    await _sistem.CreazaServiciu(n, d, c);
                    break;
                case "2":
                    Console.Write("Nume Serviciu de modificat: "); string nm = Console.ReadLine();
                    Console.Write("Noua Descriere (Enter pt a pastra): "); string nd = Console.ReadLine();
                    Console.Write("Noua Categorie (Enter pt a pastra): "); string nc = Console.ReadLine();
                    await _sistem.ModificaServiciu(nm, nd, nc);
                    break;
                case "3":
                    Console.Write("Nume Serviciu de sters: "); string ns = Console.ReadLine();
                    Console.Write("Esti sigur? Toate abonamentele vor fi sterse! (y/n): ");
                    if (Console.ReadLine() == "y") await _sistem.StergeServiciu(ns);
                    break;
                case "4":
                    Console.Write("Nume Serviciu: "); string sPlan = Console.ReadLine();
                    Console.Write("Nume Plan (ex: Basic): "); string nPlan = Console.ReadLine();
                    Console.Write("Pret: "); decimal pret = decimal.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Durata (zile): "); int zile = int.Parse(Console.ReadLine() ?? "30");
                    await _sistem.AdaugaPlan(sPlan, nPlan, pret, zile);
                    break;
                case "5":
                    Console.Write("Nume Serviciu: "); string sPret = Console.ReadLine();
                    Console.Write("Nume Plan: "); string nPret = Console.ReadLine();
                    Console.Write("Noul Pret: "); decimal pretNou = decimal.Parse(Console.ReadLine() ?? "0");
                    await _sistem.ModificaPretPlan(sPret, nPret, pretNou);
                    break;
                case "6":
                    Console.WriteLine("\n--- LISTA ABONAMENTE ---");
                    foreach(var a in _sistem.ObțineToateAbonamentele()) 
                    {
                        Console.WriteLine($"User: {a.ClientId} | Serviciu: {a.NumeServiciu} | Plan: {a.NumePlan} | Expira: {a.DataExpirare}");
                    }
                    break;
                case "7":
                    _sistem.Deconectare();
                    break;
                default:
                    Console.WriteLine("Optiune invalida.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    private async Task MeniuClient()
    {
        Console.WriteLine("\n--- MENIU CLIENT ---");
        Console.WriteLine("1. Cauta Servicii (Nume/Categorie)");
        Console.WriteLine("2. Cauta Servicii (Sub un Pret)");
        Console.WriteLine("3. Abonamentele Mele (Active & Istoric)");
        Console.WriteLine("4. Anuleaza Abonament");
        Console.WriteLine("5. Reinnoieste Abonament");
        Console.WriteLine("6. Logout");
        Console.Write("Alege optiunea: ");
        string op = Console.ReadLine();

        switch (op)
        {
            case "1":
                Console.Write("Cauta (lasa gol pentru toate): ");
                string term = Console.ReadLine();
                AfiseazaSiAboneaza(term, null);
                break;
            case "2":
                Console.Write("Pret Maxim: ");
                if(decimal.TryParse(Console.ReadLine(), out decimal maxP))
                    AfiseazaSiAboneaza(null, maxP);
                else Console.WriteLine("Pret invalid.");
                break;
            case "3":
                Console.WriteLine("\n--- ABONAMENTELE MELE ---");
                foreach(var a in _sistem.VeziAbonamenteleMele()) 
                {
                    string status = a.EsteActiv ? "ACTIV" : "ANULAT";
                    if(a.EsteActiv && a.DataExpirare < DateTime.Now) status = "EXPIRAT";
                    Console.WriteLine($"[{status}] {a.NumeServiciu} ({a.NumePlan}) - Expira: {a.DataExpirare} (ID: {a.Id})");
                }
                break;
            case "4":
                Console.Write("Introdu ID Abonament de anulat (copiaza ID-ul de la optiunea 3): ");
                if(Guid.TryParse(Console.ReadLine(), out Guid idAnulare))
                    await _sistem.AnuleazaAbonament(idAnulare);
                else Console.WriteLine("ID Invalid.");
                break;
            case "5":
                Console.Write("Introdu ID Abonament de reinnoit (copiaza ID-ul de la optiunea 3): ");
                if(Guid.TryParse(Console.ReadLine(), out Guid idRenew))
                    await _sistem.ReinnoiesteAbonament(idRenew);
                else Console.WriteLine("ID Invalid.");
                break;
            case "6":
                _sistem.Deconectare();
                break;
            default:
                Console.WriteLine("Optiune invalida.");
                break;
        }
    }

    private async void AfiseazaSiAboneaza(string term, decimal? pretMax)
    {
        var servicii = _sistem.CautaServiciiAvansat(term, pretMax);
        if (!servicii.Any()) { Console.WriteLine("Nu s-au gasit servicii."); return; }

        foreach (var s in servicii)
        {
            Console.WriteLine($"\n> SERVICIU: {s.Nume} | Categorie: {s.Categorie} | Descriere: {s.Descriere}");
            if (s.Planuri.Any())
            {
                foreach(var p in s.Planuri)
                {
                    Console.WriteLine($"   - Plan: {p.Nume} | Pret: {p.Pret} RON | Durata: {p.DurataZile} zile");
                }
                
                Console.Write("Doresti sa te abonezi la acest serviciu? (y/n): ");
                if (Console.ReadLine() == "y")
                {
                    Console.Write("Scrie numele planului dorit (ex: Basic): ");
                    string planAles = Console.ReadLine();
                    var planObj = s.Planuri.FirstOrDefault(p => p.Nume.Equals(planAles, StringComparison.OrdinalIgnoreCase));
                    
                    if(planObj != null) 
                    {
                        await _sistem.Aboneaza(s, planObj);
                        Console.WriteLine("Abonament realizat cu succes!");
                    }
                    else Console.WriteLine("Plan inexistent.");
                }
            }
            else
            {
                Console.WriteLine("   (Nu exista planuri definite pentru acest serviciu)");
            }
        }
    }
}