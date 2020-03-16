using System;
using System.Collections.Generic;

namespace Plugins.ECSEntityBuilder.Variables
{
    public class EntityVariableMap
    {
        private readonly Dictionary<Type, IEntityVariable> m_variables = new Dictionary<Type, IEntityVariable>();

        public void Set<T>(T variable) where T : class, IEntityVariable
        {
            m_variables[typeof(T)] = variable;
        }

        public T Get<T>() where T : class, IEntityVariable
        {
            IEntityVariable result;
            m_variables.TryGetValue(typeof(T), out result);
            return result as T;
        }
    }
}