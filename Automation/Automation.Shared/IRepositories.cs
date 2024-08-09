﻿using Automation.Shared.Base;
using Automation.Shared.Data;
using System.Dynamic;

namespace Automation.Shared
{
    public interface ICrudRepository<T>
    {
        public Task<T?> GetByIdAsync(Guid id);
        public Task CreateAsync(T element);
        public Task UpdateAsync(T element);
        public Task DeleteAsync(Guid id);
    }

    public interface ITaskRepository<T> : ICrudRepository<T> where T : ITaskNode
    { }

    public interface IWorkflowRepository<T> : ICrudRepository<T> where T : IWorkflowNode
    { }

    public interface IScopeRepository<T> : ICrudRepository<T> where T : IScope
    {
        public Task<T> GetRootAsync();
    }

    public interface IHistoryRepository<T> where T : ITaskHistory
    {
        public Task<PageWrapper<T>> GetByTaskAsync(Guid taskId, int page, int pageSize);
        public Task<PageWrapper<T>> GetByScopeAsync(Guid scopeId, int page, int pageSize);
    }
}
