using MauiApp1.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Service
{
    public class GeneralService
    {
        public List<TextValuePair<int>> GetListOfSeries()
        {
            TextValuePair<int> list = new TextValuePair<int>("Test", 1);
            TextValuePair<int> list2 = new TextValuePair<int>("Test2",2);
            TextValuePair<int> list3 = new TextValuePair<int>("Test3", 3);
            List<TextValuePair<int>> series = new List<TextValuePair<int>> { list, list2, list3 };
            series.Add(new TextValuePair<int>("Add New series", 0));
            return series;
        }
    }
}
