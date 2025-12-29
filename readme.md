# Inspira Solution

## Quick Start

### 1. Restore dependencies
```sh
dotnet restore
```

### 2. Build the solution
```sh
dotnet build
```

### 3. Configure MongoDB
- Update `src/Inspira.API/appsettings.json` with your MongoDB connection string.
- Default connection string: `mongodb://localhost:27017`
- [MongoDB Installation Guide](https://docs.mongodb.com/manual/installation/)

### 4. Run the API
```sh
dotnet run --project src/Inspira.API
```
- The API will be available at `https://localhost:5001` (see console output).
- Swagger UI: `/swagger` (development mode).

### 5. Run tests
```sh
dotnet test
```

---

## Sample MongoDB Data

**Form Collection**
```json
[
  { "_id": 1, "FormName": "Application Form" },
  { "_id": 2, "FormName": "Change of Address" }
]
```

**Submission Collection**
```json
[
  { "_id": 1, "SubmissionPropertyId": 10, "FormId": 1 },
  { "_id": 2, "SubmissionPropertyId": 20, "FormId": 2 }
]
```

**SubmissionProperty Collection**
```json
[
  { "_id": 10, "OwnerTaxId": "123456789", "OwnerContactId": "555" },
  { "_id": 20, "OwnerTaxId": "987654321", "OwnerContactId": "0" }
]
```

---

## API Authentication

Regarding the API authentication, I'm using auth0 as identity provider.
If you are running the solution locally, you should disable authentication
in the API project for testing purposes.


## Cybersecerity implementation


1.	Authentication and Authorization (e.g., JWT, OAuth2)
2.	Role-based access control (RBAC)
3.	Input validation and model validation
4.	HTTPS enforcement (TLS/SSL)
5.	CORS (Cross-Origin Resource Sharing) configuration
6.	Rate limiting and throttling
7.	Logging and monitoring of security events
8.	Exception handling and error message sanitization
9.	Data encryption at rest and in transit
10.	Use of secure HTTP headers (HSTS, X-Content-Type-Options, X-Frame-Options, etc.)
11.	Dependency/package vulnerability scanning
12.	Protection against injection attacks (SQL, NoSQL, command, etc.)
13.	Secure configuration management (secrets, connection strings)
14.	API key management (if applicable)



//[Authorize(Roles = "Manager,Admin")] : we should implement Role-based access control (RBAC), which is a security approach where access to resources or actions is granted based on a user's assigned roles

CORS (Cross-Origin Resource Sharing) configuration is a security feature that controls which external domains (origins) are allowed to access your API resources from a browser. By configuring CORS, you can specify which websites or client applications are permitted to make requests to your API, helping to prevent unauthorized or malicious cross-origin requests.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com") // Add allowed origins
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

Rate limiting and throttling are techniques used to control the number of requests a client can make to an API within a specific time period. This helps protect your API from abuse, denial-of-service attacks, and excessive resource consumption by limiting how frequently users or systems can access your endpoints.

Logging and monitoring of security events

Exception handling and error message sanitization

