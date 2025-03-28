using System.Dynamic;
using System.Net;
using System.Web;

namespace Crezco.PayableSchema;

public class CrezcoHttpMessageHandler : HttpMessageHandler
{
    private const string GbGbp = """
                                 {
                                   "$schema": "https://json-schema.org/draft/2020-12/schema",
                                   "type": "object",
                                   "properties": {
                                     "partnerEntityId": {
                                       "type": "string"
                                     },
                                     "recipientAmount": {
                                       "type": "object",
                                       "properties": {
                                         "currencyCode": {
                                           "type": "string"
                                         },
                                         "amountInMinorUnits": {
                                           "type": "integer"
                                         }
                                       },
                                       "required": [
                                         "currencyCode",
                                         "amountInMinorUnits"
                                       ]
                                     },
                                     "fees": {
                                       "type": "object"
                                     },
                                     "beneficiary": {
                                       "type": "object",
                                       "properties": {
                                         "address": {
                                           "$ref": "#/$defs/beneficiaryAddress"
                                         },
                                         "bankAccount": {
                                           "$ref": "#/$defs/bankAccount"
                                         }
                                       },
                                       "required": [
                                         "bankAccount"
                                       ]
                                     },
                                     "reference": {
                                       "type": "string"
                                     },
                                     "isLowPriority": {
                                       "type": "boolean"
                                     }
                                   },
                                   "required": [
                                     "partnerEntityId",
                                     "recipientAmount",
                                     "beneficiary",
                                     "reference"
                                   ],
                                   "$defs": {
                                     "beneficiaryAddress": {
                                       "type": [
                                         "object",
                                         "null"
                                       ],
                                       "properties": {
                                         "street": {
                                           "type": "string"
                                         },
                                         "city": {
                                           "type": "string"
                                         },
                                         "region": {
                                           "type": "string"
                                         },
                                         "country": {
                                           "type": "string"
                                         },
                                         "postalCode": {
                                           "type": "string"
                                         }
                                       },
                                       "required": []
                                     },
                                     "bankAccount": {
                                      "$id": "http://crezco.com/schemas/pay-run/payables/bank-account",
                                       "type": "object",
                                       "oneOf": [
                                         {
                                           "title": "Regular",
                                           "description": "Bank account details for a regular rail payment",
                                           "properties": {
                                             "country": {
                                               "type": "string"
                                             },
                                             "accountCurrency": {
                                               "type": "string"
                                             },
                                             "accountRoutingName": {
                                               "type": "string"
                                             },
                                             "gbSortCode": {
                                               "type": "string"
                                             },
                                             "gbAccountNumber": {
                                               "type": "string"
                                             }
                                           },
                                           "required": [
                                             "country",
                                             "accountCurrency",
                                             "accountRoutingName",
                                             "gbSortCode",
                                             "gbAccountNumber"
                                           ]
                                         },
                                         {
                                           "title": "Priority",
                                           "description": "Bank account details for a priority rail payment",
                                           "properties": {
                                             "country": {
                                               "type": "string"
                                             },
                                             "accountCurrency": {
                                               "type": "string"
                                             },
                                             "accountRoutingName": {
                                               "type": "string"
                                             },
                                             "iban": {
                                               "type": "string"
                                             },
                                             "swiftBic": {
                                               "type": "string"
                                             }
                                           },
                                           "required": [
                                             "country",
                                             "accountCurrency",
                                             "accountRoutingName",
                                             "iban",
                                             "swiftBic"
                                           ]
                                         }
                                       ]
                                     }
                                   }
                                 }
                                 """;

