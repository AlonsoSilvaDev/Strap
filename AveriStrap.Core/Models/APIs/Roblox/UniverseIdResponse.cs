// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Models.APIs.Roblox
{
    internal class UniverseIdResponse
    {
        [JsonPropertyName("universeId")]
        public long? UniverseId { get; set; }
    }
}