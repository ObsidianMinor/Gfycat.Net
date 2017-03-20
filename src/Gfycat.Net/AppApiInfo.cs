using System;
using System.Collections.Generic;

using Model = Gfycat.API.Models.ApplicationInfo;

namespace Gfycat
{
    public class AppApiInfo : Entity
    {
        internal AppApiInfo(GfycatClient client, string id) : base(client, id)
        {
        }

        public string ContactName { get; private set; }
        public string WebUrl { get; private set; }
        public string Company { get; private set; }
        public DateTime CreationDate { get; private set; }
        public string AppName { get; private set; }
        public IReadOnlyCollection<string> RedirectUris { get; private set; }
        public AppType AppType { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        internal static AppApiInfo Create(GfycatClient client, Model model)
        {
            return new AppApiInfo(client, model.ClientId)
            {
                AppName = model.AppName,
                AppType = model.AppType,
                Company = model.Company,
                ContactName = model.ContactName,
                CreationDate = model.CreationDate,
                Email = model.Email,
                RedirectUris = model.RedirectUris.ToReadOnlyCollection(),
                Username = model.Username,
                WebUrl = model.WebUrl
            };
        }
    }
}