using System;
using System.Collections.Generic;

using Model = Gfycat.API.Models.ApplicationInfo;

namespace Gfycat
{
    /// <summary>
    /// Represents an developer key's app information
    /// </summary>
    public class AppApiInfo : Entity
    {
        internal AppApiInfo(GfycatClient client, string id) : base(client, id)
        {
        }
        /// <summary>
        /// Gets the contact name for this app
        /// </summary>
        public string ContactName { get; private set; }
        /// <summary>
        /// Gets a url for this app
        /// </summary>
        public string WebUrl { get; private set; }
        /// <summary>
        /// Gets the company of this app
        /// </summary>
        public string Company { get; private set; }
        /// <summary>
        /// Gets the creation date of this app
        /// </summary>
        public DateTime CreationDate { get; private set; }
        /// <summary>
        /// Gets the name of this app
        /// </summary>
        public string AppName { get; private set; }
        /// <summary>
        /// Gets the redirect uris of this app
        /// </summary>
        public IReadOnlyCollection<string> RedirectUris { get; private set; }
        /// <summary>
        /// Gets the type of this app
        /// </summary>
        public AppType AppType { get; private set; }
        /// <summary>
        /// Gets the username of the account which owns this app
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// Gets the email of the account which owns this app
        /// </summary>
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