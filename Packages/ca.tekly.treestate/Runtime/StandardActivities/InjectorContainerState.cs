﻿using System;
using Tekly.Common.Utils;
using Tekly.Injectors;
using Tekly.Logging;
using UnityEngine;

namespace Tekly.TreeState.StandardActivities
{
   
    
    public class InjectorContainerState : TreeStateActivity
    {
        [Tooltip("The container will use this ref as the parent otherwise it will search up the hierarchy for a parent")]
        public InjectorContainerRef ParentContainer;
        
        [Tooltip("The container will initialize the ref with itself")]
        public InjectorContainerRef SelfContainer;
        public ScriptableBinding[] ScriptableBindings;
        
        public InjectorContainer Container { get; private set; }
        
        private InjectorContainer m_parentContainer;
        private IInjectionProvider[] m_providers;
        private ScriptableBinding[] m_instances;
        
        protected override void Awake()
        {
            base.Awake();
            m_providers = GetComponents<IInjectionProvider>();
        }
        
        protected override void PreLoad()
        {
            if (ParentContainer != null) {
                m_parentContainer = ParentContainer.Value;
            } else {
                var parent = transform.GetComponentInAncestor<InjectorContainerState>();
                if (parent != null) {
                    m_parentContainer = parent.Container;
                }
            }
            
            if (ScriptableBindings != null && ScriptableBindings.Length > 0 && (m_instances == null || m_instances.Length == 0)) {
                Array.Resize(ref m_instances, ScriptableBindings.Length);
                for (var index = 0; index < ScriptableBindings.Length; index++) {
                    var scriptableInjector = ScriptableBindings[index];
                    m_instances[index] = Instantiate(scriptableInjector);
                }
            }
            
            Container = new InjectorContainer(m_parentContainer);
            
            if (m_instances != null) {
                foreach (var scriptableInjector in m_instances) {
                    Container.Inject(scriptableInjector);
                    scriptableInjector.Bind(Container);
                }
            }
            
            foreach (var provider in m_providers) {
                provider.Provide(Container);
            }

            if (SelfContainer != null) {
                SelfContainer.Initialize(Container);
            }
        }

        protected override void InactiveStarted()
        {
            if (m_instances != null) {
                foreach (var scriptableInjector in m_instances) {
                    Container.Clear(scriptableInjector);
                }
            }
            
            Container = null;   
            
            if (SelfContainer != null) {
                SelfContainer.Clear();
            }
        }
    }
}