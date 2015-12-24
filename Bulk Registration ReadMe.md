# BulkRegistration
BulkRegistration() is called by the CSP to register and/or update registration info for one registration or more in a single transmission to the SST system.  Please see Schema [SST2015V01](https://github.com/azavar/SST-API/tree/master/SST2015V01%20Schema) ([Source](http://www.statemef.com/projects/sst/SST2015V01.zip)) for further details about constructing this object.

````csharp
BulkRegistrationAcknowledgementType BulkRegistration(BulkRegistrationTransmissionType bulkRegistrationTransmission)
````
## Testing tool
A tool is provided [here](https://github.com/azavar/SST-API/tree/master/Web%20Service%20Tool%20(For%20Service%20Providers)) to test BulkRegistration

## General Rules
- BulkRegistration is only avaiable for service providers (CSPs and CASs)
- The caller (service provider) is reponsible for generating a `TransmissionId` and a `DocumentId` (for each document), which is a 20 character string defined as: service provider ID (9 characters) + Year(2 digits) + Julian Day(3 digits) + Sequence Number (6 alphanumeric)
- TransmissionId/DocumentId can't be reused
- `<EffectiveDate>` is required and must always be today's date.
 
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

For all actions the tranmsmitter must already have authorization to manage the registration except when requesting to start managing. Newly created registrations will be automatically managable by the service provider who created them.

### Create A New Registration
A service provider can create an account on the taxpayer's behalf, the new account will start as ModelOne (or ModelTwo in case of CAS), FirstFilingPeriod must be provided in `<BulkRegistrationNew>/<FirstFilingPeriod>` 

This action can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationNew` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegNew` and `<ActionCode>` set to `N`

#### Rules
- `<TechnologyModel>` element will be ignored, but it is required for schema validation. The created account will be ModelOne or ModelTwo (depending on the service provider identity).
- `<StateRegistrationIndicator>/<RemoteSellerID>` is required for schema validation and it must be `N`.
- State-level data can be provided using `<StateRegistrationIndicator>` element (one for each state), when a state is left out then if it is a member state the `<RegistrationIndicator>` will default to `R`, otherwise the default is `N`
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
            <StateRegistrationIndicator>
        		<State>IL</State>
        		<RegistrationIndicator>R</RegistrationIndicator>
        		<FirstSalesDate>2015-10-01</FirstSalesDate>
                <RemoteSellerID>N</RemoteSellerID>
        		<SSTPAllowanceIndicator>Y</SSTPAllowanceIndicator>
      		</StateRegistrationIndicator>
			<EffectiveDate>2015-07-31</EffectiveDate>
			<FirstFilingPeriod>2015-08</FirstFilingPeriod>
			<NewPass>qweras1234</NewPass>
		</BulkRegistrationNew>
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Update Business Info
Changes business information for an already registered taxpayer, the service provider sending this request must be the one who created the account or have successfully submitted a request to manage this account.

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<BusinessInfo>` section

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
Changes registration info for a state or more for an already registered taxpayer, the service provider sending this request must be the one who created the account or have successfully submitted a request to manage this account.

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<StateIndicators>` section (one for each state)

#### Rules
- A single document can't contain 2 `<StateIndicators>` with the same state
- `<FirstFilingPeriod>` must be the first day of a month and must have the same value for all member states, for associate and non-SST states it must not be provided (null)
- `<StateIndicators>/<RegistrationIndicator>` can't be `U` for member states
- `<StateIndicators>/<LastSaleDate>` value will be used only if `<StateIndicators>/<RegistrationIndicator>` is set to `U` (for associate and non-SST states) (when `<ActionCode>` is `C`)
- `<StateIndicators>/<StateAcctInd>` and `<StateIndicators>/<AcctCloseDate>` must be null (when `<ActionCode>` is `C`), please see [Out Of Business](#out-of-business) and [Unvolunteer/Unregister](#unvolunteerunregister) for their proper use
- `<StateIndicators>/<RemoteSellerID>`, `<StateIndicators>/<RemoteEffDate>` and `<StateIndicators>/<RemoteEndDate>` will be ignored
- For this use case (updating registration info for a state or more) `<StateIndicators>/<CSPEndDate>` and `<StateIndicators>/<CSPLastFilingPd>` must be null, please see [End Managing A Registration](#end-managing-a-registration) for their proper use.
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
        		<FirstSalesDate>2015-10-01</FirstSalesDate>
        		<SSTPAllowanceIndicator>Y</SSTPAllowanceIndicator>
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
A service provider can send this request to start managing a taxpayer already registered but is not currently using any service provider, this can happen if the taxpayer registered themselves or is changing service providers (after the last service provider last filing period has passed)

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<TechnologyModel>/<ModelOne>` with `CSPCode` attribute set to be the CSP Id (or `<TechnologyModel>/<ModelTwo>` with `CASCode` attribute set to be the CAS Id).

#### Rules
- **The service provider will need to send another BulkCOU request to set the `<FirstFilingPeriod>`, see [Update registraion info for a state or more](#update-registraion-info-for-a-state-or-more).**
- CSPCode (or CASCode) must match the Id or the calling service provider.
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
A service provider can release a taxpayer by providing a `CSPLastFilingPd` for all states where the taxpayer is registered, the service provider sending this request must be currently managing this account.

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `C` and using `<StateIndicators>` section (one for each state).
#### Rules
-  All states where the taxpayer is registered (all member states + possibly other states) must have a `<StateIndicators>`element all with the same `<StateIndicators>/<CSPLastFilingPd>` and `<StateIndicators>/<CSPEndDate>`.
- If `<StateIndicators>/<CSPEndDate>` is not provided it will default to the last day of month for `<StateIndicators>/<CSPLastFilingPd>`.
- By `<StateIndicators>/<CSPEndDate>` the taxpayer TecnologyModel will be changed to None (Model 4) and the privious service provider will not be able to manage the account.
- **Don't use `<TechnologyModel>/<None>` to end managing a registration, this is not supported.**
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
            <StateIndicators>
        		<State>AK</State>
                <CSPEndDate>2015-10-31</CSPEndDate>
                <CSPLastFilingPd>2015-10</CSPLastFilingPd>
      		</StateIndicators>
            ...
            <StateIndicators>
        		<State>WY</State>
                <CSPEndDate>2015-10-31</CSPEndDate>
                <CSPLastFilingPd>2015-10</CSPLastFilingPd>
      		</StateIndicators>
			<EffectiveDate>2015-09-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Out Of Business
This will close the account and flag it as out of business, the service provider sending this request must be the one who created the account or have successfully submitted a request to manage this account. After going out of business this account will be inaccessible.

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `O`, `<StateIndicators>/<AcctCloseDate>` will be used as the end registration date.
#### Rules
- All states where the taxpayer is registered (all member states + possibly other states) must have a `<StateIndicators>`element all with the same `<StateIndicators>/<AcctCloseDate>`.
- `<StateIndicators>/<StateAcctInd>` will be ignored if provided, please see [Unvolunteer/Unregister](#unvolunteerunregister) for their proper use.
- All other fields (besides `<StateIndicators>/<AcctCloseDate>` and `<StateIndicators>/<State>`) will be ignored if provided.
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
            <StateIndicators>
        		<State>AK</State>
                <AcctCloseDate>2015-10-31</AcctCloseDate>
      		</StateIndicators>
            ...
            <StateIndicators>
        		<State>WY</State>
                <AcctCloseDate>2015-10-31</AcctCloseDate>
      		</StateIndicators>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
### Unvolunteer/Unregister
This will close the account and flag states where the account should be kept open, the service provider sending this request must be the one who created the account or have successfully submitted a request to manage this account. After unvoluneering this account will be inaccessible.

This can be done by sending a `<BulkRegistrationDocument>` with `<DocumentType>` set to `BulkRegistrationCOU` and `<BulkRegistrationHeader>/<FilingType>` set to `BulkRegCOU` and `<ActionCode>` set to `U`, `<StateIndicators>/<AcctCloseDate>` will be used as the end registration date.
#### Rules
- All states where the taxpayer is registered (all member states + possibly other states) must have a `<StateIndicators>`element all with the same `<StateIndicators>/<AcctCloseDate>`.
- `<StateIndicators>/<StateAcctInd>` can be set to `Y` to indicate the account should be kept open a state, the default value is `N` (don't keep the account open).
- All other fields (besides `<StateIndicators>/<AcctCloseDate>`, `<StateIndicators>/<StateAcctInd>` and `<StateIndicators>/<State>`) will be ignored if provided.

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
        		<State>AK</State>
                <AcctCloseDate>2015-10-31</AcctCloseDate>
                <StateAcctInd>Y</StateAcctInd>
      		</StateIndicators>
            ...
            <StateIndicators>
        		<State>WY</State>
                <AcctCloseDate>2015-10-31</AcctCloseDate>
      		</StateIndicators>
			<EffectiveDate>2015-10-22</EffectiveDate>
		</BulkRegistrationCOU>    
	</BulkRegistrationDocument>
</BulkRegistrationTransmission>
````
