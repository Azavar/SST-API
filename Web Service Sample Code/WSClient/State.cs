using System;
using System.Configuration;
using System.ServiceModel.Security;
using WSClient.ServiceReference1;

namespace WSClient
{

    /// <summary>
    /// Sample code to demonstrate using the SSTGB API to retrieve state registrations and acknowledging them.
    /// </summary>
    public static class State
    {

        public static void TestGetDocuments()
        {
            using (var client = new ApiServiceClient())
            {
                // (if used in production we recommend not storing your user/pass in the web.config)
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SSTUser"];
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SSTPass"];

                try
                {
                    Console.WriteLine("Calling API GetDocuments(\"U\")");

                    // Make web service call to request unacknowledged registrations.
                    var transmission = client.GetDocuments("U");

                    /*
                    
                    // Write result xml to a file
                    var xmlWriterSettings = new XmlWriterSettings { Encoding = new UTF8Encoding(false) };
                    var xmlserializer = new XmlSerializer(transmission.GetType());
                    string xml;
                    using (var stringWriter = new StringWriter())
                    {
                        using (var writer = XmlWriter.Create(stringWriter, xmlWriterSettings))
                        {
                            xmlserializer.Serialize(writer, transmission);
                            xml = stringWriter.ToString();
                        }
                    }
                    File.WriteAllText(path,xml);
                    */
                }
                catch (MessageSecurityException)
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

        public static void TestGetTransmission()
        {
            using (var client = new ApiServiceClient())
            {
                //(if used in production we recommend not storing your user/pass in the web.config)
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SSTUser"];
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SSTPass"];

                try
                {
                    var transmissionId = ConfigurationManager.AppSettings["TransmissionId"];
                    Console.WriteLine("Calling API GetTransmission(\"{0}\")", transmissionId);

                    var transmission = client.GetTransmission(transmissionId);
                    
                }
                catch (MessageSecurityException)
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

        public static void TestAcknowledgeTransmission()
        {
            using (var client = new ApiServiceClient())
            {
                // (if used in production we recommend not storing your user/pass in the web.config)
                client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SSTUser"];
                client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SSTPass"];

                try
                {
                    const string transmissionId = ""; // please provide valid TransmissionId
                    Console.WriteLine("Calling API AcknowledgeTransmission(\"{0}\")", transmissionId);

                    // prepare acknowledgement object
                    var ack = new SSTPAcknowledgementType
                    {
                        AcknowledgementHeader = new AcknowledgementHeaderType
                        {
                            AcknowledgementCount = "1",
                            AcknowledgementTimestamp = DateTime.UtcNow
                        },
                        DocumentAcknowledgement = new[]
                        {
                            new DocumentAcknowledgementType
                            {
                                DocumentId = "", // please provide a valid DocumentId
                                DocumentStatus = StatusType.R,
                                Errors = new Errors
                                {
                                    Error = new []
                                    {
                                        new Error
                                        {
                                            errorId = "1000",
                                            ErrorMessage = "Test Message"
                                        },
                                        new Error
                                        {
                                            errorId = "2000",
                                            ErrorMessage = "Test Message2",
                                            AdditionalErrorMessage = "AdditionalErrorMessage",
                                            Item = "FiledName",
                                            ItemElementName = ItemChoiceType.FieldIdentifier,
                                            DataValue = "Value",
                                            Severity = "Critical"
                                        }
                                    },
                                    errorCount = "2"
                                }
                            }
                        },
                        TransmissionAcknowledgement = new TransmissionAcknowledgementType
                        {
                            TransmissionId = "", // please provide valid TransmissionId
                            TransmissionStatus = StatusType.A
                        },
                        acknowledgementVersion = "SST2015V01"
                    };

                    var receipt = client.AcknowledgeTransmission(ack);
                   
                }
                catch (MessageSecurityException)
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
    }
}
