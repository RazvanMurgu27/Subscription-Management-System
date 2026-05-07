# Subscription Management System 📦

A robust console-based application built with **C# and .NET**, designed to manage digital services and user subscriptions. 

This project demonstrates clean architecture principles, utilizing the **.NET Generic Host** for Dependency Injection, asynchronous programming, and structured JSON data persistence.

## 🚀 Tech Stack & Concepts Demonstrated
* **Language:** C#
* **Framework:** .NET Core / .NET 5+ 
* **Architecture:** Dependency Injection (`Microsoft.Extensions.Hosting`, `Microsoft.Extensions.DependencyInjection`), Separation of Concerns (Models, Services, Logic).
* **Data Storage:** Asynchronous file I/O operations with `System.Text.Json`.
* **OOP Concepts:** Interfaces (`IServiciuStocare`), encapsulation, and immutable data models using `record` types.

## 🔑 Key Features

### Administrator Panel
* **Service Management:** Create, modify, and delete digital services.
* **Plan Management:** Define custom subscription plans (e.g., Basic, Premium) with specific pricing and durations.
* **Global Overview:** View all active and expired subscriptions across the platform.

### Client Panel
* **Advanced Search:** Browse available services by name, category, or maximum price.
* **Subscription Handling:** Subscribe to specific service plans, renew existing subscriptions, or cancel them.
* **Personal Dashboard:** Track active, expired, and canceled subscriptions.

## 🛠️ How to Run
1. Clone the repository:
   ```bash
   git clone https://github.com/RazvanMurgu27/Subscription-Management-System.git
   ```
2. Navigate to the project directory.

3. Run the application using your preferred IDE (Rider/Visual Studio) or the .NET CLI:

```bash
dotnet run
```

4. Default Credentials:
Admin: User: admin | Pass: admin
Client: User: client | Pass: client

5. 📂 Project Structure
/Modele - Contains data entities (Utilizator, Serviciu, Abonament, PlanAbonament).

/Servicii - Contains the IServiciuStocare interface and its JSON file-based implementation.

/Logica - Core business logic (SistemCentral) handling operations and validations.
