using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Gfycat
{
    internal static class Utils
    {
        internal static IEnumerable<HttpStatusCode> Ignore404 => new[] { HttpStatusCode.NotFound };

        internal static bool IsSuccessfulStatus(this HttpStatusCode code) => ((int)code >= 200 && (int)code <= 299);
        
        internal static string CreateQueryString(IDictionary<string, object> dictionary)
        {
            var nullParsedDictionary = dictionary.Where(kv => (kv.Key != null && !string.IsNullOrWhiteSpace(kv.Key)) && kv.Value != null);

            if (nullParsedDictionary.Count() == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder("?");

            KeyValuePair<string, object> current = nullParsedDictionary.First();
            result.Append($"{WebUtility.UrlEncode(current.Key)}={WebUtility.UrlEncode(current.Value.ToString())}");

            for (int i = 1; i < nullParsedDictionary.Count(); i++)
            {
                current = nullParsedDictionary.ElementAt(i);
                result.Append($"&{WebUtility.UrlEncode(current.Key)}={WebUtility.UrlEncode(current.Value.ToString())}");
            }

            return result.ToString();
        }

        internal static string CreateQueryString(params (string Name, object Value)[] parameters)
            => CreateQueryString(new Dictionary<string, object>(parameters.ToDictionary(t => t.Name, t => t.Value)));

        internal static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
            => new ReadOnlyCollection<T>(enumerable?.ToList() ?? new List<T>());

        internal static IAlbumInfo CreateAlbum(GfycatClient client, API.Models.AlbumInfo albumModel, string ownerId)
        {
            if (string.Equals(albumModel.FolderSubType, "Album", StringComparison.OrdinalIgnoreCase))
                return AlbumInfo.Create(client, albumModel, ownerId);
            else
                return AlbumFolder.Create(client, albumModel, ownerId);
        }

        internal static readonly Dictionary<ReactionLanguage, string> _reactionLangToString = new Dictionary<ReactionLanguage, string>
        {
            { ReactionLanguage.English, null },
            { ReactionLanguage.Russian, "ru-RU" },
            { ReactionLanguage.Japanese, "ja-JP" },
            { ReactionLanguage.SimplifiedChinese, "zh-CN" },
            { ReactionLanguage.TraditionalChinese, "zh-TW" },
            { ReactionLanguage.French, "fr-FR" },
            { ReactionLanguage.German, "de-DE" },
            { ReactionLanguage.Spanish, "es-LA" },
            { ReactionLanguage.Korean, "ko-KR" },
            { ReactionLanguage.Arabic, "ar-SA" },
            { ReactionLanguage.Farsi, "fa-IR" },
            { ReactionLanguage.Hebrew, "he-IL" },
            { ReactionLanguage.Italian, "it-IT" },
            { ReactionLanguage.Thai, "th-TH" },
            { ReactionLanguage.Turkish, "tr-TR" },
            { ReactionLanguage.Vietnamese, "vi-VN" },
            { ReactionLanguage.Portuguese, "pt-BR" },
            { ReactionLanguage.HanChinese, "zh-Ha" }
        };
    }
}
