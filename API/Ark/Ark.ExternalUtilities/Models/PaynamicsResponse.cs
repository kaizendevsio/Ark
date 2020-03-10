using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Ark.ExternalUtilities.Models
{
    [XmlRoot(ElementName = "application")]
    public class Application
    {
        [XmlElement(ElementName = "merchantid")]
        public string Merchantid { get; set; }
        [XmlElement(ElementName = "request_id")]
        public string Request_id { get; set; }
        [XmlElement(ElementName = "response_id")]
        public string Response_id { get; set; }
        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlElement(ElementName = "rebill_id")]
        public string Rebill_id { get; set; }
        [XmlElement(ElementName = "signature")]
        public string Signature { get; set; }
        [XmlElement(ElementName = "ptype")]
        public string Ptype { get; set; }
    }

    [XmlRoot(ElementName = "responseStatus")]
    public class ResponseStatus
    {
        [XmlElement(ElementName = "response_code")]
        public string Response_code { get; set; }
        [XmlElement(ElementName = "response_message")]
        public string Response_message { get; set; }
        [XmlElement(ElementName = "response_advise")]
        public string Response_advise { get; set; }
        [XmlElement(ElementName = "processor_response_id")]
        public string Processor_response_id { get; set; }
        [XmlElement(ElementName = "processor_response_authcode")]
        public string Processor_response_authcode { get; set; }
    }

    [XmlRoot(ElementName = "transactionHistory")]
    public class TransactionHistory
    {
        [XmlElement(ElementName = "transaction")]
        public string Transaction { get; set; }
    }

    [XmlRoot(ElementName = "ServiceResponseWPF")]
    public class PaynamicsResponse
    {
        [XmlElement(ElementName = "application")]
        public Application Application { get; set; }
        [XmlElement(ElementName = "responseStatus")]
        public ResponseStatus ResponseStatus { get; set; }
        [XmlElement(ElementName = "sub_data")]
        public string Sub_data { get; set; }
        [XmlElement(ElementName = "transactionHistory")]
        public TransactionHistory TransactionHistory { get; set; }
        [XmlElement(ElementName = "MetaData")]
        public string MetaData { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }


}
