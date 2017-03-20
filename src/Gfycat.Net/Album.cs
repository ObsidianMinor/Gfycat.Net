using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Album;

namespace Gfycat
{
    public class Album : Entity, IFolder, IUpdatable
    {
        internal Album(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            Title = model.Title;
            Count = model.GfyCount;
            Content = model.PublishedGfys.Select(g => Gfy.Create(Client, g)).ToReadOnlyCollection();
            CoverImageUrl = model.CoverImageUrl;
            CoverImageUrlMobile = model.CoverImageUrlMobile;
            NsfwSetting = model.Nsfw;
            Published = model.Published;
            Order = model.Order;
        }

        internal static Album Create(GfycatClient client, Model model)
        {
            Album album = new Album(client, model.Id);
            album.Update(model);
            return album;
        }

        public string Title { get; private set; }
        public int Count { get; private set; }
        public IReadOnlyCollection<Gfy> Content { get; private set; }
        public string CoverImageUrl { get; private set; }
        public string CoverImageUrlMobile { get; private set; }
        public NsfwSetting NsfwSetting { get; private set; }
        public bool Published { get; private set; }
        public int Order { get; private set; }

        public Task CreateNewFolderAsync(string folderName, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task CreateNewAlbumAsync(string albumName, string description, IEnumerable<Gfy> gfys = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task MoveFolderAsync(IFolder parent, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task MoveGfysAsync(IFolder folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetAlbumContentsAsync(Id, options));
    }
}
