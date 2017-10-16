using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FruitAppAPI.Utils
{
    public class KeyVaultUtils
    {
        private string ClientId { get; set; }
        private string ClientSecret { get; set; }
        private string KeyVaultUrl { get; set; }

        private KeyVaultClient keyVaultClient;

        public KeyVaultUtils(string clientID, string clientSecret, string keyVaultUrl)
        {
            ClientId = clientID;
            ClientSecret = clientSecret;
            KeyVaultUrl = keyVaultUrl;

            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
        }

        public async Task<X509Certificate2> GetCertificate(string certName)
        {
            var rawCert = await keyVaultClient.GetCertificateAsync(KeyVaultUrl, certName);
            var keyReference = await keyVaultClient.GetSecretAsync(rawCert.Sid);

            var rawData = Convert.FromBase64String(keyReference.Value);

            var certificate = new X509Certificate2(
                rawData,
                string.Empty,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            return certificate;
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var credentials = new ClientCredential(ClientId, ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, credentials);
            if (result == null)
                throw new Exception();

            return result.AccessToken;
        }
    }
}
