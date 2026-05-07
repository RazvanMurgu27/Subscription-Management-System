using System;
using System.Collections.Generic;

namespace ProiectAbonamente.Modele;

public record Serviciu(Guid Id, string Nume, string Descriere, string Categorie, List<PlanAbonament> Planuri);