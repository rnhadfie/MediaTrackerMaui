using MauiApp1.BackEnd.Database;
using MauiApp1.BackEnd.Modals;
using MauiApp1.BackEnd.Repository;
using MauiApp1.Modals;
using MauiApp1.Shared;
using static MauiApp1.BackEnd.Shared.Enums;

namespace MauiApp1.BackEnd.Service
{
    class MusicSerivce
    {
        private Lazy<MusicRepository> MusicRepository;
        private MusicRepository _MusicRepository;

        public MusicSerivce(DataContext dataContext)
        {
            _MusicRepository = new Lazy<MusicRepository>(() =>
            {
                // You can specify any additional
                // initialization steps here.
                return new MusicRepository(dataContext);
            }).Value;
        }

        public bool AddMusic(Cd newCd)
        {
            byte[] CompressedImageData = newCd.Cover ?? [];
            if (CompressedImageData.Length > 0)
            {
                newCd.Cover = SharedService.CompressImage(CompressedImageData, 100, 60);
            }
            return _MusicRepository.SaveCd(newCd);
        }

        

        public Cd GetMusicInfo(int id)
        {
            return _MusicRepository.GetCd(id);
        }

        public List<DisplayViewItem> GetCds(int take = -1)
        {
            
            List<Cd> cdList = take == -1 ? _MusicRepository.GetCdList() : _MusicRepository.GetCdList(take);
            List<DisplayViewItem> cdDisplayList = new List<DisplayViewItem>();
            cdList.ForEach(x =>
            {
                cdDisplayList.Add(new DisplayViewItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Cover = x.Cover,
                    Type = MediaDataType.Cd

                });
            });
            return cdDisplayList;
        }

        public List<Cd> GetCdList(int take = -1)
        {

            List<Cd> cdList = take == -1 ? _MusicRepository.GetCdList() : _MusicRepository.GetCdList(take);
          
            return cdList;
        }



        public List<TextValuePair<int>> GetGenres()
        {
            var genres = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("Pop", 1),
                new TextValuePair<int>("Rock", 2),
                new TextValuePair<int>("Rap",  3 ),
                new TextValuePair<int>("R&B", 4 ),
                new TextValuePair<int>("Alternative",  5 ),
                new TextValuePair<int>("Punk",  6 ),
            };
            return genres;
        }

        public List<TextValuePair<int>> GetLanguage()
        {
            var genres = new List<TextValuePair<int>>
            {
                new TextValuePair<int>("English", 1),
                new TextValuePair<int>("Japanese", 2),
                new TextValuePair<int>("Spanish",  3 ),
                new TextValuePair<int>("French", 4 ),
            };
            return genres;
        }
    }
}
