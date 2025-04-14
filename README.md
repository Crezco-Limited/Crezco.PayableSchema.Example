# Crezco Payable Schema Example

This project demonstrates how to consume and process JSON Schema in a C# application. The example focuses on retrieving and handling JSON Schema definitions for different country and currency combinations.

## Overview

The project consists of the following main components:

1. **CrezcoHttpMessageHandler**: A custom HTTP message handler that returns different JSON Schema based on the country and currency provided in the request.
2. **AuthorizationDelegatingHandler**: A delegating handler that adds an `Authorization` header with a Bearer token retrieved from the `CrezcoApiKey` environment variable.
3. **VersionDelegatingHandler**: A delegating handler that adds a `Crezco-Version` header to specify the API version being used.
4. **Program**: A console application to fetch and process the JSON Schema.

## Features

- Retrieve JSON Schema definitions for specific country and currency combinations.
- Extract and display required bank account properties from the JSON Schema.
- Extract and display possible purpose codes from the JSON Schema.
- Extract and display possible beneficiaries from the JSON Schema.

## Program Workflow

The console application performs the following steps:

1. Prompts the user to enter a country code and a currency code.
   - Supported combinations: `GB/GBP`, `FR/EUR`, `US/USD`, `MX/USD`.
3. Validates the response and reads the JSON content.
4. Extracts and prints the following details from the JSON Schema:
   - **Possible Beneficiaries**: Lists all possible beneficiaries defined in the schema.
   - **Required Bank Account Properties**: Displays the required properties for bank accounts.
   - **Possible Purpose Codes**: Displays purpose codes, indicating whether they are required and enabled.

## Example

```plaintext
Enter Country Code: FR
Enter Currency Code: EUR

** Possible Beneficiaries **
Individual
Company

** Bank Account Properties **
Regular
=============
country
accountCurrency
accountRoutingName
iban

Purpose Code is required:
** Possible Purpose Codes **
Const: Rent, Title: Rent Payment (Enabled: True)
Const: LoanRepayment, Title: Loan Repayment (Enabled: True)
Const: Donation, Title: Charitable Donation (Enabled: True)
Const: Gambling, Title: Gambling Payment (Enabled: False)
```
