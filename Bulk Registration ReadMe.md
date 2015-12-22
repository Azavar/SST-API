# BulkRegistration
BulkRegistration() is called by the CSP to register and/or update registration info for one registration or more in a single transmission to the SST system.  Please see Schema [SST2015V01](https://github.com/azavar/SST-API/tree/master/SST2015V01%20Schema) ([Source](http://www.statemef.com/projects/sst/SST2015V01.zip)) for further details about constructing this object.

````csharp
BulkRegistrationAcknowledgementType BulkRegistration(BulkRegistrationTransmissionType bulkRegistrationTransmission)
````
## Testing tool
A tool is provided [here](https://github.com/azavar/SST-API/tree/master/Web%20Service%20Tool%20(For%20Service%20Providers)) to test BulkRegistration

## General Rules
- BulkRegistration is only avaiable for service providers (CSPs and CASs)
- The caller is reponsible for generating a `TransmissionId`, which is a 20 character string defined as: service provider ID (9 characters) + Year(2 digits) + Julian Day(3 digits) + Sequence Number (6 alphanumeric)
- TransmissionId can't be reused
 
## Input
As defined in SST2015V01, BulkRegistration input is a Transmission that contains a number of Documents.
A document can convey one of several actions a service provider can apply for a registraion:
- [Create new registration (N)](#create-a-new-registration)
- Change an exsisting registration (C)
  - [Update business info](#update-business-info)
  - [Update registraion info for a state or more](#update-registraion-info-for-a-state-or-more)
  - [Start managing a registration](#start-managing-a-registration)
  - [End managing a registration](#end-managing-a-registration)
- [Out of business (O)](#out-of-business)
- [Unvolunteer/Unregister (U)](#unvolunteerunregister)

For all actions the tranmsmitter must already have authorization to manage the registration except when confirming to start managing. Newly created registrations will be automatically managable by the service provider who created them.

### Create A New Registration

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationNew` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegNew` and `<ActionCode>` set to `N`

`<TechnologyModel>` element will be ignored, but it is required for schema validation.

`<EffectiveDate>` will be ignored, but it is required for schema validation.
#### Create A New Registration - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationNew</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegNew</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationNew>
			<ActionCode>N</ActionCode>
	    	<RegistrationEntity>SP</RegistrationEntity>
			<IndividualName>
				<FirstName>First</FirstName>
				<LastName>Last</LastName>
			</IndividualName>
			<NAICSCode>154693</NAICSCode>
			<PhysicalAddress>
				<USAddress>
					<AddressLine1Txt>Address 1</AddressLine1Txt>
					<AddressLine2Txt>Address 2</AddressLine2Txt>
					<CityNm>Chicago</CityNm>
					<StateAbbreviationCd>IL</StateAbbreviationCd>
					<ZIPCd>60604</ZIPCd>
				</USAddress>
			</PhysicalAddress>
			<SellerPhone>1234567890</SellerPhone>
			<SSTPContact>
				<ContactName>
					<FirstName>Contact</FirstName>
					<LastName>Last Name</LastName>
				</ContactName>
				<ContactPhone>9876543210</ContactPhone>
				<ContactEmail>email@website.com</ContactEmail>
			</SSTPContact>
			<StateIncorporated >IL</StateIncorporated>
			<TechnologyModel>
				<None/>
			</TechnologyModel>
			<EffectiveDate>2015-07-31</EffectiveDate>
			<FirstFilingPeriod>2015-08</FirstFilingPeriod>
			<NewPass>qweras1234</NewPass>
		</BulkRegistrationNew>
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Update Business Info
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<BusinessInfo>` section

`<EffectiveDate>` will be ignored, but it is required for schema validation.
#### Update Business Info - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>C</ActionCode>
			<SSTPID>S00046251</SSTPID>
			<BusinessInfo>
				<DBAName>dba name</DBAName>
			</BusinessInfo>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````

### Update registraion info for a state or more
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<StateIndicators>` section (one for each state)

`<EffectiveDate>` will be ignored, but it is required for schema validation.
#### Update registraion info for a state or more - Example

````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>C</ActionCode>
			<SSTPID>S00046251</SSTPID>
			<StateIndicators>
        		<State>IL</State>
        		<RegistrationIndicator>R</RegistrationIndicator>
        		<FirstSaleDate>2015-10-01</FirstSaleDate>
        		<SSTPAllowanceIndicator>Y</SSTPAllowanceIndicator>
        		<FirstFilingPeriod>2015-10-01</FirstFilingPeriod>
      		</StateIndicators>
            <StateIndicators>
        		<State>ID</State>
        		<RegistrationIndicator>A</RegistrationIndicator>
        		<SSTPAllowanceIndicator>N</SSTPAllowanceIndicator>
      		</StateIndicators>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````

### Start Managing A Registration
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C`
`<BulkRegistrationCOU>` element must contain `<TechnologyModel>/<ModelOne>` with `CSPCode` attribute set to be the service provider Id, `<EffectiveDate>` will be used as the first filing period.
#### Start Managing A Registration - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>C</ActionCode>
			<SSTPID>S00046251</SSTPID>
			<TechnologyModel>
				<ModelOne CSPCode="CSP000099" />
			</TechnologyModel>
			<EffectiveDate>2015-10-01</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### End Managing A Registration
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C`
`<BulkRegistrationCOU>` element must contain `<TechnologyModel>/<None>`, `<EffectiveDate>` will be used as the last filing period.
#### End Managing A Registration - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>C</ActionCode>
			<SSTPID>S00046251</SSTPID>
			<TechnologyModel>
				<None />
			</TechnologyModel>
			<EffectiveDate>2015-10-31</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Out Of Business
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `O`, `<EffectiveDate>` will be used as the end registration date.
#### Out Of Business - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>O</ActionCode>
			<SSTPID>S00046251</SSTPID>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Unvolunteer/Unregister
This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `U`, `<EffectiveDate>` will be used as the end registration date.

`<StateIndicators>` section can be used to indicate if you want to keep the account open for a state by providing `<StateAcctInd>` as `Y`, the default value is `N` (don't keep the account open)
#### Unvolunteer/Unregister - Example
````xml
<BulkRegistrationTransmission xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" transmissionVersion="SST2015V01">
	<TransmissionHeader>
		<TransmissionId>CSP00009915123123456</TransmissionId>
        <DocumentCount>1</DocumentCount>
	</TransmissionHeader>
    <BulkRegistrationDocument>
		<DocumentId>CSP00009915123123457</DocumentId>
		<DocumentType>BulkRegistrationCOU</DocumentType>
   		<BulkRegistrationHeader>
			<ElectronicPostmark CSPID="CSP000099">2015-10-22</ElectronicPostmark>
    		<FilingType>BulkRegCOU</FilingType>
    		<TIN TypeTIN="SSN">
      			<FedTIN>001122334</FedTIN>
    		</TIN>
    	</BulkRegistrationHeader>
		<BulkRegistrationCOU>
			<ActionCode>U</ActionCode>
			<SSTPID>S00046251</SSTPID>
            <StateIndicators>
        		<State>AL</State>
        		<LastSaleDate>2015-11-30</LastSaleDate>
        		<StateAcctInd>N</StateAcctInd>
      		</StateIndicators>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
