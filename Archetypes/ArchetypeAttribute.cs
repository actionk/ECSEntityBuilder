using System;
using Plugins.ECSEntityBuilder.Worlds;

namespace Plugins.ECSEntityBuilder.Archetypes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ArchetypeAttribute : Attribute
    {
        public WorldType World { get; set; }

        public ArchetypeAttribute(WorldType world = WorldType.DEFAULT)
        {
            World = world;
        }
    }
}