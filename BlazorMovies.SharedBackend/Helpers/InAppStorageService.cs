namespace BlazorMovies.SharedBackend.Helpers
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly string _webRootPath;
        private readonly string _currentUrl;

        public InAppStorageService(string webRootPath, string currentUrl)
        {
            _webRootPath = webRootPath;
            _currentUrl = currentUrl;
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";

            var folder = Path.Combine(_webRootPath, containerName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var savingPath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savingPath, content);

            var pathForDatabase = Path.Combine(_currentUrl, containerName, fileName);

            return pathForDatabase;
        }

        public Task DeleteFile(string fileRoute, string containerName)
        {
            var fileName = Path.GetFileName(fileRoute);

            var fileDirectory = Path.Combine(_webRootPath, containerName, fileName);

            if (!File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute)
        {
            if (!string.IsNullOrEmpty(fileRoute))
            {
                await DeleteFile(fileRoute, containerName);
            }

            return await SaveFile(content, extension, containerName);
        }
    }
}
