🏃 RaceDay - Event Management System
https://github.com/Lekgumo/RaceDay/actions/workflows/build-and-test.yml/badge.svg

A full-stack event management system for the South African road running, walking, and cycling community.

📖 System Description
RaceDay is a full-stack application built with ASP.NET Core 8 and Entity Framework Core. The solution consists of three projects:

RaceDay (API): A RESTful API with session-based authentication, Azure Blob Storage integration, and role-based access control.

RaceDay.Web (MVC): The frontend web application that consumes the API.

RaceDay.Tests: Unit tests for the API using xUnit.

👥 Roles
Organiser: Can create and manage events, add categories, upload banner images, and record results.

Participant: Can browse upcoming events, enrol in categories, and view their personal results history.

🚀 How to Run the Project
Option 1: Using Docker (Recommended)
The easiest way to run the entire stack (API + MVC) is using Docker.

Prerequisites:

Docker Desktop installed and running on your machine.

Steps:

Open a terminal in the root folder of this repository.

Run the following command to build and start the containers:

bash
docker-compose up --build -d
Once the containers are running, you can access the applications:

MVC Web App: http://localhost:8081

API Swagger UI: http://localhost:8080/swagger

To stop the containers, run:

bash
docker-compose down
(Note: The Docker setup uses an In-Memory database so it works out-of-the-box without needing a local SQL Server connection).

Option 2: Running Locally (Visual Studio)
If you prefer to run the application locally, you will need SQL Server LocalDB.

Prerequisites:

.NET 8 SDK

Visual Studio 2022

SQL Server Express LocalDB

Steps:

Open RaceDay.sln in Visual Studio.

Right-click the Solution in Solution Explorer, select Configure Startup Projects, and choose Multiple startup projects.

Set both RaceDay (API) and RaceDay.Web (MVC) to Start.

Open the Package Manager Console (set Default project to RaceDay) and run:

powershell
Update-Database
Press F5 to run both projects.

The API Swagger UI will open automatically, and the MVC app will open at its designated port.

🎥 Video Presentation
Watch the Part 3 Walkthrough on YouTube

