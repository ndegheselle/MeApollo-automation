﻿using Automation.Plugins.Shared;

namespace Automation.Plugins.Flow
{
    public class WaitDelay : ITask
    {
        public IProgress? Progress { get; set; }
        public async Task<Dictionary<string, object>?> ExecuteAsync(TaskContext context)
        {
            await Task.Delay(5000);
            return new Dictionary<string, object>()
            {
                { "test", "Wow that's some data." }
            };
        }
    }
}
