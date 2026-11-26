// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Media;
using AveriStrap.UI.ViewModels;

namespace AveriStrap.Models
{
    internal sealed class RobloxIconEntry : NotifyPropertyChangedViewModel
    {
        public RobloxIcon IconType { get; set; }
        public IImage ImageSource => IconType.GetIcon().GetImageSource();
        public void RefreshImage() => OnPropertyChanged(nameof(ImageSource));
    }
}