﻿using System.Collections.Generic;
using UnityEngine;

namespace Ecs
{
    public class ComponentUpdate : IFixedUpdateSystem
    {
        private static List<IComponentFilter> _filters = new();
        private static List<EcsComponent> _currentComponents = new();

        private EventFilter<ComponentAdded> _addeds = new();
        private EventFilter<ComponentRemoved> _removeds = new();

        public static void AddFilter(IComponentFilter filter)
        {
            foreach (var savedFilter in _filters)
                if (savedFilter.GetComponentType() == filter.GetComponentType())
                    return;

            foreach (var component in _currentComponents)
                filter.AddComponent(component);

            _filters.Add(filter);
        }

        public static void RemoveFilter(IComponentFilter filter)
            => _filters.Remove(filter);

        public void FixedUpdate(float deltaTime)
        {
            foreach (var added in _addeds)
                AddComponent(added.Component);

            foreach (var removed in _removeds)
                RemoveComponent(removed.Component);
        }

        private void AddComponent(EcsComponent component)
        {
            _currentComponents.Add(component);

            foreach (var filter in _filters)
            {
                if (filter.GetComponentType() != component.GetType())
                    continue;

                filter.AddComponent(component);
                return;
            }
        }

        private void RemoveComponent(EcsComponent component)
        {
            _currentComponents.Remove(component);

            foreach (var filter in _filters)
            {
                if (filter.GetComponentType() != component.GetType())
                    continue;

                filter.RemoveComponent(component);
                return;
            }
        }
    }
}