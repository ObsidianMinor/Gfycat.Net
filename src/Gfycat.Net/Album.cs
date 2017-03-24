using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Album;

namespace Gfycat
{
    [DebuggerDisplay("Album: {Title}")]
    public class Album : Entity, IFolderContent, IUpdatable
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

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options);
            await UpdateAsync();
        }

        public async Task ModifyDescriptionAsync(string newDescription, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyDescriptionAsync(Id, newDescription, options);
            await UpdateAsync();
        }

        public async Task ModifyNsfwSettingAsync(NsfwSetting newSetting, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyNsfwSettingAsync(Id, newSetting, options);
            await UpdateAsync();
        }

        public async Task ModifyPublishSettingAsync(bool published, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyPublishedSettingAsync(Id, published, options);
            await UpdateAsync();
        }

        public async Task AddGfysAsync(IEnumerable<Gfy> gfysToAdd, RequestOptions options = null)
        {
            await Client.ApiClient.AddGfysAsync(Id, gfysToAdd.Select(g => g.Id), options);
            await UpdateAsync();
        }

        public async Task MoveGfysAsync(Album folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            await Client.ApiClient.MoveGfysAsync(Id, folderToMoveTo.Id, gfysToMove.Select(g => g.Id), options);
            await UpdateAsync();
            await folderToMoveTo.UpdateAsync();
        }

        public async Task RemoveGfysAsync(IEnumerable<Gfy> gfysToRemove, RequestOptions options = null)
        {
            await Client.ApiClient.RemoveGfysAsync(Id, gfysToRemove.Select(g => g.Id), options);
            await UpdateAsync();
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options);
        }

        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetAlbumContentsAsync(Id, options));

        async Task IFolderContent.MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options)
            => await MoveGfysAsync(folderToMoveTo as Album ?? throw new ArgumentException($"{nameof(folderToMoveTo)} must be an Album"), gfysToMove, options);
    }
}