    private const string UsUSD = """
                                 {
                                   "$schema": "https://json-schema.org/draft/2020-12/schema",
                                   "type": "object",
                                   "properties": {
                                     "partnerEntityId": {
                                       "type": "string"
                                     },
                                     "recipientAmount": {
                                       "type": "object",
                                       "properties": {
                                         "currencyCode": {
                                           "type": "string"
                                         },
                                         "amountInMinorUnits": {
                                           "type": "integer"
                                         }
                                       },
                                       "required": [
                                         "currencyCode",
                                         "amountInMinorUnits"
                                       ]
                                     },
                                     "fees": {
                                       "type": "object"
                                     },
                                     "beneficiary": {
                                       "oneOf": [
                                         {
                                           "type": "object",
                                           "title": "Individual",
                                           "description": "Beneficiary details for an individual",
                                           "properties": {
                                             "$type": {
                                               "const": "Individual"
                                             },
                                             "firstName": {
                                               "type": "string"
                                             },
                                             "lastName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "firstName",
                                             "lastName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         },
                                         {
                                           "type": "object",
                                           "title": "Organisation",
                                           "description": "Beneficiary details for an organisation",
                                           "properties": {
                                             "$type": {
                                               "const": "Organisation"
                                             },
                                             "companyName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "companyName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         }
                                       ]
                                     },
                                     "reference": {
                                       "type": "string"
                                     },
                                     "isLowPriority": {
                                       "type": "boolean"
                                     }
                                   },
                                   "required": [
                                     "partnerEntityId",
                                     "recipientAmount",
                                     "beneficiary",
                                     "reference"
                                   ],
                                   "$defs": {
                                     "beneficiaryAddress": {
                                       "type": [
                                         "object"
                                       ],
                                       "properties": {
                                         "street": {
                                           "type": "string"
                                         },
                                         "city": {
                                           "type": "string"
                                         },
                                         "region": {
                                           "type": [
                                             "string",
                                             "null"
                                           ]
                                         },
                                         "country": {
                                           "type": "string"
                                         },
                                         "postalCode": {
                                           "type": "string"
                                         }
                                       },
                                       "required": [
                                         "street",
                                         "city",
                                         "country",
                                         "postalCode"
                                       ]
                                     },
                                     "bankAccount": {
                                       "$id": "http://crezco.com/schemas/pay-run/payables/bank-account",
                                       "type": "object",
                                       "oneOf": [
                                         {
                                           "title": "Regular",
                                           "properties": {
                                             "country": {
                                               "type": "string"
                                             },
                                             "accountCurrency": {
                                               "type": "string"
                                             },
                                             "accountRoutingName": {
                                               "type": "string"
                                             },
                                             "usAchAba": {
                                               "type": "string"
                                             },
                                             "usAccountNumber": {
                                               "type": "string"
                                             }
                                           },
                                           "required": [
                                             "country",
                                             "accountCurrency",
                                             "accountRoutingName",
                                             "usAchAba",
                                             "usAccountNumber"
                                           ]
                                         },
                                         {
                                           "title": "Priority",
                                           "properties": {
                                             "country": {
                                               "type": "string"
                                             },
                                             "accountCurrency": {
                                               "type": "string"
                                             },
                                             "accountRoutingName": {
                                               "type": "string"
                                             },
                                             "usAccountNumber": {
                                               "type": "string"
                                             },
                                             "swiftBic": {
                                               "type": "string"
                                             }
                                           },
                                           "required": [
                                             "country",
                                             "accountCurrency",
                                             "accountRoutingName",
                                             "usAccountNumber",
                                             "swiftBic"
                                           ]
                                         },
                                         {
                                           "title": "Priority",
                                           "properties": {
                                             "country": {
                                               "type": "string"
                                             },
                                             "accountCurrency": {
                                               "type": "string"
                                             },
                                             "accountRoutingName": {
                                               "type": "string"
                                             },
                                             "usFedwireAba": {
                                               "type": "string"
                                             },
                                             "usAccountNumber": {
                                               "type": "string"
                                             }
                                           },
                                           "required": [
                                             "country",
                                             "accountCurrency",
                                             "accountRoutingName",
                                             "usFedwireAba",
                                             "usAccountNumber"
                                           ]
                                         }
                                       ]
                                     }
                                   }
                                 }
                                 """;

