using System.Collections.Generic;
using Newtonsoft.Json;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Responses
{
    public class ConfigResponse : SabioResponse<ConfigResult>
    {}


    public class ConfigResult
    {
        public User User { get; set; }
        public System System { get; set; }
        public Privileges Privileges { get; set; }
        public Dictionary<string, object> State { get; set; }
        public Branch[] Branches { get; set; }
        public Theme Theme { get; set; }
        public Realm Realm { get; set; }
        public License License { get; set; }
    }

    public class User 
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Resource { get; set; }
        public string Language { get; set; }
        public string Login { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public bool SabioUser { get; set; }
        public Group[] Groups { get; set; }      
        public string ObjectType { get; set; }
        public Permission UserPermission { get; set; }
    }

    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Resource { get; set; }
    }

    public class System
    {
        public string Environment { get; set; }
        public bool OpenDocumentsInExternalViewer { get; set; }
        public bool Stateful { get; set; }
        public bool Tracking { get; set; }
        public Version Version { get; set; }
        public Logger Logger { get; set; }
        public Router Router { get; set; }
        public Product[] Products { get; set; }
        public bool SingleNodeAttachment { get; set; }
        public bool SingleTextFragment { get; set; }
        public bool EnableRating { get; set; }
        public bool EnableApprovals { get; set; }
        public bool EnableSubmissions { get; set; }
        public bool Multilanguage { get; set; }
        public bool CreateContactOnUserCreation { get; set; }
        public bool UserEmailIsMandatory { get; set; }
        public bool EnableTextTemplates { get; set; }
        public bool EnableMetaData { get; set; }
        public bool MessageFormDefaultUserGroupsToAll { get; set; }
        public bool MessageFromTextFormDefaultUserGroupsToAll { get; set; }
        public bool PasswordSecurity { get; set; }
        public bool UseUrlLoginToken { get; set; }
        public string[] TextFooterItems { get; set; }
        public bool EnableTeams { get; set; }
        public bool OneTeamPerGroup { get; set; }
        public string[] ContentLanguages { get; set; }
        public string DefaultContentLanguage { get; set; }
        public Tree Tree { get; set; }
        public Search Search { get; set; }
        public bool EnableExperimentalSearch { get; set; }
        public int ExportCsvRowLimit { get; set; }
        public bool ColorBranchName { get; set; }
        public Guide Guide { get; set; }
        public Analyser Analyser { get; set; }
        public bool EnableStructuredMetadataInUserGroups { get; set; }
        public bool ShowExportNightlyOptionInBranch { get; set; }
        public bool EnablePcs { get; set; }
        public bool EnableBranchHtmlExport { get; set; }
        public bool EnableUserlane { get; set; }
        public bool EnableSupportChat { get; set; }
        public bool UserSync { get; set; }
        public int SynonymsLimit { get; set; }
        public bool ExternalClientRememberContentViewSelection { get; set; }
        public bool EnableCustomCss { get; set; }
    }

    public class Version
    {
        public string Client { get; set; }
        public string Server { get; set; }
        public string Application { get; set; }
    }

    public class Logger
    {
        public string Level { get; set; }
        public bool Enabled { get; set; }
    }

    public class Router
    {
        public string Index { get; set; }
    }

    public class Tree
    {
        public bool Reload { get; set; }
        public int ReloadInterval { get; set; }
    }

    public class Search
    {
        public Autosearch Autosearch { get; set; }
        public Autosuggest Autosuggest { get; set; }
    }

    public class Autosearch
    {
        public int Delay { get; set; }
        public int MinChars { get; set; }
        public bool Enable { get; set; }
    }

    public class Autosuggest
    {
        public int Delay { get; set; }
        public int MinChars { get; set; }
        public bool Enable { get; set; }
        public int Limit { get; set; }
    }

    public class Guide
    {
        public bool PreconditionEnabled { get; set; }
    }

    public class Analyser
    {
        public string Path { get; set; }
        public string ResourceName { get; set; }
        public Analysermeta AnalyserMeta { get; set; }
    }

    public class Analysermeta 
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string RealmId { get; set; }
        public string LastModified { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public Navigation[] Navigation { get; set; }
    }

    public class Navigation 
    {
        public string Id { get; set; }
        public string RealmId { get; set; }
        public string LastModified { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public Child[] Children { get; set; }
        public string Resource { get; set; }
        public Filter[] Filter { get; set; }
        public object[] Sort { get; set; }
        public string NavigationType { get; set; }
        public int ChildOrder { get; set; }
        public Facetset[] FacetSets { get; set; }
        public string FilterList { get; set; }
        public string Text { get; set; }
        public string[] FacetList { get; set; }
        public string Cls { get; set; }
        public bool Leaf { get; set; }
        public string EventTargetType { get; set; }
        public string Route { get; set; }
    }

    public class Child 
    {
        public string Id { get; set; }
        public string Cls { get; set; }
        public string RealmId { get; set; }
        public string LastModified { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public object[] Children { get; set; }
        public string Resource { get; set; }
        public Filter[] Filter { get; set; }
        public object[] Sort { get; set; }
        public string EventTargetType { get; set; }
        public string ParentId { get; set; }
        public string NavigationType { get; set; }
        public int ChildOrder { get; set; }
        public Facetset[] FacetSets { get; set; }
        public string FilterList { get; set; }
        public string Text { get; set; }
        public string[] FacetList { get; set; }
        public bool Leaf { get; set; }
    }

  
    public class Facetset 
    {
        public string Id { get; set; }
        public string RealmId { get; set; }
        public string Created { get; set; }
        public string LastModified { get; set; }
        public string Name { get; set; }
        public string[] Facets { get; set; }
        public string EntityId { get; set; }
        public bool DefaultSet { get; set; }
    }

    public class Filter 
    {
        public string Id { get; set; }
        [JsonProperty("hierarchical")]
        public bool IsHierarchical { get; set; }
        [JsonProperty("multiselect")]
        public bool MultiSelect { get; set; }
        public string Title { get; set; }
        public string RealmId { get; set; }
        public string LastModified { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public bool NotFilter { get; set; }
        public string NavId { get; set; }
    }

    public class Privileges
    {
        public string[] Clientsetting { get; set; }
        public string[] Note { get; set; }
        public string[] Metadata { get; set; }
        public string[] Role { get; set; }
        public string[] Texttemplate { get; set; }
        public string[] Eop { get; set; }
        public string[] Document { get; set; }
        public string[] Branch { get; set; }
        public string[] Systemsettings { get; set; }
        public string[] Dialog { get; set; }
        public string[] Search { get; set; }
        public string[] Labelset { get; set; }
        public string[] Contact { get; set; }
        public string[] Theme { get; set; }
        public string[] Text { get; set; }
        public string[] Placeholder { get; set; }
        public string[] Guide { get; set; }
        public string[] Resourceversion { get; set; }
        public string[] Motd { get; set; }
        public string[] Statistic { get; set; }
        public string[] Question { get; set; }
        public string[] Apikey { get; set; }
        public string[] Approval { get; set; }
        public string[] Ruleset { get; set; }
        public string[] Tree { get; set; }
        public string[] Archive { get; set; }
        public string[] Label { get; set; }
        public string[] Message { get; set; }
        public string[] Version { get; set; }
        public string[] License { get; set; }
        public string[] External { get; set; }
        public string[] Dictionary { get; set; }
        public string[] Cancellation { get; set; }
        public string[] Report { get; set; }
        public string[] Usergroup { get; set; }
        public string[] Submission { get; set; }
        public string[] Contextdata { get; set; }
        public string[] User { get; set; }
    }

    public class Theme 
    {
        public string Id { get; set; }

        public string ColorProfileName { get; set; }
        public Colors Colors { get; set; }
        public string Created { get; set; }
        public string LastModified { get; set; }
        public string Name { get; set; }
        public string ObjectType { get; set; }
        public string PackageName { get; set; }
        public string Resource { get; set; }
        public string Title { get; set; }
        public Permission UserPermission { get; set; }
    }

    public class Colors
    {
        public string Treeitemactivebackgroundcolor { get; set; }
        public string Tabbackgroundcolor { get; set; }
        public string Tabcolor { get; set; }
        public string Treeitemactivecolor { get; set; }
        public string Linksintexts { get; set; }
        public string Applicationheaderbackgroundcolor { get; set; }
        public string Navigationitemselectcolor { get; set; }
        public string Headsandlinkscolor { get; set; }
        public string Navigationitemactivecolor { get; set; }
    }

    public class Realm 
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class License 
    {
        public string Id { get; set; }
    }

    public class Branch
    {
        public string Id { get; set; }
        public string Color { get; set; }
        public Theme Theme { get; set; }
        public string Title { get; set; }

    }

}