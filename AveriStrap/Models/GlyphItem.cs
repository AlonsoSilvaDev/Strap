// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Media;

namespace AveriStrap.Models
{
    internal sealed class GlyphItem
    {
        public Geometry? Data { get; set; }
        public IBrush? Brush { get; set; }
    }
}