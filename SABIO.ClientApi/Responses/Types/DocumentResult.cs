using System.Threading.Tasks;
using SABIO.ClientApi.Core;
using NotImplementedException = System.NotImplementedException;

namespace SABIO.ClientApi.Responses.Types
{
    public class DocumentResult : BaseResult<Document>
    {
        public Task<byte[]> GetBytesAsync(SabioClient client)
        {
            return Result?.GetBytesAsync(client);
        }

    }

    public class Document
    {

        public string Id { get; set; }
        public string Created { get; set; }
        public string LastModified { get; set; }
        public string ObjectType { get; set; }
        public string AttachmentUri { get; set; }
        public bool Bookmark { get; set; }
        public Branch[] Branches { get; set; }
        public long Checksum { get; set; }
        public User CreatedBy { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public Group Group { get; set; }
        public Permission GroupPermission { get; set; }
        public bool Hidden { get; set; }
        public string InlineUri { get; set; }
        public User LastModifiedBy { get; set; }
        public Link[] Links { get; set; }
        public Permission OtherPermission { get; set; }
        public Path[][] Paths { get; set; }
        public string Resource { get; set; }
        public int Size { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Permission UserPermission { get; set; }

        public Task<byte[]> GetBytesAsync(SabioClient client)
        {
            return client.HttpClient.GetByteArrayAsync(InlineUri);
        }
    }
}