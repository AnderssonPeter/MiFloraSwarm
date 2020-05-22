using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HuaHuaCaoCao.Client
{
    public class ImageClient
    {
        HttpClient client;
        public ImageClient(HttpClient client)
        {
            this.client = client;
        }

        public class Image
        {
            public string ContentType { get; set; }
            public Byte[] Content { get; set; }
            public string ETag { get; set; }
        }

        public async Task<Image> DownloadImageAsync(string url)
        {
            var uri = new Uri(url);
            string path = String.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
            var result = await client.GetAsync(path);
            result.EnsureSuccessStatusCode();
            var contentType = result.Content.Headers.ContentType.MediaType;
            var etag = result.Headers.ETag.Tag;
            var content = await result.Content.ReadAsByteArrayAsync();
            return new Image() { ContentType = contentType, Content = content, ETag = etag };
        }
    }
}
