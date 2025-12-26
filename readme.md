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