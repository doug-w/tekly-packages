﻿using UnityEngine;

namespace Tekly.Common.Utils
{
    public static class PrefabProtector
    {
        private static Transform s_container;

        private static Transform GetContainer()
        {
            if (s_container == null) {
                var go = new GameObject("[PrefabProtector]");
                Object.DontDestroyOnLoad(go);
                s_container = go.transform;
            }

            return s_container;
        }
        
        public static T Protect<T>(T component) where T : Component
        {
#if UNITY_EDITOR
            var gameObject = component.gameObject;
            
            if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(gameObject)) {
                gameObject.SetActive(false);
                return component;
            }

            var wasActive = gameObject.activeSelf;
            
            gameObject.SetActive(false);
            var instance = Object.Instantiate(component, GetContainer());
            instance.gameObject.name = gameObject.name;
            gameObject.SetActive(wasActive);

            return instance;
#else
            component.gameObject.SetActive(false);
            return component;
#endif
        }

        public static GameObject Protect(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(gameObject)) {
                gameObject.SetActive(false);
                return gameObject;
            }
            
            var wasActive = gameObject.activeSelf;
            
            gameObject.SetActive(false);
            var instance = Object.Instantiate(gameObject, GetContainer());
            instance.gameObject.name = gameObject.name;
            gameObject.SetActive(wasActive);

            return instance;
#else
            gameObject.SetActive(false);
            return gameObject;
#endif
        }
    }
}