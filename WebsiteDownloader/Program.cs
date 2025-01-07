// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using WebsiteDownloader;

Uri[] sitesUrl = [
    new Uri("https://www.google.com"),
    new Uri("https://www.msn.com"),
    new Uri("https://www.sbc.com"),
    new Uri("https://www.wikipedia.com"),
    new Uri("https://www.cnbc.com"),
    new Uri("https://www.bloomberg.com"),
    new Uri("https://www.weather.com"),
];

ConcurrentQueue<Downloader> downloadersQueue = new();
Parallel.ForEach(sitesUrl, site => downloadersQueue.Enqueue(new Downloader(site)));

//parallel forEach

List<Task<int>> downloadTask = new();

while (downloadersQueue.TryDequeue(out Downloader downloader))
{ downloadTask.Add(downloader.DownloadAsync()); }

await Task.WhenAll(downloadTask);
int totalBytes = downloadTask.Sum(t => t.Result);
Console.WriteLine($"All sites downloaded, {totalBytes} bytes written.");