    private const string FrEur = """
                                 {
                                   "$schema": "https://json-schema.org/draft/2020-12/schema",
                                   "type": "object",
                                   "properties": {
                                     "partnerEntityId": {
                                       "type": "string"
                                     },
                                     "recipientAmount": {
                                       "type": "object",
                                       "properties": {
                                         "currencyCode": {
                                           "type": "string"
                                         },
                                         "amountInMinorUnits": {
                                           "type": "integer"
                                         }
                                       },
                                       "required": [
                                         "currencyCode",
                                         "amountInMinorUnits"
                                       ]
                                     },
                                     "fees": {
                                       "type": "object"
                                     },
                                     "beneficiary": {
                                       "oneOf": [
                                         {
                                           "type": "object",
                                           "title": "Individual",
                                           "description": "Beneficiary details for an individual",
                                           "properties": {
                                             "$type": {
                                               "const": "Individual"
                                             },
                                             "firstName": {
                                               "type": "string"
                                             },
                                             "lastName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "firstName",
                                             "lastName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         },
                                         {
                                           "type": "object",
                                           "title": "Organisation",
                                           "description": "Beneficiary details for an organisation",
                                           "properties": {
                                             "$type": {
                                               "const": "Organisation"
                                             },
                                             "companyName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "companyName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         }
                                       ]
                                     },
                                     "reference": {
                                       "type": "string"
                                     },
                                     "isLowPriority": {
                                       "type": "boolean"
                                     }
                                   },
                                   "required": [
                                     "partnerEntityId",
                                     "recipientAmount",
                                     "beneficiary",
                                     "reference"
                                   ],
                                   "$defs": {
                                     "beneficiaryAddress": {
                                       "type": [
                                         "object"
                                       ],
                                       "properties": {
                                         "street": {
                                           "type": [
                                             "string",
                                             "null"
                                           ]
                                         },
                                         "city": {
                                           "type": [
                                             "string",
                                             "null"
                                           ]
                                         },
                                         "region": {
                                           "type": [
                                             "string",
                                             "null"
                                           ]
                                         },
                                         "country": {
                                           "type": "string"
                                         },
                                         "postalCode": {
                                           "type": [
                                             "string",
                                             "null"
                                           ]
                                         }
                                       },
                                       "required": [
                                         "country"
                                       ]
                                     },
                                     "bankAccount": {
                                       "$id": "http://crezco.com/schemas/pay-run/payables/bank-account",
                                       "type": "object",
                                       "title": "Regular",
                                       "description": "Bank account details for a regular rail payment",
                                       "properties": {
                                         "country": {
                                           "type": "string"
                                         },
                                         "accountCurrency": {
                                           "type": "string"
                                         },
                                         "accountRoutingName": {
                                           "type": "string"
                                         },
                                         "iban": {
                                           "type": "string"
                                         }
                                       },
                                       "required": [
                                         "country",
                                         "accountCurrency",
                                         "accountRoutingName",
                                         "iban"
                                       ]
                                     }
                                   }
                                 }
                                 """;

