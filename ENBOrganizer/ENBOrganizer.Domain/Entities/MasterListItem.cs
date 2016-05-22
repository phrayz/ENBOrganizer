﻿namespace ENBOrganizer.Domain.Entities
{
    public enum MasterListItemType
    {
        File,
        Directory
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class MasterListItem : EntityBase
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public MasterListItemType Type { get; set; }

        public MasterListItem() { } // Required for XML serialization.

        public MasterListItem(string name, MasterListItemType type)
            : base(name)
        {
            Type = type;
        }
    }
}
