using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Concurrent;

namespace BlazorServer.ParallelFileUpload
{
    public class FileUploadManager
    {
        private readonly FileUploadApiService fileUploadService;

        private readonly List<Task> tasks;
        private readonly TaskFactory factory;

        private readonly ConcurrentQueue<IBrowserFile> fileUploadQueue;

        // Use our factory to run a set of tasks. 
        private readonly SemaphoreSlim semaphoreSlim;

        public FileUploadManager(FileUploadApiService fileUploadService)
        {
            this.fileUploadService = fileUploadService;

            fileUploadQueue = new ConcurrentQueue<IBrowserFile>();
            tasks = new List<Task>();
            factory = new TaskFactory();

            var maxParallelUplaods = 5;
            semaphoreSlim = new SemaphoreSlim(maxParallelUplaods, maxParallelUplaods);
        }

        public void AddFilesToUploadQueue(IReadOnlyList<IBrowserFile> files)
        {
            lock (fileUploadQueue)
            {
                foreach (var item in files)
                {
                    fileUploadQueue.Enqueue(item);
                }
            }

            for (int i = 0; i < files.Count; i++)
            {
                Task t = factory.StartNew(async () =>
                {
                    await semaphoreSlim.WaitAsync();

                    IBrowserFile? file = null;

                    lock (fileUploadQueue)
                    {
                        fileUploadQueue.TryDequeue(out file);
                    }

                    using (var readStream = file.OpenReadStream(int.MaxValue))
                    {
                        var streamContent = fileUploadService.CreateStreamContent(readStream, file.Name, file.ContentType);

                        var result = await fileUploadService.UploadFileAsync(streamContent);
                    }

                    semaphoreSlim.Release();

                });

                tasks.Add(t);
            }
        }
    }
}