using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SABIO.ClientApi.Responses;
using System.Threading.Tasks;
using SABIO.ClientApi.Responses.Types;
using File = SABIO.ClientApi.Responses.Types.File;
using FileAccess = SABIO.ClientApi.Responses.Types.FileAccess;
using System.Net.Http.Headers;
using SABIO.ClientApi.Extensions;


namespace SABIO.ClientApi.Core.Api
{
    public class FileManagementApi : SabioApiBase
    {
        public Task<FilesResponse> GetAllAsync(string dir = null) => Client.GetAsync<FilesResponse>($"/fm/dir/{dir ?? "root"}");
        public Task<FileResponse> GetAsync(string fileId) => Client.GetAsync<FileResponse>($"/fm/{fileId}");
        public Task<SabioResponse<BaseResult<FileAccess>>> GetFileAccessUrlAsync(File file) => GetFileAccessUrlAsync(file.Id);
        public Task<SabioResponse<BaseResult<FileAccess>>> GetFileAccessUrlAsync(string fileId) => Client.GetAsync<SabioResponse<BaseResult<FileAccess>>>($"/fm/url/{fileId}");
        public Task<FilesResponse> CreateFolderAsync(FileFolder folder) => Client.PostAsync<FilesResponse>($"/fm/dir/", folder);
        public Task<FileResponse> CreateFileAsync(File file, byte[] data) => CreateFileAsync(file.ToUploadableFile(data));
        public async Task<FileResponse> CreateFileAsync(File file) => await CreateFileAsync(file.ToUploadableFile(await file.GetBytesAsync(Client)));

        public Task<FileResponse> CreateFileAsync(UploadableFile file)
        {
            file.Title ??= file.FileName;
            file.ParentFolderId ??= "root";
            var content = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(file.Data);
            if (!string.IsNullOrEmpty(file.Type))
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.Type);
            content.Add(fileContent, "file", file.FileName);
            content.Add(file.ToStringContent(file.ToJson()), "fileMeta");

            return Client.PostAsync<FileResponse>("/fm/file", content);
        }

        public Task DeleteFolderAsync(FileFolder folder) => DeleteFolderAsync(folder.Id);
        public Task DeleteFileAsync(File file) => Client.DeleteAsync($"/fm/{file.Id}");
        public Task DeleteFileAsync(string fileId) => Client.DeleteAsync($"/fm/{fileId}");
        public Task DeleteFolderAsync(string folderId) => Client.DeleteAsync($"/fm/dir/{folderId}");

        public async Task<bool> CanWorkAsync()
        {
            if (!await Client.CanWorkAsync())
                return false;
            var config = await Client.Apis.Config.ConfigAsync();
            return config.Data.System.FileManagementEnabled;
        }

        public async Task<List<FileFolder>> CreateFolderStructureAsync(params string[] structures)
        {
            var result = new List<FileFolder>();
            FileFolder folder;
            string parentId;

            foreach (var s in structures)
            {
                parentId = "root";
                foreach (var n in s.Split('/').Where(str => !string.IsNullOrWhiteSpace(str)))
                {
                    folder = await GetOrCreateFolderAsync(new FileFolder { Title = n, ParentFolderId = parentId });
                    result.Add(folder);
                    parentId = folder.Id;
                }
            }

            return result;
        }

        public async Task<FileFolder> GetOrCreateFolderAsync(FileFolder options)
        {
            var response = await GetAllAsync(options.ParentFolderId);
            var folder = response?.Data?.Result?.FirstOrDefault(f => f.Title == options.Title);

            if (folder == null)
            {
                var newFolder = new FileFolder
                {
                    Title = options.Title,
                    ParentFolderId = options.ParentFolderId
                };
                var createResponse = await CreateFolderAsync(newFolder);
                folder = createResponse?.Data?.Result?.FirstOrDefault();
            }

            return folder;
        }

    }
}