    private const string MxUSD = """
                                 {
                                   "$schema": "https://json-schema.org/draft/2020-12/schema",
                                   "type": "object",
                                   "properties": {
                                     "partnerEntityId": {
                                       "type": "string"
                                     },
                                     "recipientAmount": {
                                       "type": "object",
                                       "properties": {
                                         "currencyCode": {
                                           "type": "string"
                                         },
                                         "amountInMinorUnits": {
                                           "type": "integer"
                                         }
                                       },
                                       "required": [
                                         "currencyCode",
                                         "amountInMinorUnits"
                                       ]
                                     },
                                     "fees": {
                                       "type": "object"
                                     },
                                     "beneficiary": {
                                       "oneOf": [
                                         {
                                           "type": "object",
                                           "title": "Individual",
                                           "description": "Beneficiary details for an individual",
                                           "properties": {
                                             "$type": {
                                               "const": "Individual"
                                             },
                                             "firstName": {
                                               "type": "string"
                                             },
                                             "lastName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "firstName",
                                             "lastName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         },
                                         {
                                           "type": "object",
                                           "title": "Organisation",
                                           "description": "Beneficiary details for an organisation",
                                           "properties": {
                                             "$type": {
                                               "const": "Organisation"
                                             },
                                             "companyName": {
                                               "type": "string"
                                             },
                                             "address": {
                                               "$ref": "#/$defs/beneficiaryAddress"
                                             },
                                             "bankAccount": {
                                               "$ref": "#/$defs/bankAccount"
                                             }
                                           },
                                           "required": [
                                             "$type",
                                             "companyName",
                                             "address",
                                             "bankAccount"
                                           ]
                                         }
                                       ]
                                     },
                                     "reference": {
                                       "type": "string"
                                     },
                                     "isLowPriority": {
                                       "type": "boolean"
                                     },
                                     "purposeCode": {
                                       "$ref": "#/$defs/purposeCode"
                                     }
                                   },
                                   "required": [
                                     "partnerEntityId",
                                     "recipientAmount",
                                     "beneficiary",
                                     "reference",
                                     "purposeCode"
                                   ],
                                   "$defs": {
                                     "beneficiaryAddress": {
                                       "type": [
                                         "object"
                                       ],
                                       "properties": {
                                         "street": {
                                           "type": "string"
                                         },
                                         "city": {
                                           "type": "string"
                                         },
                                         "region": {
                                           "type": "string"
                                         },
                                         "country": {
                                           "type": "string"
                                         },
                                         "postalCode": {
                                           "type": "string"
                                         }
                                       },
                                       "required": [
                                         "street",
                                         "city",
                                         "region",
                                         "country",
                                         "postalCode"
                                       ]
                                     },
                                     "bankAccount": {
                                       "$id": "http://crezco.com/schemas/pay-run/payables/bank-account",
                                       "type": "object",
                                       "title": "Priority",
                                       "description": "Bank account details for a priority rail payment",
                                       "properties": {
                                         "country": {
                                           "type": "string"
                                         },
                                         "accountCurrency": {
                                           "type": "string"
                                         },
                                         "accountRoutingName": {
                                           "type": "string"
                                         },
                                         "swiftBic": {
                                           "type": "string"
                                         },
                                         "mxClabe": {
                                           "type": "string"
                                         }
                                       },
                                       "required": [
                                         "country",
                                         "accountCurrency",
                                         "accountRoutingName",
                                         "swiftBic",
                                         "mxClabe"
                                       ]
                                     },
                                     "purposeCode": {
                                       "title": "Payment Purpose",
                                       "description": "Payment Purpose codes",
                                       "oneOf": [
                                         { "const": "Salary", "title": "Salary Payment", "enabled": false },
                                         { "const": "Rent", "title": "Rent Payment", "enabled": true },
                                         { "const": "LoanRepayment", "title": "Loan Repayment", "enabled": true },
                                         { "const": "Donation", "title": "Charitable Donation", "enabled": true }
                                       ]
                                     }
                                   }
                                 }
                                 """;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri?.PathAndQuery.Contains("/pay-run/payables/schema") == false)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        // Get Query string from request and check country code
        var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
        var country = query["country"];
        var currency = query["currency"];

        return Task.FromResult(CreateHttpResponseMessage(country, currency));
    }

    private static HttpResponseMessage CreateHttpResponseMessage(string? country, string? currency)
    {
        if (country == "US" && currency == "USD")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(UsUSD)
            };
        if (country == "GB" && currency == "GBP")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GbGbp)
            };

        if (country == "FR" && currency == "EUR")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(FrEur)
            };

        if (country == "MX" && currency == "USD")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(MxUSD)
            };

        return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }
}