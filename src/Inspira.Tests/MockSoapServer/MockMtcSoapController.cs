using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Linq;

namespace Inspira.Tests.MockSoapServer;

[ApiController]
[Route("mockmtc")]
public class MockMtcSoapController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] string body)
    {
        // simple parse to get ssn and role
        try
        {
            var xml = XDocument.Parse(body);
            XNamespace tem = "http://tempuri.org/";
            var ssnElem = xml.Descendants().FirstOrDefault(e => e.Name.LocalName == "ssn");
            var roleElem = xml.Descendants().FirstOrDefault(e => e.Name.LocalName == "role");
            var ssn = ssnElem?.Value ?? string.Empty;
            var role = roleElem?.Value ?? "0";

            // produce different responses based on ssn
            if (ssn == "000000000")
            {
                var fault = @"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><s:Fault><faultcode>s:Client</faultcode><faultstring>Exception at: ProcessContactGetResults Message: Multiple active contact master records found: for ssn XXX-XX-0000 returned Resolution:</faultstring></s:Fault></s:Body></s:Envelope>";
                return Content(fault, "text/xml");
            }

            if (ssn == "123456789")
            {
                var fault = @"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><s:Fault><faultcode>s:Client</faultcode><faultstring>Exception at: ProcessContactGetResults Message: ContactGet for: ssn XXX-XX-6789 returned 829 records Resolution:</faultstring></s:Fault></s:Body></s:Envelope>";
                return Content(fault, "text/xml");
            }

            int contactId = ssn switch
            {
                "495657532" => 6753148,
                "495657533" => 6753149,
                _ => ssn.Length > 0 ? ssn.GetHashCode() & 0x7fffffff : 0
            };

            var resp = $@"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><SSNInternalCheckResponse xmlns=\"http://tempuri.org/\"><SSNInternalCheckResult>{contactId}</SSNInternalCheckResult></SSNInternalCheckResponse></s:Body></s:Envelope>";

            return Content(resp, "text/xml");
        }
        catch
        {
            return BadRequest();
        }
    }
}
