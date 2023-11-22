using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml.Serialization;
using System.Xml;
using DConfigOS_Core.Providers.HttpContextProviders;

namespace DConfigOS_Core.Models
{
    public enum ContentTypes { Page = 0, Partial = 1, Redirect = 2, Download = 3, Resource = 4 }

    [Serializable]
    public class Content : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [XmlIgnore]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        [XmlAttribute("name")]
        public string Name { get; set; }
        public int? DomainId { get; set; }
        [JsonIgnore]
        public virtual Domain Domain { get; set; }
        [NotMapped]
        public virtual List<string> DomainAliases
        {
            get
            {
                if (Domain != null && !String.IsNullOrEmpty(Domain.DomainAliases))
                {
                    string[] aliases = Domain.DomainAliases.TrimStart(';').TrimEnd(';').Split(';');
                    return aliases.ToList();
                }
                return null;
            }
        }
        public int? ViewTypeId { get; set; }
        [JsonIgnore]
        public virtual ViewType ViewType { get; set; }
        [NotMapped]
        [JsonIgnore]
        [XmlAttribute("viewType")]
        public string ViewTypeName { get; set; }
        [Required]
        [XmlAttribute("online")]
        public bool Online { get; set; }
        [Required]
        [XmlAttribute("plenyChildren")]
        public bool PlentyChildren { get; set; }
        [Index(IsUnique = false)]
        public int? ParentId { get; set; }
        [JsonIgnore]
        public virtual Content Parent { get; set; }
        [MaxLength(250)]
        [XmlAttribute("urlName")]
        public string UrlName { get; set; }
        [Required]
        [XmlAttribute("priority")]
        public int Priority { get; set; }
        public int? PublicViewPackageId { get; set; }
        [JsonIgnore]
        //[ForeignKey("PublicViewPackageId")]
        public virtual PublicViewsPackage PublicViewsPackage { get; set; }
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlAttribute("contentType")]
        public ContentTypes ContentType { get; set; }
        [XmlArray("contents")]
        [XmlArrayItem("content", typeof(Content))]
        [JsonIgnore]
        public virtual ICollection<Content> ChildrenContents { get; set; }
        [XmlArray("contentInstances")]
        [XmlArrayItem("contentInstance", typeof(ContentInstance))]
        [JsonIgnore]
        public virtual ICollection<ContentInstance> ContentInstances { get; set; }
        [NotMapped]
        public ICollection<ViewType> PossibleViewTypes { get; set; }
        [NotMapped]
        public ICollection<ViewType> PossibleChildViewTypes { get; set; }
        [NotMapped]
        public ICollection<ViewTemplate> PossibleChildViewTemplates { get; set; }
        [Index()]
        [MaxLength(2048)]
        public string UrlFullCode { get; set; }
        [NotMapped]
        [JsonIgnore]
        private ContentInstance _activeContentInstance;
        [NotMapped]
        [JsonIgnore]
        public ContentInstance ActiveContentInstance
        {
            get
            {
                string language = DConfigRequestContext.Current.Language;
                int? version = DConfigRequestContext.Current.Version;
                if (_activeContentInstance == null && this.ContentInstances != null)
                {
                    _activeContentInstance = this.ContentInstances.Where(m => m.Online == true && (m.Language == language || m.Language == null) && (version == null || m.Version == version)).FirstOrDefault();
                }
                return _activeContentInstance;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public string Path
        {
            get
            {
                string language = DConfigRequestContext.Current.Language;
                int? version = DConfigRequestContext.Current.Version;
                if (String.IsNullOrEmpty(UrlFullCode))
                {
                    return language;
                }
                else
                {
                    if (String.IsNullOrEmpty(language))
                    {
                        return UrlFullCode;
                    }
                    else
                    {
                        return '/' + language + UrlFullCode;
                    }
                }
            }

        }

        public string CreatorId { get; set; }
        [JsonIgnore]
        public virtual SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser Creator { get; set; }

        [NotMapped]
        public DateTime? CreateDate { get { return CreatedDate; } set { CreatedDate = value; } }

        public DateTime? DueDate { get; set; }

        public int? StageId { get; set; }
        public virtual Stage Stage { get; set; }

        [NotMapped]
        public string CurrentStages
        {
            get
            {
                if (ContentInstances != null)
                {
                    StringBuilder stages = new StringBuilder();
                    Dictionary<string, ContentInstance> LangVersionDic = new Dictionary<string, ContentInstance>();
                    foreach (ContentInstance c in ContentInstances)
                    {
                        if (c.StageId.HasValue)
                        {
                            if (LangVersionDic.ContainsKey(c.Language) && LangVersionDic[c.Language].Version < c.Version)
                            {
                                LangVersionDic[c.Language] = c;
                            }
                            else
                            {
                                LangVersionDic.Add(c.Language, c);
                            }
                        }
                    }
                    foreach (ContentInstance c in LangVersionDic.Values)
                    {
                        stages.Append(c.Stage.Name + "(" + c.Language + ");");
                    }
                    return stages.ToString();
                }
                return null;
            }
        }

        [NotMapped]
        public class Lang_Comment
        {
            public string lang;
            public string comment;
        }

        [NotMapped]
        public class Lang_Curr_Prev_Instances
        {
            public string lang;
            public string currentInstance;
            public string prevInstance;
        }

        [NotMapped]
        public List<Lang_Curr_Prev_Instances> CurrentPreviousStages
        {
            get
            {
                if (ContentInstances != null)
                {
                    List<Lang_Curr_Prev_Instances> returnedData = new List<Lang_Curr_Prev_Instances>();

                    List<string> languages = ContentInstances.Select(x => x.Language).Distinct().ToList();
                    foreach (var lang in languages)
                    {
                        Lang_Curr_Prev_Instances obj = new Lang_Curr_Prev_Instances();
                        obj.lang = lang;

                        int maxVersion = ContentInstances.Where(x => x.Language == lang).Max(n => n.Version);
                        if (maxVersion >= 1)
                            obj.currentInstance = ContentInstances.Where(x => x.Language == lang && x.Version == maxVersion).FirstOrDefault().Stage.Name;
                        if (maxVersion > 1)
                            obj.prevInstance = ContentInstances.Where(x => x.Language == lang && x.Version == maxVersion - 1).FirstOrDefault().Stage.Name;

                        returnedData.Add(obj);
                    }

                    return returnedData;
                }
                return null;
            }
        }

        [NotMapped]
        public List<Lang_Comment> CurrentComments
        {
            get
            {
                if (ContentInstances != null)
                {
                    List<Lang_Comment> returnedData = new List<Lang_Comment>();

                    List<string> languages = ContentInstances.Select(x => x.Language).Distinct().ToList();
                    foreach (var lang in languages)
                    {
                        Lang_Comment obj = new Lang_Comment();
                        obj.lang = lang;
                        int maxVersion = ContentInstances.Where(x => x.Language == lang).Max(n => n.Version);
                        if (maxVersion >= 1)
                            obj.comment = ContentInstances.Where(x => x.Language == lang && x.Version == maxVersion).FirstOrDefault().Comments;

                        returnedData.Add(obj);
                    }
                    return returnedData;
                }
                return null;
            }
        }
    }
}

