using System.Linq;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Responses.Types
{
    public class PinboardResult : BaseResult<Pinboard>
    {}

    public class Pinboard
    {
        private Group group;
        public string Id { get; set; }
        [RequiredParameter]
        public string Title { get; set; }
        [RequiredParameter]
        public string Content { get; set; }

        [RequiredParameter]
        public Group Group
        {
            get => group ?? Groups?.FirstOrDefault();
            set => group = value;
        }

        [RequiredParameter]
        public User CreatedBy { get; set; }

        public string Created { get; set; }
        public string LastModified { get; set; }
        public string ObjectType { get; set; }
        public Group[] Groups { get; set; }
        public Permission UserPermission { get; set; }
    }
}