using System;
using Play.Common.Entities;

namespace Play.Inventory.Service.Models.Entities
{
    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
