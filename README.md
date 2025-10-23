# MiniTickets

**MVP fonctionnel** de gestion de tickets via **API ASP.NET Core**, **Blazor WebAssembly**, **MongoDB**, avec tests et pipeline CI/CD minimal.

## Structure du projet

- Api/ : API ASP.NET Core avec endpoints REST, validation, Swagger et persistance MongoDB.  
- Client/ : Application Blazor WebAssembly pour créer, lister et mettre à jour les tickets.  
- tests/ : Tests unitaires et d’intégration pour l’API.  
- Dockerfile : Conteneurisation multi-stage pour l’API (optionnel si Docker disponible).  

## Prérequis

- .NET 8 SDK : https://dotnet.microsoft.com/download  
- MongoDB dans le cloud (Atlas)  
- (Optionnel) Visual Studio / VS Code pour développement local.  

## Installation et exécution

### 1. Exécution locale de l’API

Naviguer dans le dossier Api et lancer l’API :

cd src/Api  
dotnet run

L’API sera disponible sur http://localhost:5105/ 
Swagger UI pour tester les endpoints : http://localhost:5105/swagger

### 2. Exécution locale du client Blazor

Naviguer dans le dossier Client et lancer le client :

cd src/Client  
dotnet run

Le client sera accessible sur http://localhost:5063/ (ou le port indiqué par dotnet)

### 3. Tests

Pour exécuter les tests unitaires et d’intégration pour l’API :

dotnet test

## Fonctionnalités disponibles

- Création de tickets (Title obligatoire 3–100 caractères, Description optionnelle 0–500 caractères)  
- Liste des tickets avec filtre par status (Open, InProgress, Done) et pagination  
- Consultation d’un ticket par ID  
- Mise à jour du status d’un ticket (PUT /api/tickets/{id}/status)  
- Persistance MongoDB avec index sur Status et CreatedAt  
- Documentation Swagger activée  
- Logging structuré minimal via Serilog  
- Tests unitaires et d’intégration inclus

## Limites connues / points d’amélioration

- Pas d’authentification ni d’autorisation  
- Pas d’assignation d’un ticket à un utilisateur  
- Pas de suivi d’audit ou historique des modifications  
- UI minimale côté Blazor WebAssembly (alertes JS simples, pas de styles avancés)

## Statut du projet

✅ MVP terminé et fonctionnel  
- Tous les endpoints demandés sont implémentés et testés  
- Code structuré avec séparation Domain / Service / Repository  
- Projet prêt à être poussé sur GitHub et partagé
