﻿using System.Threading.Tasks;
using Tekly.Content;
using UnityEngine;

namespace Tekly.Glass
{
    public class Glass : MonoBehaviour
    {
        [SerializeField] private LayerManager m_layerManager;

        public async Task<T> DisplayModal<T>(string modalId) where T : Component
        {
            var instance = await DisplayModal(modalId);
            return instance.GetComponent<T>();
        }

        public async Task<GameObject> DisplayModal(string modalId)
        {
            var modalLayer = m_layerManager.Get("Modals");
            var modalRef = await ContentProvider.Instance.LoadAssetAsync<GameObject>(modalId);

            var instance = Instantiate(modalRef);
            modalLayer.Add(instance);

            return instance;
        }
    }
}