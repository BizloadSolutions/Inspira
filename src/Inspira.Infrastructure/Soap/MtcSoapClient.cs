using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Inspira.Application.Services;

namespace Inspira.Infrastructure.Soap;

public class MtcSoapClient : IMtcSoapClient
{
    private readonly HttpClient _http;
    private readonly string _endpoint;

    public MtcSoapClient(HttpClient http, string endpoint)
    {
        _http = http;
        _endpoint = endpoint;
    }

    public async Task<int> SsnInternalCheckAsync(string ssn, int role)
    {
        // Build SOAP XML programmatically to avoid string escaping issues
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace tem = "http://tempuri.org/";

        var envelope = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(soapenv + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XAttribute(XNamespace.Xmlns + "tem", tem),
                new XElement(soapenv + "Header"),
                new XElement(soapenv + "Body",
                    new XElement(tem + "SSNInternalCheck",
                        new XElement(tem + "ssn", ssn ?? string.Empty),
                        new XElement(tem + "role", role)
                    )
                )
            )
        );

        var soapXml = envelope.ToString(SaveOptions.DisableFormatting);

        var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");
        var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = content
        };
        request.Headers.Add("SOAPAction", "http://tempuri.org/IMTCService/SSNInternalCheck");

        var resp = await _http.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var respStr = await resp.Content.ReadAsStringAsync();

        // parse
        var xml = XDocument.Parse(respStr);

        // look for fault
        var fault = xml.Descendants().FirstOrDefault(e => e.Name.LocalName == "Fault");
        if (fault != null)
        {
            var faultString = fault.Descendants().FirstOrDefault(e => e.Name.LocalName == "faultstring")?.Value ?? string.Empty;
            throw new Exception(faultString);
        }

        var result = xml.Descendants().FirstOrDefault(e => e.Name.LocalName == "SSNInternalCheckResult");
        if (result != null && int.TryParse(result.Value, out var id)) return id;
        return 0;
    }
}
