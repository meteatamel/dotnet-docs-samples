/*
 * Copyright 2024 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START secretmanager_list_regional_secrets]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListRegionalSecretsSample
{
    public List<Secret> ListRegionalSecrets(string projectId = "my-project", string locationId = "my-location")
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Call the API.
        List<Secret> secrets = client.ListSecrets(locationName).ToList();

        // Traversing the secret list.
        foreach (Secret secret in secrets)
        {
            Console.WriteLine($"Got regional secret : {secret.Name}");
        }

        return secrets;
    }
}
// [END secretmanager_list_regional_secrets]
