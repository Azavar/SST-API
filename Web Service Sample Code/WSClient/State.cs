using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using WSClient.ServiceReference1;

namespace WSClient
{

    /// <summary>
    /// Sample code to demonstrate using the SSTGB API to retrieve state registrations and acknowledging them.
    /// </summary>
    public static class State
    {
        public static void Test()
        {
            using (var client = new ServiceReference1.ApiServiceClient())
            {
                //Specify connection credentials 
                //(if used in production we recommend not storing your user/pass in the web.config)
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SSTUser"];
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SSTPass"];

                try
                {
                    Console.WriteLine("Calling API GetDocuments(..)");

                    //Make web service call to request unacknowledged registrations.
                    var transmission = client.GetDocuments("U", "", "");


                    var registrations = new List<InternalRegistration>();
                    foreach (var doc in transmission.RegistrationDocument)
                    {
                        //Transform the schema to another structure for validation
                        var myRegistration = new InternalRegistration();
                        myRegistration.SSTID = doc.SSTRegistrationHeader.SSTPID;
                        myRegistration.TIN = doc.SSTRegistrationHeader.TIN.FedTIN;
                        myRegistration.TINType = doc.SSTRegistrationHeader.TIN.TypeTIN == TINTypeTypeTIN.SSN ? "SSN" : doc.SSTRegistrationHeader.TIN.TypeTIN == TINTypeTypeTIN.FEIN ? "FEIN" : "FOREIGN";
                        myRegistration.DocumentId = doc.DocumentId;

                        ReadRegistrationType(doc.RegistrationInformation.Item, myRegistration);
                        registrations.Add(myRegistration);
                        

                        //Do something with your internal representation e.g. we will perform some validation and then acknowledge it.
                        Console.WriteLine("Recieved {0} registration for [{1}] {2} in document {3}", 
                            doc.DocumentType==RegistrationDocumentTypeDocumentType.SSTRegistrationNew?"new":"changed",
                            myRegistration.SSTID,
                            myRegistration.BusinessNameLine1,
                            myRegistration.DocumentId
                            );

                    }


                    //Acknowledge receipt of one ore more documents
                    var ackMessage = new SSTPAcknowledgementType
                        {
                            AcknowledgementHeader = new AcknowledgementHeaderType()
                                {
                                    AcknowledgementCount = registrations.Count.ToString(),
                                    AcknowledgementTimestamp = DateTime.UtcNow
                                },
                            TransmissionAcknowledgement = new TransmissionAcknowledgementType()
                                {
                                    TransmissionId = transmission.TransmissionHeader.TransmissionId,
                                    TransmissionStatus = StatusType.A,
                                    TransmissionTimestamp = DateTime.UtcNow
                                },
                            DocumentAcknowledgement = new DocumentAcknowledgementType[registrations.Count]
                        };
                    
                    int i = 0;
                    foreach (var doc in registrations)
                    {
                        var ack = new DocumentAcknowledgementType
                                      {
                                          DocumentId = doc.DocumentId, 
                                          DocumentStatus = StatusType.A, 
                                          DocumentType = DocumentAcknowledgementTypeDocumentType.SSTPRegistration
                                      };

                        //Relay any errors to service
                        //ack.Errors.Error = new ErrorsError[1];
                        //ack.Errors.Error[0].ErrorMessage = "A descriptive error";
                        //ack.Errors.errorCount = "1";

                        ackMessage.DocumentAcknowledgement[i] = ack;

                        i++;
                    }

                    //Submit the acknowledgement
                    client.AcknowledgeTransmission(ackMessage);


                } catch (MessageSecurityException ex)
                {
                    Console.WriteLine("Invalid login credentials or security certificate.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                    //Handle errors appropriately
                }
            }
        }



        private static void ReadRegistrationType(object registrationType, InternalRegistration i)
        {
            if (registrationType.GetType() == typeof (RegistrationNewType))
            {
                var reg = (RegistrationNewType) registrationType;
                i.DBA = reg.DBAName;
                ReadNameType(reg.Item, i);
                ReadAddressType(reg.MailingAddress, i);

            } else if (registrationType.GetType() == typeof (RegistrationCOUType))
            {
                var reg = (RegistrationCOUType) registrationType;
                i.DBA = reg.DBAName;
                ReadNameType(reg.Item, i);
                
            }
        }

        private static void ReadNameType(object nameType, InternalRegistration i)
        {
            if (nameType.GetType() == typeof (IndividualNameType))
            {
                var name = (IndividualNameType) nameType;
                i.First = name.FirstName;
                i.Middle = name.MiddleInitial;
                i.Last = name.LastName;
            }
            else if (nameType.GetType() == typeof(BusinessNameType))
            {
                var name = (BusinessNameType) nameType;
                i.BusinessNameLine1 = name.BusinessNameLine1Txt;
                i.BusinessNameLine2 = name.BusinessNameLine2Txt;
            }
        }

        private static void ReadAddressType(object addresstype, InternalRegistration i)
        {
            if (addresstype.GetType() == typeof (AddressTypeUSAddress))
            {
                var address = (AddressTypeUSAddress) addresstype;
                i.MailingAddressLine1 = address.AddressLine1Txt;
                i.MailingAddressLine2 = address.AddressLine2Txt;
                i.MailingCity = address.CityNm;
                i.MailingStateProvince = address.StateAbbreviationCd.ToString();
                i.MailingPostalCode = address.ZIPCd;
                i.MailingCountry = "US";

            } else if (addresstype.GetType() == typeof (AddressTypeForeignAddress))
            {
                var address = (AddressTypeForeignAddress) addresstype;
                i.MailingAddressLine1 = address.AddressLine1Txt;
                i.MailingAddressLine2 = address.AddressLine2Txt;
                i.MailingCity = address.CityNm;
                i.MailingStateProvince = address.ProvinceOrStateNm;
                i.MailingPostalCode = address.ForeignPostalCd;
                i.MailingCountry = address.CountryCd.ToString();
            }
        }


    }
}
