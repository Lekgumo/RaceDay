CREATE DATABASE RaceDay;

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    DateRegistered DATETIME2 DEFAULT GETUTCDATE(),
    IsActive BIT DEFAULT 1
);

CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Date DATETIME2 NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    MaxParticipants INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Draft' CHECK (Status IN ('Draft', 'Open', 'Closed', 'Cancelled')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId) ON DELETE CASCADE,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0,
    StartTime TIME NULL,
    MaxCapacity INT NOT NULL,
    CONSTRAINT UQ_Category_EventName UNIQUE (EventId, Name)
);

CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    EnrolmentDate DATETIME2 DEFAULT GETUTCDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled', 'Completed')),
    PaymentReference NVARCHAR(100) NULL,
    BibNumber INT NULL,
    CONSTRAINT UQ_Enrolment_ParticipantCategory UNIQUE (ParticipantId, CategoryId)
);

CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId),
    FinishTime TIME NULL,
    Position INT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'DNS' CHECK (Status IN ('DNS', 'DNF', 'Finished')),
    Notes NVARCHAR(MAX) NULL
);

CREATE TABLE WeatherForecasts (
    ForecastId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId) ON DELETE CASCADE,
    ForecastDate DATE NOT NULL,
    TemperatureHigh DECIMAL(5,2) NULL,
    TemperatureLow DECIMAL(5,2) NULL,
    [Condition] NVARCHAR(100) NULL,
    WindSpeed DECIMAL(5,2) NULL,
    Humidity DECIMAL(5,2) NULL,
    LastUpdated DATETIME2 DEFAULT GETUTCDATE()
);

INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role)
VALUES
('Lerato', 'Mokoena', 'lerato.m@raceday.co.za', 'hashed_password_123', 'Organiser'),
('Sipho', 'Ndlovu', 'sipho.n@raceday.co.za', 'hashed_password_456', 'Organiser');


INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role)
VALUES
('Thabo', 'Mkhize', 'thabo.m@example.com', 'hashed_password_789', 'Participant'),
('Zanele', 'Khumalo', 'zanele.k@example.com', 'hashed_password_101', 'Participant'),
('Ahmed', 'Ebrahim', 'ahmed.e@example.com', 'hashed_password_112', 'Participant');


INSERT INTO Events (OrganiserId, Name, Description, Date, Location, MaxParticipants, Status)
VALUES
(1, 'Cape Town Cycle Tour 2026', 'The world''s largest timed cycle event.', '2026-03-08 06:00:00', 'Cape Town, Western Cape', 35000, 'Open'),
(2, 'Comrades Marathon 2026', 'The ultimate human race.', '2026-06-01 05:30:00', 'Pietermaritzburg to Durban', 25000, 'Draft'),
(1, 'Soweto Marathon 2026', 'Experience the energy of Soweto.', '2026-11-01 06:00:00', 'Soweto, Gauteng', 15000, 'Open');


DECLARE @CapeTownEventId INT = (SELECT EventId FROM Events WHERE Name = 'Cape Town Cycle Tour 2026');
DECLARE @ComradesEventId INT = (SELECT EventId FROM Events WHERE Name = 'Comrades Marathon 2026');
DECLARE @SowetoEventId INT = (SELECT EventId FROM Events WHERE Name = 'Soweto Marathon 2026');


INSERT INTO Categories (EventId, Name, Description, EntryFee, StartTime, MaxCapacity)
VALUES
(@CapeTownEventId, 'Fun Ride (35km)', 'Leisure ride for beginners and families.', 150.00, '07:00:00', 10000),
(@CapeTownEventId, 'Challenge (78km)', 'The full Cape Town Cycle Tour experience.', 250.00, '06:30:00', 15000),
(@CapeTownEventId, 'Elite (109km)', 'For competitive cyclists and clubs.', 350.00, '06:00:00', 5000);


INSERT INTO Categories (EventId, Name, Description, EntryFee, StartTime, MaxCapacity)
VALUES
(@ComradesEventId, 'Up Run (86km)', 'The classic run from Durban to Pietermaritzburg.', 800.00, '05:30:00', 12000),
(@ComradesEventId, 'Down Run (89km)', 'From Pietermaritzburg to Durban.', 800.00, '05:30:00', 13000);


INSERT INTO Categories (EventId, Name, Description, EntryFee, StartTime, MaxCapacity)
VALUES
(@SowetoEventId, '5km Fun Walk', 'Family-friendly walk.', 50.00, '08:00:00', 5000),
(@SowetoEventId, '21km Half Marathon', 'A challenging half marathon.', 150.00, '07:00:00', 4000),
(@SowetoEventId, '42km Full Marathon', 'The main Soweto Marathon event.', 250.00, '06:00:00', 6000);


DECLARE @ThaboId INT = (SELECT UserId FROM Users WHERE Email = 'thabo.m@example.com');
DECLARE @ZaneleId INT = (SELECT UserId FROM Users WHERE Email = 'zanele.k@example.com');
DECLARE @AhmedId INT = (SELECT UserId FROM Users WHERE Email = 'ahmed.e@example.com');

DECLARE @FunRideCatId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Fun Ride (35km)');
DECLARE @ChallengeCatId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Challenge (78km)');
DECLARE @UpRunCatId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Up Run (86km)');
DECLARE @FullMarathonCatId INT = (SELECT CategoryId FROM Categories WHERE Name = '42km Full Marathon');

INSERT INTO Enrolments (ParticipantId, CategoryId, Status, PaymentReference, BibNumber)
VALUES
(@ThaboId, @FunRideCatId, 'Confirmed', 'PAY-12345-ABC', 101),
(@ZaneleId, @ChallengeCatId, 'Pending', NULL, NULL),
(@ThaboId, @UpRunCatId, 'Confirmed', 'PAY-67890-DEF', 202),
(@AhmedId, @FullMarathonCatId, 'Confirmed', 'PAY-11223-GHI', 303);


INSERT INTO Results (ParticipantId, CategoryId, EventId, FinishTime, Position, Status, Notes)
VALUES
(@ThaboId, @FunRideCatId, @CapeTownEventId, '01:15:30', 150, 'Finished', 'Good performance.'),
(@ZaneleId, @ChallengeCatId, @CapeTownEventId, '02:45:12', 300, 'Finished', NULL),
(@ThaboId, @UpRunCatId, @ComradesEventId, '08:30:45', 500, 'Finished', 'Personal best.'),
(@AhmedId, @FullMarathonCatId, @SowetoEventId, '04:20:00', 100, 'DNF', 'Cramped at 35km.');


INSERT INTO WeatherForecasts (EventId, ForecastDate, TemperatureHigh, TemperatureLow, [Condition], WindSpeed, Humidity)
VALUES
(@CapeTownEventId, '2026-03-08', 28.5, 18.0, 'Sunny', 15.0, 55.0),
(@ComradesEventId, '2026-06-01', 22.0, 12.0, 'Partly Cloudy', 20.0, 65.0),
(@SowetoEventId, '2026-11-01', 30.0, 16.0, 'Hot and Clear', 10.0, 40.0);