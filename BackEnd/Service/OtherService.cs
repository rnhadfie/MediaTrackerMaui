using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Repository;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    public class OtherService
    {

        private Lazy<OtherRepository> OtherRepository;
        private OtherRepository _OtherRepository;

        public OtherService(DataContext dataContext)
        {
            _OtherRepository = new Lazy<OtherRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new OtherRepository(dataContext);
            }).Value;
        }

        public bool AddOther(BackEnd.Modals.Other newVideo)
        {
            byte[] CompressedImageData = newVideo.Image ?? [];
            if (CompressedImageData.Length > 0)
            {
                newVideo.Image = SharedService.CompressImage(CompressedImageData, 100, 60);
            }
            return _OtherRepository.SaveOtherItem(newVideo);
        }


        public Other GetOtherItemInfo(int id)
        {
            return _OtherRepository.GetOtherItem(id);
        }

        public List<DisplayViewItem> GetOtherDisplayItems(int take = -1)
        {
            List<Other> otherList = take != -1 ? _OtherRepository.GetOtherList(take) : _OtherRepository.GetOtherList();
            List<DisplayViewItem> videoDisplayList = new List<DisplayViewItem>();
            otherList.ForEach(x =>
            {
                videoDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Cover = x.Image,
                    Type = MediaDataType.Other

                });
            });
            return videoDisplayList;
        }

        public List<Other> GetOtherItems(int take = -1)
        {
            List<Other> otherList = take != -1 ? _OtherRepository.GetOtherList(take) : _OtherRepository.GetOtherList();
           return otherList;
        }
    }
}
