# SST-API
Streamlined Sales Tax developer API
---

This repository is the home for the Streamlined Sales Tax SDK where developers can view and test sample code for interacting with the SST website API.

There are 2 applications: Web Service Tool and Web Service Sample Code.
* If you only want to test the SST API then the Web Service Tool makes it easy for you to construct valid messages and inspect the results.
* If you want to write a .Net application in C# to automate or integrate the API into some other software and want to see sample code of how to get started then you will be interested in the Web Service Sample Code.



The API exposes the following functions: BulkRegistration, Registration, GetDocuments, GetTransmission, and AcknowledgeTransmission.  For CSP's who will be registering businesses on behalf of their clients they would use BulkRegistration.  For States to pick up the registrations they would use GetDocuments, GetTransmission and AcknowledgeTransmission.  


## BulkRegistration
BulkRegistration() is called by the CSP to register and/or update registration info for one registration or more in a single transmission to the SST system.
Documentation of BulkRegistration can be found [here](https://github.com/azavar/SST-API/blob/master/Bulk%20Registration%20ReadMe.md)

## GetDocuments

GetDocuments() will supply the requestor with a collection of registrations which match the filters that they supply.  Only States have permissions to call this function, and by default only the documents related to registrations in that state will be included in the response. Once calling GetDocuments, it creates a record of this request and may be re-requested via TransmissionId, see GetTransmission. For more information please check [this document](https://github.com/azavar/SST-API/blob/master/State%20web%20services%20description.docx)

````csharp
SSTRegistrationTransmissionType GetDocuments(string AcknowledgementStatus);
````
AcknowledgementStatus | Description
----------------------| -----------
U | Unacknowledged or Rejected
A | Acknowledged
R | Rejected

## GetTransmission

GetTransmission() is used to retransmit a transmission which had previously been requested in the call GetDocuments().  A transmission is a wrapper of registrations which were requested matching a filter in the call to GetDocuments().  Use of GetTransmission() is optional. For more information please check [this document](https://github.com/azavar/SST-API/blob/master/State%20web%20services%20description.docx)

````csharp
SSTRegistrationTransmissionType GetTransmission(string TransmissionId);
````

## AcknowledgeTransmission

AcknowledgeTransmission() allows a State to convey to SST the result of processing which has occurred at the States internal system.  After processing the registration at the States system, any errors which were identified may be submitted back to the system in the acknowledgement parameter.  Please see Schema SST2015V01 for details in constructing this object.

````csharp
SSTReceiptType AcknowledgeTransmission(SSTPAcknowledgementType acknowledgement);
````

