using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Modals
{
    public class DisplayViewItem: ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MediaDataType Type { get; set; }
        public byte[]? Cover { get; set; }

    }
}
