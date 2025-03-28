# Crezco Payable Schema Example

This project demonstrates how to consume and process JSON Schema in a C# application. The example focuses on retrieving and handling JSON Schema definitions for different country and currency combinations.

## Overview

The project consists of two main components:

1. **CrezcoHttpMessageHandler**: A custom HTTP message handler that returns different JSON Schema based on the country and currency provided in the request.
2. **Program**: A console application that interacts with the `CrezcoHttpMessageHandler` to fetch and process the JSON Schema.

## Program

The console application performs the following steps:

1. Prompts the user to enter a country code and a currency code.
   - Supported country and currency code combinations : `GB/GBP`, `FR/EUR`, `US/USD`, `MX/USD`.
2. Sends an HTTP GET request to the `CrezcoHttpMessageHandler` with the provided country and currency codes.
3. Ensures the response is successful and reads the JSON content.
4. Extracts and prints the required bank account properties from the JSON Schema.
5. Extracts and prints the possible purpose codes from the JSON Schema.

## Usage

1. Run the console application.
2. Enter the country code and currency code when prompted.
3. The application will display the required bank account properties and possible purpose codes based on the provided inputs.

## Example

```plaintext
Enter Country Code: FR
Enter Currency Code: EUR

** Bank Account Properties **
Regular
=============
country
accountCurrency
accountRoutingName
iban

Purpose Code is required:
- Rent Payment (Rent) (Enabled)
- Loan Repayment (LoanRepayment) (Enabled)
- Charitable Donation (Donation) (Enabled)
```

This example demonstrates how to consume and process JSON Schema in a C# application. The provided code can be extended or modified to handle different JSON Schema structures and requirements.