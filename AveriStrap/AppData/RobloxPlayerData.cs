// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using AveriStrap.Clients.Contracts;
using AveriStrap.Clients.Model;

namespace AveriStrap.AppData;

internal sealed class RobloxPlayerData(IClientLayout layout) : CommonAppData, IAppData
{
    public string ProductName => "Roblox";

    public override string BinaryType => layout.BinaryType!;

    public override string ExecutableName => layout.ExecutableName!;

    public bool IsBundle => layout.Shape == ExecutableShape.MacBundle;

    public override JsonManager<DistributionState> DistributionStateManager => App.PlayerState;
}
