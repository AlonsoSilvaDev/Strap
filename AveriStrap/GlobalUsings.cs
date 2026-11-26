// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;

global using AveriStrap.Enums;
global using AveriStrap.Enums.Messagebox;
global using AveriStrap.Enums.BootstrapperDialogs;
global using AveriStrap.Exceptions;
global using AveriStrap.Extensions;
global using AveriStrap.Models;
global using AveriStrap.Models.APIs;
global using AveriStrap.Models.APIs.Config;
global using AveriStrap.Models.APIs.GitHub;
global using AveriStrap.Models.APIs.Roblox;
global using AveriStrap.Models.APIs.RoValra;
global using AveriStrap.Models.APIs.WebView2;
global using AveriStrap.Models.Attributes;
global using AveriStrap.Models.BloxstrapRPC;
global using AveriStrap.Models.Entities;
global using AveriStrap.Models.AveriStrapStudioRPC;
global using AveriStrap.Models.Manifest;
global using AveriStrap.Models.Persistable;
global using AveriStrap.Models.SettingTasks;
global using AveriStrap.Models.SettingTasks.Base;
global using AveriStrap.Localisation;
global using AveriStrap.UI;
global using AveriStrap.BackgroundUpdater;
global using AveriStrap.Utility;
