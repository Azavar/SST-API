using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSClient.ServiceReference1;

namespace WSClient
{
    /// <summary>
    /// Code sample for constructing a valid SST message to construct a bulk registration message.
    /// </summary>
    public static class Registration
    {
        public static void Test()
        {
            //Instanciate a web services client
            var client = new ServiceReference1.ApiServiceClient();

            //Specify connection credentials 
            //(if used in production we recommend not storing your user/pass in the web.config)
            client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SSTUser"];
            client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SSTPass"];


            //Construct a transmission, to wrap one or more bulk registrations.
            //Please see the SST2015V01 Schema for documentation about constructing this message
            //http://www.statemef.com/projects/sst/SST2015V01.zip

            var transmission = new BulkRegistrationTransmissionType();
            transmission.TransmissionHeader = new TransmissionHeaderType()
            {
                DocumentCount = "1",
                Timestamp = DateTime.Now,

                //You should increment transmission each time.  Do not submit duplicate transmissionids
                TransmissionId = "CSP00001615200000037",
            };

            transmission.BulkRegistrationDocument = new BulkRegistrationDocumentType[1];
            transmission.BulkRegistrationDocument[0] = new BulkRegistrationDocumentType()
            {
                BulkRegistrationHeader = new BulkRegistrationHeaderType()
                {
                    DateReceived = new DateTime(),
                    FilingType = BulkRegistrationHeaderTypeFilingType.BulkRegNew
                },
                DocumentId = "1234",
                DocumentType = BulkRegistrationDocumentTypeDocumentType.BulkRegistrationNew,
            };

            var registrationNew = new BulkRegistrationNewType()
                                      {
                                          ActionCode = BulkRegistrationNewTypeActionCode.N,
                                          Item = "Acme Inc.",
                                          MailingAddress = new AddressType()
                                                               {
                                                                   Item = new AddressTypeUSAddress()
                                                                              {
                                                                                  AddressLine1Txt = "P.O. Box 9876",
                                                                                  CityNm = "Chicago",
                                                                                  StateAbbreviationCd = (StateType)Enum.Parse(typeof(StateType), "IL", true),
                                                                                  ZIPCd = "60603"
                                                                              }
                                                               },
                                          PhysicalAddress = new AddressType()
                                                               {
                                                                   Item = new AddressTypeUSAddress()
                                                                              {
                                                                                  AddressLine1Txt = "1234 Main Street",
                                                                                  CityNm = "Chicago",
                                                                                  StateAbbreviationCd = (StateType)Enum.Parse(typeof(StateType), "IL", true),
                                                                                  ZIPCd = "60604"
                                                                              }
                                                               },

                                      };
            transmission.BulkRegistrationDocument[0].Item = registrationNew;


            try
            {
                //Transmit the bulk registration message
                var receipt = client.BulkRegistration(transmission);
                if (receipt != null)
                {
                    if (receipt.TransmissionAcknowledgement.TransmissionStatus == StatusType.R)
                    {
                        Console.WriteLine("Transmission was rejected, {0} errors:", receipt.TransmissionAcknowledgement.Errors.errorCount);
                        foreach (var error in receipt.TransmissionAcknowledgement.Errors.Error)
                        {
                            Console.WriteLine(GenerateMessage(error));
                        }
                    }
                    else
                    {
                        foreach (var ack in receipt.BulkRegAcknowledgement)
                        {
                            var errors = ack.Errors;
                            Console.WriteLine("There were {0} errors detected", errors.errorCount);
                            foreach (var error in ack.Errors.Error)
                            {
                                Console.WriteLine(GenerateMessage(error));
                            }
                        }
                    }

                    //Store the new SSTPID that was provided to you.
                    //ack.SSTPID
                }
                else
                {
                    Console.WriteLine("Success");
                }

                //Console.WriteLine("Received Transmission Status: {0}", receipt.);

            }
            catch (TimeoutException toException)
            {
                //The application timed out waiting for a response.
                Console.Error.WriteLine(toException.Message);
                client.Abort();
            }
            catch (Exception ex)
            {
                //Generally this would mean a login/password error, or failure to authenticate or establish certificate trust.
                Console.Error.WriteLine(ex.Message);
                client.Abort();
                throw;
            }

            client.Close();
        }

        private static string GenerateMessage(ErrorsError error)
        {
            var msg = string.Format("Error Code: {0}, Error Message: \"{1}\" in {2} ({3})", error.errorId, error.ErrorMessage, error.Item, error.ItemElementName);
            if (!string.IsNullOrEmpty(error.AdditionalErrorMessage))
                msg += ", Additional Error Message: " + error.AdditionalErrorMessage;
            if (!string.IsNullOrEmpty(error.DataValue))
                msg += ", Data Value: " + error.DataValue;
            return msg;
        }
    }
}
