using System.Text;
using Microsoft.Extensions.Configuration;
using Oci.Common.Auth;
using Oci.SecretsService;
using Oci.SecretsService.Models;
using Oci.SecretsService.Requests;

namespace ContentModerationService.Infrastructure;

public class OracleVaultSecretLoader
{
    public static async Task LoadAsync(
        IConfigurationBuilder config,
        IEnumerable<(string ConfigKey, string SecretName)> secrets)
    {
        var provider = new InstancePrincipalsAuthenticationDetailsProvider();

        const string VAULT_ID = "ocid1.vault.oc1.eu-frankfurt-1.enuus7fkaaag6.abtheljsjwjrmefowdmggafbmncmqxwmxbdijm3khz3rlvyhzynsgjwaadiq";

        using var secretsClient = new SecretsClient(
            provider, 
            clientConfiguration: null,
            endpoint: "https://secrets.vaults.eu-frankfurt-1.oci.oraclecloud.com");

        var values = new Dictionary<string, string>();

        foreach (var (configKey, secretName) in secrets)
        {
            var response = await secretsClient.GetSecretBundleByName(
                new GetSecretBundleByNameRequest
                {
                    SecretName = secretName,
                    VaultId = VAULT_ID
                });

            var content = (Base64SecretBundleContentDetails)response.SecretBundle.SecretBundleContent;

            values[configKey] = Encoding.UTF8.GetString(Convert.FromBase64String(content.Content)); ;
        }

        config.AddInMemoryCollection(values);
    }
}