using MauiApp1.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface IMusicRepository
    {
        public List<Cd> GetCdList();

        public List<Cd> GetCdList(int take);

        public Cd GetCd(int id);

        public bool SaveCd(Cd item);

        public bool DeleteCd(Cd item);

    }
}
