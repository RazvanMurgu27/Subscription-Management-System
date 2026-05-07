using System;

namespace ProiectAbonamente.Modele;

public record Abonament(Guid Id, Guid ClientId, string NumeServiciu, string NumePlan, DateTime DataStart, DateTime DataExpirare, bool EsteActiv);