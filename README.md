# Szablon Clean Architecture

Szablon startowy Web API w ASP.NET Core oparty na zasadach Clean Architecture.  
Repozytorium pokazuje przykładową strukturę projektu oraz podstawowe mechanizmy używane w nowoczesnych aplikacjach .NET.

## Architektura

- **Warstwa Domain** – definicje encji i modeli domenowych  
- **Warstwa Application** – przypadki użycia (CQRS) i logika aplikacyjna  
- **Warstwa Infrastructure** – dostęp do danych i integracje zewnętrzne  
- **Warstwa Web.Api** – kontrolery REST API  

## Kluczowe technologie

- **.NET 10**
- **Entity Framework Core**
- **MediatR**
- **FluentValidation**
- **JWT Bearer**
- **SQL Server**

## Testowanie

- **TUnit**
- **NSubstitute**
- **NetArchTest**
- **Testcontainers**

## Uwierzytelnianie i autoryzacja

- JWT Bearer
- Refresh tokeny
- Autoryzacja oparta na uprawnieniach
- Reset hasła
- Hashowanie haseł BCrypt

## Przykładowe funkcjonalności

- Zarządzanie użytkownikami (CRUD)
- Logowanie i odświeżanie tokenów
- Kontrola dostępu oparta na uprawnieniach
- Monitorowanie OpenTelemetry
- Powiadomienia email

## Cel projektu

Projekt stanowi **szablon startowy Clean Architecture**, który może być użyty jako baza do budowy nowych aplikacji ASP.NET Core.