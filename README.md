## Technical Task - Payment Gateway

### Problem space and requirements
https://github.com/cko-recruitment/#requirements

### Key design considerations and assumptions
1. Applied clean architecture principles for maintainability and a domain-focused application.
2. Value objects are used to serve as a single source of truth for validation. 
3. The Banking Simulator API for payment creation is idempotent and handled by the simulator due to lack of explicit fields in the API.
4. Payments are processed synchronously without persistent storage or messaging for simplicity, assuming no network failures or concurrent requests.
5. MerchantId is passed as a header to uniquely identify clients.
6. The RESTful API is designed without versioning around the payment resource for simplicity.
7. Comprehensive authentication and authorization layers are omitted for simplicity.
8. The test suite addresses key risk areas, including API endpoint functionality, validation, 
security checks for exposing more than four digits of card numbers, and ensuring merchants cannot access payments that do not belong to them.

### Production Readiness Action Items
1. Ensure exactly-once processing by prioritizing consistency over availability.
2. Enhance durability by implementing real persistent storage.
3. Implement robust authentication and authorization mechanisms.
4. Encrypt sensitive card data in persistent storage (encryption at rest).
5. Enable encryption in transit for all communication: (Merchant Service → API Gateway) and (API Gateway → Bank API).
6. Mask sensitive data in logs to prevent exposing card details.
7. Confirm compliance with legal and regulatory requirements, including GDPR, Data Privacy, PCI DSS, and SCA.
8. Add basics (logging, metrics) of observability to the Payment Gateway.

### Installation
The Payment Gateway is built with .NET 8.

1. Install .NET 8 SDK
Ensure that the .NET 8 SDK is installed on your machine. You can download and install it from the official [.NET website.](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

2. To verify the installation, open a terminal and run:
```dotnet --version```
You should see the version of the .NET SDK installed (e.g., 8.x.x).

3. Clone or Download the Application
Obtain the application source code, and then navigate to its directory.

4. To install Docker on Windows, download and install Docker Desktop from the official website, and on Linux, 
install Docker Engine using your distribution’s package manager and Docker's official repository.

### Running
1. Open a terminal and navigate to the root of the application directory.
2. Run the following command to remove any existing containers
```docker-compose down```
3. Start the BankSimulator in detached mode with
```docker-compose up -d```
4. Open a terminal and navigate to the application directory
```cd src/PaymentGateway.WebApi```
5. Run the following command to build the application
```dotnet build```
6. Start the application 
```dotnet run```
7. Open the following URL in your browser ```http://localhost:5145```

### Testing
1. Open a terminal and move to the application directory
2. Run the following command to restore dependencies
```dotnet restore```
3. Run all the tests
```dotnet test```