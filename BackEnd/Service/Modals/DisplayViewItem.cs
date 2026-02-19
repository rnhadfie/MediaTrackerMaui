using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using static MauiApp1.Shared.Enums;

namespace MauiApp1.BackEnd.Service.Modals
{
    public class DisplayViewItem: ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MediaDataType Type { get; set; }
        public byte[]? Cover { get; set; }

    }
}
