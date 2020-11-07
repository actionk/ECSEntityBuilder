using Plugins.ECSEntityBuilder.Variables;
using Unity.Entities;

namespace Plugins.ECSEntityBuilder.Steps
{
    public class SetNameStep : IEntityBuilderStep
    {
        private string m_name;

        public void SetValue(string name)
        {
            m_name = name;
        }

        public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, Entity entity)
        {
#if UNITY_EDITOR
            wrapper.SetName(entity, m_name);
#endif
        }
    }
}