// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Models
{
    internal sealed class StoredCredential
    {
        [JsonPropertyName("$type")]
        public string Type { get; set; } = "CredentialWithToken";

        [JsonPropertyName("Token")]
        public string Token { get; set; } = "";
    }
}
