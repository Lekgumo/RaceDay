RaceDay - Event Management System

[![Build and Test](https://github.com/YOUR-USERNAME/RaceDay/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/YOUR-USERNAME/RaceDay/actions/workflows/build-and-test.yml)

> A full-stack RESTful API for managing road running, walking, and cycling events in South Africa.


System Description

RaceDay is a comprehensive event management platform built for the South African road running, walking, and cycling community. From the iconic Comrades Marathon and Cape Town Cycle Tour to community park runs and charity walks, RaceDay streamlines the entire event lifecycle.

The system replaces paper-based registration, spreadsheets, and disconnected communication channels with a modern, secure, RESTful API. Organisers manage events and results; Participants browse, enrol, and track their performance history.


Roles

The system has two distinct user roles**:

Organiser
Event creators and managers. Can:
- Register and log in with secure hashed credentials
- Create, update, and delete events
- Define multiple categories per event (e.g., 5km Walk, 21km Half Marathon, 42km Full Marathon)
- Set entry fees, start times, and capacity limits per category
- View all enrolments for their events
- Record finish times and positions for participants
- View event weather forecasts

Participant
Event attendees and competitors. Can:
- Register and log in
- Browse all upcoming events (publicly visible)
- View event details and available categories
- Enrol in categories (subject to capacity)
- View personal enrolment history
- Cancel their own enrolments
- View personal performance results

Technology Stack

| Component | Technology |
|-----------|------------|
| **Framework** | ASP.NET Core 8 Web API |
| **Database** | SQL Server (LocalDB for development) |
| **ORM** | Entity Framework Core 8 (Code-First) |
| **Authentication** | Session-based (custom middleware) |
| **Password Hashing** | BCrypt.Net-Next |
| **API Documentation** | Swagger (Swashbuckle) |
| **Testing** | xUnit + FluentAssertions + InMemory DB |
| **CI/CD** | GitHub Actions |


Repository Structure
