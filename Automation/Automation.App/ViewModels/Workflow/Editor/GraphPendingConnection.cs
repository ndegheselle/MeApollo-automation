﻿using Automation.App.Shared.ViewModels.Work;
using Automation.Shared.Data;
using System.Windows.Input;
using Usuel.Shared;

namespace Automation.App.ViewModels.Workflow.Editor
{
    public class GraphPendingConnection
    {
        private readonly GraphEditorViewModel _editor;
        private TaskConnector? _source;

        public GraphPendingConnection(GraphEditorViewModel editor)
        {
            _editor = editor;
            StartCommand = new DelegateCommand<TaskConnector>(source => _source = source);
            FinishCommand = new DelegateCommand<TaskConnector>(target =>
            {
                if (target == null || _source == null)
                    return;

                if (_source == target)
                {
                    _editor.RaiseAlert("Can't connect a connector to itself.");
                    return;
                }

                if (_source.Type != target.Type)
                {
                    _editor.RaiseAlert("Can't connect two connectors of different types.");
                    return;
                }

                if (_source.Direction == target.Direction)
                {
                    _editor.RaiseAlert("Can't connect two output or input connectors.");
                    return;
                }

                // If target is output, swap source and target
                if (target.Direction == EnumTaskConnectorDirection.Out)
                {
                    var temp = _source;
                    _source = target;
                    target = temp;
                }

                // If already exists delete the connection
                var existingConnection = _editor.Graph.Connections.FirstOrDefault(x => x.Source == _source && x.Target == target);
                if (existingConnection != null)
                {
                    _editor.Disconnect(existingConnection);
                    return;
                }

                _editor.Connect(_source, target);
            });
        }

        public ICommand StartCommand { get; }
        public ICommand FinishCommand { get; }
    }

}
