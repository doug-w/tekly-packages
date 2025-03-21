﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tekly.Content
{
    public class SingleContentOperation<T> : IContentOperation<T> where T : Object
    {
        public Task<T> Task => m_task ??= GetTask();
        public bool HasError => m_handle.Status == AsyncOperationStatus.Failed;
        public bool IsDone => m_handle.IsDone;
        public T Result
        {
            get {
                if (m_handle.Result != null && m_handle.Result.Count > 0) {
                    return m_handle.Result[0];
                }

                return null;
            }
        }

        private Task<T> m_task;
        private readonly AsyncOperationHandle<IList<T>> m_handle;

        public SingleContentOperation(AsyncOperationHandle<IList<T>> handle)
        {
            m_handle = handle;
        }

        public void Release()
        {
            Addressables.Release(m_handle);
        }
        
        public TaskAwaiter<T> GetAwaiter()
        {
            return Task.GetAwaiter();
        }

        private async Task<T> GetTask()
        {
            var result = await m_handle.Task;

            if (m_handle.Status == AsyncOperationStatus.Failed) {
                return null;
            }

            return result[0];
        }
    }
}