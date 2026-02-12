# IoT Asset Tracking And Sensor Monitoring

IoTDevices is an IoT asset tracking and sensor monitoring hardware company
that has multiple different device types on offer, each with varying capabilities,
from Low-power GPS Tracker to IoT Data Loggers.

## Deployment

The backend is hosted on Azure Free Tier.  
After periods of inactivity, the API may take a few seconds to wake up.  
If data does not appear, please refresh the page.

https://vermillion-faloodeh-8d3f2f.netlify.app/

- Database: I deployed my database on Azure SQL Database
- Backend: I deployed the backend API on Azure App Service
- Frontend: I Deployed the frontend on Netlify

## Tech Stack

- Database: MSSQL
- Backend: ASP.NET Core Web API, C#
- Frontend: HTML, CSS, JavaScript
- Tools: Entity Framework Core, SQL Server Management Studio

## Project Structure

```
IoT_Asset_Tracking_And_Sensor_Monitoring/
│
├── .github/workflows/  # GitHub Actions CI/CD workflows
│
├── Backend/ # ASP.NET Core Web API 
│ ├── Controllers/ # API controllers (CRUD endpoints)
│ ├── Data/ # DbContext 
│ ├── DTOs/ # Data Transfer Objects
│ ├── Models/ # Entity models      
│ ├── Backend.csproj # Project definition
│ ├── Program.cs # Application entry point
│ ├── appsettings.json # Application configuration
│ └── README.md # Backend documentation
│
├── Database/ # SQL Server database and Deliverable 1
│ ├── DatabaseScripts.sql # Database schema creation scripts
│ ├── FewRecords.sql # Few Records to populate the tables
│ └── README.md # Database documentation
│
├── Frontend/ # Frontend (HTML, CSS, JS)
│ ├── css/
│ │ └── styles.css # Styling
│ ├── js/
│ │ ├── firmware.js # Firmware CRUD logic
│ │ └── groups.js # Device group logic
│ ├── index.html # Landing page
│ ├── firmware.html # Firmware management UI
│ ├── groups.html # Device group UI
│ └── README.md # Frontend documentation
│
├── README.md # Main project documentation
└── .gitignore # Git ignored files
```

## Running the Web App locally

### Prerequisites
- .NET 10 SDK
- SQL Server Express
- SQL Server Management Studio (SSMS)
- Visual Studio Code
- Live Server Extension
- Git/GitHub

### 1. Clone the Repository

```bash
git clone https://github.com/Ntokozo-skosana/IoTAssetTracking.git
cd IoTAssetTracking
```

### 2. Set up the Database

- My deliverable 1 is under the Database folder, I named
  the scripts to create the database structure DatabaseScripts.sql
  and the records are name FewRecords.sql
- Open SSMS and connect to your SQL Server instance.
- New Query.
- Paste DatabaseScripts.sql then execute and the IoTDevices database with tables will be created.
- New Query.
- Paste FewRecords.sql to populate the database with few records the execute.

### 3. Configure the Connection String

- Change the connection string in Backend/appsettings.json
- Currently it has the Microsoft Azure connection string which I inserted when I was deploying.
- When i was working locally, it was: "Server=localhost\\SQLEXPRESS;Database=IoTDevices;Trusted_Connection=True;TrustServerCertificate=True;

### 4. Run Backend
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

### 5. Run Frontend
- In the JavaScript files, configure backend API URL.
- Currently my JavaScript files have the deployed backend URL, you
  can add the local backend URL obtained after running backend.
- Right Click on index.html in VS Code
- Open with live server




