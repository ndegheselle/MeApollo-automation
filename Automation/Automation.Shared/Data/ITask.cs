﻿namespace Automation.Shared.Data
{
    public enum EnumTaskConnectorType
    {
        Data,
        Flow
    }

    public enum EnumTaskConnectorDirection
    {
        In,
        Out
    }

    public interface INamed
    {
        Guid Id { get; set; }
        string Name { get; }
    }

    public interface ITaskNode : INamed
    {
        TargetedPackage? Package { get; set; }
        Guid ScopeId { get; set; }
        IEnumerable<ITaskConnector> Inputs { get; }
        IEnumerable<ITaskConnector> Outputs { get; }
    }

    public interface ITaskConnector
    {
        Guid Id { get; }
        string Name { get; set; }
        EnumTaskConnectorType Type { get; set; }
        EnumTaskConnectorDirection Direction { get; set; }
    }
}
