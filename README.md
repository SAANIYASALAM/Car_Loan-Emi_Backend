# Car Loan EMI Backend API

A complete ASP.NET Core 8.0 Web API for managing car loan applications and EMI payments, designed to integrate with the Angular frontend.

## Overview

This backend provides a comprehensive REST API for a Car Loan EMI application with the following features:

- User registration and authentication with JWT
- Loan application and eligibility checking
- EMI calculation and payment processing
- User profile and dashboard management
- Loan rules based on car types
- Payment history and upcoming payment tracking

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: JWT Bearer Token
- **Password Hashing**: BCrypt
- **ORM**: Entity Framework Core (Code-First)
- **API Documentation**: Swagger/OpenAPI

## Project Structure

```
CAR-LOAN-EMI/
├── Controllers/           # API Controllers
│   ├── AuthController.cs
│   ├── LoanController.cs
│   ├── EmiController.cs
│   └── UserController.cs
├── Models/
│   ├── Entities/         # Database entities
│   ├── DTOs/             # Data transfer objects
│   └── Enums/            # Enumerations
├── Services/
│   ├── Interfaces/       # Service contracts
│   └── Implementations/  # Service implementations
├── Repositories/
│   ├── Interfaces/       # Repository contracts
│   └── Implementations/  # Repository implementations
├── Helpers/              # Utility classes
│   ├── EmiCalculator.cs
│   ├── JwtHelper.cs
│   ├── LoanRulesEngine.cs
│   └── PasswordHasher.cs
├── Data/                 # Database context
│   └── ApplicationDbContext.cs
├── Middleware/           # Custom middleware
│   ├── ErrorHandlerMiddleware.cs
│   └── JwtMiddleware.cs
└── Program.cs           # Application entry point
```

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK or higher
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or Visual Studio Code

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Car_Loan-Emi_Backend/CAR-LOAN-EMI
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string**
   
   Edit `appsettings.json` and update the connection string to match your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=CarLoanDB;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```
   
   This will create the database and all required tables with seed data.

5. **Run the application**
   ```bash
   dotnet run
   ```
   
   The API will be available at:
   - HTTPS: `https://localhost:7XXX` (port may vary)
   - HTTP: `http://localhost:5XXX`
   - Swagger UI: `https://localhost:7XXX/swagger`

## API Endpoints

### Authentication Endpoints

- **POST** `/api/auth/register` - Register a new user
- **POST** `/api/auth/login` - Login and receive JWT token
- **POST** `/api/auth/logout` - Logout (client-side token removal)

### Loan Endpoints (Requires Authorization)

- **POST** `/api/loan/apply` - Apply for a car loan
- **GET** `/api/loan/user/{userId}` - Get all loans for a user
- **GET** `/api/loan/{loanId}` - Get specific loan details
- **POST** `/api/loan/check-eligibility` - Check loan eligibility
- **GET** `/api/loan/rules/{carType}` - Get loan rules for a car type
- **POST** `/api/loan/calculate-emi` - Calculate EMI amount

### EMI Payment Endpoints (Requires Authorization)

- **GET** `/api/emi/loan/{loanId}` - Get all EMI payments for a loan
- **GET** `/api/emi/upcoming/{userId}` - Get upcoming payments for user
- **GET** `/api/emi/history/{userId}` - Get payment history for user
- **POST** `/api/emi/pay` - Process an EMI payment

### User Endpoints (Requires Authorization)

- **GET** `/api/user/{userId}` - Get user profile
- **PUT** `/api/user/{userId}` - Update user profile
- **GET** `/api/user/{userId}/dashboard` - Get dashboard data with loans and stats

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. To access protected endpoints:

1. Register or login to receive a JWT token
2. Include the token in the Authorization header:
   ```
   Authorization: Bearer <your-token>
   ```

Token expiry is set to 24 hours (1440 minutes) by default.

## CORS Configuration

The API is configured to accept requests from the Angular frontend at:
- `http://localhost:4200`

To modify CORS settings, update the CORS policy in `Program.cs`.

## Database Schema

### Main Tables

- **Users** - User accounts and profiles
- **Loans** - Loan applications and details
- **EmiPayments** - EMI payment records
- **LoanRules** - Rules for different car types
- **KycDocuments** - KYC document uploads

### Key Features

- Cascade delete for related entities
- Decimal precision (18,2) for currency fields
- Decimal precision (5,2) for rates and percentages
- UTC timestamps for all date/time fields

## Business Logic

### Loan Eligibility Rules

- Minimum credit score: 650
- Maximum EMI-to-income ratio: 40%
- Down payment requirements vary by car type

### Interest Rates by Credit Score

- 800+: 8.5%
- 750-799: 9.5%
- 650-749: 11.5%
- Below 650: 15% (Not eligible)

### Loan Rules by Car Type

- **Hatchback**: 9.0% base rate, 10% min down payment
- **Electric/Hybrid**: 8.5% base rate with 1% green discount
- **Luxury Vehicles**: 9.5% base rate, 20% min down payment
- **Used Cars**: 11.5% base rate
- **Commercial**: 12.0% base rate, 25% min down payment

### EMI Calculation Formula

```
EMI = [P x R x (1+R)^N] / [(1+R)^N-1]
Where:
  P = Principal loan amount
  R = Monthly interest rate
  N = Number of months
```

## Development

### Running in Development Mode

```bash
dotnet run --environment Development
```

### Building for Production

```bash
dotnet build --configuration Release
dotnet publish --configuration Release
```

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add <MigrationName>
```

Update database:
```bash
dotnet ef database update
```

Rollback migration:
```bash
dotnet ef database update <PreviousMigrationName>
```

## Testing with Swagger

1. Start the application
2. Navigate to Swagger UI at `https://localhost:7XXX/swagger`
3. Use the `/api/auth/register` or `/api/auth/login` endpoint to get a token
4. Click the "Authorize" button and enter: `Bearer <your-token>`
5. You can now test all protected endpoints

## Configuration

### JWT Settings (appsettings.json)

```json
"Jwt": {
  "Secret": "YourSuperSecretKeyForJWT12345!@#$%",
  "ExpiryMinutes": 1440
}
```

### Logging

The application uses built-in ASP.NET Core logging. Logs are configured in `appsettings.json`.

## Error Handling

All API errors return a standardized response format:

```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Error detail 1", "Error detail 2"]
}
```

## Security Features

- Password hashing with BCrypt
- JWT token-based authentication
- HTTPS enforcement
- Input validation with data annotations
- SQL injection prevention via EF Core parameterized queries
- CORS restrictions

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the GitHub repository.

## API Response Format

All endpoints return responses in the following format:

### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* response data */ },
  "errors": null
}
```

### Error Response
```json
{
  "success": false,
  "message": "Operation failed",
  "data": null,
  "errors": ["Error message"]
}
```

## Additional Notes

- All currency values are in Indian Rupees (₹)
- All timestamps are stored in UTC
- EMI calculations match the frontend implementation
- Loan status automatically updates when all EMIs are paid
- Payment history maintains complete audit trail
