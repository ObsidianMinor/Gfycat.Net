using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.Album;

namespace Gfycat
{
    /// <summary>
    /// Represents a user album on Gfycat
    /// </summary>
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

        /// <summary>
        /// The title of this album
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// The number of <see cref="Gfy"/>s in this album
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// All <see cref="Gfy"/>s in this album
        /// </summary>
        public IReadOnlyCollection<Gfy> Content { get; private set; }
        /// <summary>
        /// The cover image this album uses on gfycat.com
        /// </summary>
        public string CoverImageUrl { get; private set; }
        /// <summary>
        /// The mobile cover image this album uses on gfycat.com
        /// </summary>
        public string CoverImageUrlMobile { get; private set; }
        /// <summary>
        /// The current NSFW (Not Safe For Work) setting
        /// </summary>
        public NsfwSetting NsfwSetting { get; private set; }
        /// <summary>
        /// The current published setting. If true, the album is public
        /// </summary>
        public bool Published { get; private set; }
        public int Order { get; private set; }

        /// <summary>
        /// Changes the title of the album on Gfycat to the specified string
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyTitleAsync(Id, newTitle, options);
            await UpdateAsync();
        }
        /// <summary>
        /// Changes the NSFW (Not Safe For Work) setting of the album on Gfycat to the specified setting
        /// </summary>
        /// <param name="newSetting"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyNsfwSettingAsync(NsfwSetting newSetting, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyNsfwSettingAsync(Id, newSetting, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Changes the published setting of the album on Gfycat to the specified setting
        /// </summary>
        /// <param name="published"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ModifyPublishSettingAsync(bool published, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyPublishedSettingAsync(Id, published, options);
            await UpdateAsync();
        }

        /// <summary>
        /// Adds the specified <see cref="Gfy"/>s to the album on Gfycat
        /// </summary>
        /// <param name="gfysToAdd"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task AddGfysAsync(IEnumerable<Gfy> gfysToAdd, RequestOptions options = null)
        {
            await Client.ApiClient.AddGfysAsync(Id, gfysToAdd.Select(g => g.Id), options);
            await UpdateAsync();
        }

        /// <summary>
        /// Moves the specified <see cref="Gfy"/>s to the specified album on Gfycat
        /// </summary>
        /// <param name="folderToMoveTo"></param>
        /// <param name="gfysToMove"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveGfysAsync(Album folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            await Client.ApiClient.MoveGfysAsync(Id, folderToMoveTo.Id, gfysToMove.Select(g => g.Id), options);
            await UpdateAsync();
            await folderToMoveTo.UpdateAsync();
        }

        /// <summary>
        /// Removes the specified <see cref="Gfy"/>s from the album on Gfycat
        /// </summary>
        /// <param name="gfysToRemove"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task RemoveGfysAsync(IEnumerable<Gfy> gfysToRemove, RequestOptions options = null)
        {
            await Client.ApiClient.RemoveGfysAsync(Id, gfysToRemove.Select(g => g.Id), options);
            await UpdateAsync();
        }

        /// <summary>
        /// Deletes the album on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteAsync(Id, options);
        }

        /// <summary>
        /// Updates this object with the data on Gfycat
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetAlbumContentsAsync(Id, options));

        async Task IFolderContent.MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options)
            => await MoveGfysAsync(folderToMoveTo as Album ?? throw new ArgumentException($"{nameof(folderToMoveTo)} must be an Album"), gfysToMove, options);
    }
}
