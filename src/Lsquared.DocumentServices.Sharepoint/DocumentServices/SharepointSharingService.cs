using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lsquared.DocumentServices
{
    public class SharepointSharingService : IDocumentSharingService
    {
        public SharepointSharingService(SharepointSharingOptions options)
        {
            _options = options;
        }

        public IEnumerable<Folder> ListFolders(string folderPath)
        {
            var client = CreateClientAndAuthenticate(_options.UserName, _options.Password, _options.Domain).Result;
            client.DefaultRequestHeaders.Add("Accept", Consts.AcceptJson);
            client.DefaultRequestHeaders.Add("ContentType", Consts.JsonContentType);

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = _options.Domain,
                Path = string.Format("_api/web/GetFolderByServerRelativeUrl('{0}')/Folders", folderPath),
            };

            var response = client.GetAsync(builder.Uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                // TODO
                throw new Exception();
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;

            var jsonResult = JObject.Parse(responseContent);
            var folders = jsonResult["d"]["results"];

            return from folder in folders
                   let name = folder["Name"].ToString()
                   where name != "Forms"
                   select new Folder
                   {
                       Name = folder["Name"].ToString(),
                       Path = folder["ServerRelativeUrl"].ToString(),
                       Uri = new Uri(folder["__metadata"]["uri"].ToString(), UriKind.RelativeOrAbsolute)
                   };
        }

        public IEnumerable<Document> ListDocuments(string folderPath)
        {
            var client = CreateClientAndAuthenticate(_options.UserName, _options.Password, _options.Domain).Result;
            client.DefaultRequestHeaders.Add("Accept", Consts.AcceptJson);
            client.DefaultRequestHeaders.Add("ContentType", Consts.JsonContentType);

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = _options.Domain,
                Path = string.Format("_api/web/GetFolderByServerRelativeUrl('{0}')/Files", folderPath),
            };

            var response = client.GetAsync(builder.Uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                // TODO
                throw new Exception();
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;

            var jsonResult = JObject.Parse(responseContent);
            var files = jsonResult["d"]["results"];

            return from file in files
                   select CreateDocument(file, _options.Domain);
        }

        public Document ShareDocument(string remoteFolderPath, string fileName, Stream file)
        {
            var client = CreateClientAndAuthenticate(_options.UserName, _options.Password, _options.Domain).Result;
            var digest = GetDigest(client).Result;

            client.DefaultRequestHeaders.Add("X-RequestDigest", digest);
            client.DefaultRequestHeaders.Add("binaryStringRequestBody", "true");

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = _options.Domain,
                Path = string.Format("_api/web/GetFolderByServerRelativeUrl('{0}')/Files/add(url='{1}',overwrite=true)", remoteFolderPath, fileName),
            };

            var response = client.PostAsync(builder.Uri, new StreamContent(file)).Result;

            if (!response.IsSuccessStatusCode)
            {
                // TODO
                throw new Exception();
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var jsonResult = JObject.Parse(responseContent);

            return CreateDocument(jsonResult["d"], _options.Domain);
        }

        private static Document CreateDocument(JToken json, string domain)
        {
            var viewUriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = domain,
            };

            var fileName = json["Name"].ToString();
            var extension = Path.GetExtension(fileName).ToUpperInvariant();
            if (extension == ".PDF")
            {
                viewUriBuilder.Path = json["ServerRelativeUrl"].ToString();
            }
            else
            {
                viewUriBuilder.Path = "_layouts/WopiFrame.aspx";

                var uniqueId = json["UniqueId"].ToString().ToUpperInvariant();
                viewUriBuilder.Query = string.Format("sourcedoc=%7B{0}%7D&file={1}&action=view", uniqueId, fileName);
            }

            return new Document
            {
                Name = json["Name"].ToString(),
                Path = json["ServerRelativeUrl"].ToString(),
                Uri = new Uri(json["__metadata"]["uri"].ToString(), UriKind.RelativeOrAbsolute),
                ViewUri = viewUriBuilder.Uri
            };
        }

        private static async Task<HttpClient> CreateClientAndAuthenticate(string userName, string password, string domainAddress)
        {
            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = domainAddress,
            };

            var client = new HttpClient();
            var token = await GetSamlToken(client, userName, password, domainAddress);
            await SendSamlToken(client, builder.Uri, token);

            return client;
        }

        private async Task<string> GetDigest(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Accept", Consts.AcceptJson);
            client.DefaultRequestHeaders.Add("ContentType", Consts.JsonContentType);

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = _options.Domain,
                Path = "_api/contextinfo",
            };

            var response = await client.PostAsync(builder.Uri, new StringContent(""));

            if (!response.IsSuccessStatusCode)
            {
                // TODO
                throw new Exception();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResult = JObject.Parse(responseContent);
            return jsonResult["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
        }

        private static async Task SendSamlToken(HttpClient client, Uri baseUri, string token)
        {
            client.DefaultRequestHeaders.Host = baseUri.Host;
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)");

            Uri requestUri;
            if (!Uri.TryCreate(baseUri, Consts.SigninRelativeAddress, out requestUri))
            {
                // TODO
                throw new Exception();
            }

            var response = await client.PostAsync(requestUri, new StringContent(token));
            if (response.IsSuccessStatusCode)
            {
            }
            else
            {
                // TODO
                throw new Exception();
            }
        }

        private static async Task<string> GetSamlToken(HttpClient client, string userName, string password, string domainAddress)
        {
            var content = new StringContent(string.Format(Consts.SamlMessage, userName, password, domainAddress));
            var response = await client.PostAsync(Consts.SamlAddress, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var xdoc = XDocument.Parse(responseContent);
                var tokenElement = xdoc.Descendants(Consts.OasisNs + "BinarySecurityToken").FirstOrDefault();
                return tokenElement.Value;
            }
            else
            {
                // TODO
                throw new Exception();
            }
        }

        private readonly SharepointSharingOptions _options;

        private static class Consts
        {
            internal const string AcceptJson = @"application/json;odata=verbose";

            internal const string JsonContentType = @"application/json";

            internal static readonly XNamespace OasisNs = XNamespace.Get("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            internal const string SharepointAddress = "https://{0}.sharepoint.com/_api/";

            internal const string SigninRelativeAddress = "_forms/default.aspx?wa=wsignin1.0";

            internal const string SamlAddress = "https://login.microsoftonline.com/extSTS.srf";

            internal const string SamlMessage = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
  <s:Header>
    <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>
    <a:ReplyTo>
      <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
    </a:ReplyTo>
    <a:To s:mustUnderstand=""1"">https://login.microsoftonline.com/extSTS.srf</a:To>
    <o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
      <o:UsernameToken>
        <o:Username>{0}</o:Username>
        <o:Password>{1}</o:Password>
      </o:UsernameToken>
    </o:Security>
  </s:Header>
  <s:Body>
    <t:RequestSecurityToken xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
      <wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
        <a:EndpointReference>
          <a:Address>{2}</a:Address>
        </a:EndpointReference>
      </wsp:AppliesTo>
      <t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType>
      <t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType>
      <t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType>
    </t:RequestSecurityToken>
  </s:Body>
</s:Envelope>";

        }
    }
}
