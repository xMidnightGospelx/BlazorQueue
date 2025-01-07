using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebsiteDownloader
{
    internal class Downloader
    {
        public Uri Website { get; }
        public byte[] ResultBytes { get; set; }

        private HttpClient _httpClient;
        private string _domain;

        public Downloader(Uri website)
        {
            Website = website;
            _httpClient = new HttpClient();

            var matched = Regex.Match(Website.ToString(), @"https?://(www\.)?([a-zA-Z0-9]+)(\.[a-zA-Z0-9.-]+)");
            _domain = matched.Groups[2].Value;
        }

        internal async Task<int> DownloadAsync()
        {
            int threadId = Environment.CurrentManagedThreadId;
            Console.WriteLine($"Before await in GetByteArrayAsync: Thread ID {threadId}");

            ResultBytes = await _httpClient.GetByteArrayAsync(Website);
            Console.WriteLine($"{Website.ToString()} received. {ResultBytes.Length} bytes.");

            threadId = Environment.CurrentManagedThreadId;
            Console.WriteLine($"After await in GetByteArrayAsync: Thread ID {threadId}");

            string fileName = $"{_domain}-{Path.GetRandomFileName()[0..5]}.txt";
            await File.WriteAllBytesAsync(fileName, ResultBytes);

            Console.WriteLine($"{fileName} written to disk.");
            return ResultBytes.Length;
        }
    }
}
