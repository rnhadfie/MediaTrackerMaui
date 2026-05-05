using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Interface
{
    public interface IOtherRepository
    {
        public List<BackEnd.Modals.Other> GetOtherList();

        public List<BackEnd.Modals.Other> GetOtherList(int take);

        public BackEnd.Modals.Other GetOtherItem(int id);

        public bool SaveOtherItem(BackEnd.Modals.Other item);

        public bool DeleteOtherItem(BackEnd.Modals.Other item);

    }
}
