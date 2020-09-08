using System;
using Plugins.ECSEntityBuilder.Worlds;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ArchetypeAttribute : Attribute
    {
        public WorldType WorldType { get; set; }
    }
}