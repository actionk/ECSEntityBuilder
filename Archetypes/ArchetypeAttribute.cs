using System;
using Plugins.Shared.ECSEntityBuilder.Worlds;

namespace Plugins.Shared.ECSEntityBuilder.Archetypes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ArchetypeAttribute : Attribute
    {
        public WorldType WorldType { get; set; }
    }
}