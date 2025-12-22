using System.Text;
using Microsoft.Extensions.Configuration;
using Oci.Common.Auth;
using Oci.SecretsService;
using Oci.SecretsService.Requests;

namespace ContentModerationService.Infrastructure;

public class OracleVaultSecretLoader
{
    public static async Task LoadAsync(
        IConfigurationBuilder config,
        IEnumerable<(string ConfigKey, string SecretOcid)> secrets)
    {
        var provider = new InstancePrincipalsAuthenticationDetailsProvider();
        using var client = new SecretsClient(provider);

        var values = new Dictionary<string, string>();

        foreach (var (configKey, secretOcid) in secrets)
        {
            var response = await client.GetSecretBundle(
                new GetSecretBundleRequest
                {
                    SecretId = secretOcid
                });

            // Testing
            var base64 = response.SecretBundle.SecretBundleContent;
            Console.WriteLine("SecretBundle: {Base64}", base64);

            var base64String = response.SecretBundle.SecretBundleContent.ToString();
            Console.WriteLine("SecretBundle ToString(): {Base64String}", base64String);
            var value = Encoding.UTF8.GetString(Convert.FromBase64String(base64String!));

            values[configKey] = value;
        }

        config.AddInMemoryCollection(values);
    }
}