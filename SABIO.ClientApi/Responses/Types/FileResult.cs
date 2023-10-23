using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SABIO.ClientApi.Core;
using NotImplementedException = System.NotImplementedException;

namespace SABIO.ClientApi.Responses.Types
{
    public class FilesResponse : SabioResponse<FilesResult>
    { }

    public class FileResponse : SabioResponse<FileResult>
    { }

    public class FileResult : BaseResult<File>
    { }

    public class FilesResult : BaseResult<File[]>
    {
        public int Limit { get; set; }
        public int Start { get; set; }
        public int Total { get; set; }
    }

    public class FileFolder
    {
        public string Id { get; set; }
        public string Created { get; set; }
        public string LastModified { get; set; }
        public string ObjectType { get; set; }
        public string ParentFolderId { get; set; }
        public string Title { get; set; }
        public bool Folder { get; set; }
        public bool Empty { get; set; }
        public string RealmId { get; set; }
    }


    public class File : FileFolder
    {
        public string Description { get; set; }
        public string Filename { get; set; }
        public int Size { get; set; }
        public int Checksum { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public bool IsBinary { get; set; }
        public Group[] TargetGroups { get; set; }
        public Group OwnerGroup { get; set; }
        public User Owner { get; set; }
        public object[] Path { get; set; }
        public Branch[] Branches { get; set; }
        public Relation[] Relations { get; set; }
        public int UserPermission { get; set; }
        public bool Bookmark { get; set; }
        public UploadableFile ToUploadableFile(byte[] data) => new(this, data);
        public async Task<string> GetAccessUrlAsync(SabioClient client) => (await client.Apis.FileManagement.GetFileAccessUrlAsync(this)).Data.Result.Url;

        public async Task<byte[]> GetBytesAsync(SabioClient client) => await client.HttpClient.GetByteArrayAsync(await GetAccessUrlAsync(client));
    }

    public class UploadableFile
    {
        public UploadableFile()
        {}

        public UploadableFile(File file, byte[] data)
        {
            Data = data;
            Description = file.Description;
            FileName = file.Filename;
            Title = file.Title;
            Type = file.MimeType;
            ParentFolderId = file.ParentFolderId ?? "root";
            OwnerGroupId = file.OwnerGroup?.Id;
            OwnerId = file.Owner?.Id;
            TargetGroupIds = file.TargetGroups?.Select(g => g.Id).ToArray();
        }

        public byte[] Data { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ParentFolderId { get; set; } = "root";
        public string OwnerGroupId { get; set; }
        public string OwnerId { get; set; }
        public string[] TargetGroupIds { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                owner = new { id = OwnerId },
                ownerGroup = new { id = OwnerGroupId },
                targetGroups = TargetGroupIds.Select(id => new { id = id }).ToArray(),
                parentFolderId = ParentFolderId,
                title = Title,
                description = Description,
                filename = FileName,
            });
        }
    }


    public class Relation
    {
        public string Id { get; set; }
        public string EntityType { get; set; }
        public string RelationType { get; set; }
        public Entitydetails EntityDetails { get; set; }
    }

    public class Entitydetails
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TextId { get; set; }
        public string[] ContentviewIds { get; set; }
        public int Size { get; set; }
    }

    public class FileAccess
    {
        public string Url { get; set; }
    }
}