using System;
using System.Xml.Serialization;

namespace MOD4.Web.DomainService.Entity
{
    public class TestEntity
    {
        [XmlElement(ElementName = "testele")]
        public string testele { get; set; }
    }
}
