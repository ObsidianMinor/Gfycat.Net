﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Model = Gfycat.API.Models.Folder;

namespace Gfycat
{
    public class Folder : Entity, IFolderContent, IUpdatable
    {
        public string Title { get; private set; }
        /// <summary>
        /// The number of <see cref="Gfy"/>s in this folder
        /// </summary>
        public int Count { get; private set; }
        public IReadOnlyCollection<Gfy> Content { get; private set; }

        internal Folder(GfycatClient client, string id) : base(client, id)
        {
        }

        internal void Update(Model model)
        {
            Title = model.Title;
            Count = model.GfyCount;
            Content = model.PublishedGfys.Select(g => Gfy.Create(Client, g)).ToReadOnlyCollection();
        }

        internal static Folder Create(GfycatClient client, Model model)
        {
            Folder folder = new Folder(client, model.Id);
            folder.Update(model);
            return folder;
        }

        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await Client.ApiClient.GetFolderContentsAsync(Id, options));

        public async Task ModifyTitleAsync(string newTitle, RequestOptions options = null)
        {
            await Client.ApiClient.ModifyFolderTitleAsync(Id, newTitle, options);
            await UpdateAsync();
        }
        
        /// <summary>
        /// Move some of the gfycats within a folder to another folder
        /// </summary>
        /// <param name="folderToMoveTo"></param>
        /// <param name="gfysToMove"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task MoveGfysAsync(Folder folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options = null)
        {
            await Client.ApiClient.MoveGfysAsync(Id, new API.GfyFolderAction() { Action = "move_contents", GfycatIds = gfysToMove.Select(g => g.Id), ParentId = folderToMoveTo.Id }, options);
            await UpdateAsync();
            await folderToMoveTo.UpdateAsync();
        }

        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Client.ApiClient.DeleteFolderAsync(Id, options);
        }
        
        #region Expicit IFolder implimentation
        
        async Task IFolderContent.MoveGfysAsync(IFolderContent folderToMoveTo, IEnumerable<Gfy> gfysToMove, RequestOptions options)
            => await MoveGfysAsync(folderToMoveTo as Folder ?? throw new ArgumentException($"Can't move gfys from a folder into {folderToMoveTo.GetType()}"), gfysToMove, options);

        #endregion
    }
}
