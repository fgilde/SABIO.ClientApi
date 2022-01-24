using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using SABIO.ClientApi.Extensions;

namespace SABIO.ClientApi.Responses.Types
{
    public class SearchResult 
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public Branch[] Branches { get; set; }
        public string BranchNames => string.Join(",", Branches.Select(b => b.Title));
        public Colors Color { get; set; }
        public bool Confirmed { get; set; }
        public bool ContentPublic { get; set; }
        public string LastModified { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string LastExecuted { get; set; }
        public string LastActive { get; set; }
        public string Created { get; set; }

        public DateTime? LastModifiedDate => SabioConvert.ParseSabioDate(LastModified);
        public DateTime? ValidFromDate => SabioConvert.ParseSabioDate(ValidFrom);

        public DateTime? ValidToDate => SabioConvert.ParseSabioDate(ValidTo);
        public DateTime? LastExecutedDate => SabioConvert.ParseSabioDate(LastExecuted);
        public DateTime? LastActiveDate=> SabioConvert.ParseSabioDate(LastActive);
        public DateTime? CreatedDate => SabioConvert.ParseSabioDate(Created);    
        public string CreatedBy { get; set; }
        public bool CreatorDeleted { get; set; }
        public string Definition { get; set; }
        public string Department { get; set; }
        public string Excerpt { get; set; }
        public string Expert { get; set; }
        public string Explanation { get; set; }
        public string Extension { get; set; }
        public string FallBackSnippet { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }        
        public string LastModifiedBy { get; set; }
        public string LastModifiedById { get; set; }
        public bool LastModifierDeleted { get; set; }
        //public MatchedTerms MatchedTerms { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public bool NightlyExport { get; set; }
        public string ObjectType { get; set; }
        public string Phone { get; set; }
        public string Priority { get; set; }
        public float Rating { get; set; }

        public string Resource { get; set; }            
        public string Score { get; set; }            
        public string Size { get; set; }            
        public string TargetId { get; set; }
        public Theme Theme { get; set; }                        
        public string Title { get; set; }                    
        [JsonProperty("treenodeId")]
        public string TreeNodeId { get; set; }                        
        public string TreePath { get; set; }                        
        public string Type { get; set; }
        public Group[] UserGroups { get; set; }
        public Group[] Groups { get; set; }
 
        public string Status { get; set; }
        public string RealmName { get; set; }
        public string Login { get; set; }
        public string Language { get; set; }
        public string SyncId { get; set; }
        public bool Active { get; set; }
        public bool Locked { get; set; }
        public bool SabioUser { get; set; }

        //public Role[] Roles { get; set; }
        
    }
}