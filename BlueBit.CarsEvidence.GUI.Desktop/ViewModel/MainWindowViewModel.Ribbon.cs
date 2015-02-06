using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers;
using dotNetExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel
{
    partial class MainWindowViewModel
    {
        private class EntityCmdGrpKey :
            ICmdGrpKey
        {
            public EntityType Value { get; set; }

            public string NameKey { get { return string.Format(App.ResourceDictionary.StrObjects, Value); } }
            public string ImageSourceKey { get { return string.Format(App.ResourceDictionary.ImgObjects, Value); } }
        }

        private enum RibbonCmdGrpType
        {
            Data,
        }

        private class RibbonCmdGrpKey :
            ICmdGrpKey
        {
            public RibbonCmdGrpType Value { get; set; }

            public string NameKey { get { return string.Format(App.ResourceDictionary.StrRibbonGroups, Value); } }
            public string ImageSourceKey { get { return string.Format(App.ResourceDictionary.ImgRibbonGroups, Value); } }
        }


        private static CommandsGroupsViewModel CreateRepositoryCommandsGroups(
            Func<IEnumerable<IShowCommandHandler>> showCommands, 
            Func<IEnumerable<IAddCommandHandler>> addCommands,
            Func<IEnumerable<IEditAllCommandHandler>> editAllCommands)
        {
            var showCmds = showCommands()
                .ToDictionary(_ => _.ForType, _ => CreateCommand(CmdKey.ShowList, _));
            var addCmds = addCommands()
                .ToDictionary(_ => _.ForType, _ => CreateCommand(CmdKey.Add, _));
            var editAllCmds = editAllCommands()
                .Where(_ => !showCmds.ContainsKey(_.ForType))
                .ToDictionary(_ => _.ForType, _ => CreateCommand(CmdKey.Edit, _));
            Contract.Assert(showCmds.Count == addCmds.Count);
            var cmds = showCmds.Union(addCmds).Union(editAllCmds)
                .GroupBy(_ => _.Key, _ => _.Value)
                .OrderBy(_ => App.VisibleTypesOrder[_.Key])
                .Select(_ => new CommandsGroupInfo
                {
                    Key = new EntityCmdGrpKey() { Value = _.Key },
                    Commands = _.ToArray()
                });

            //Wyświetlenie zakładek.
            showCmds
                .OrderBy(_ => App.VisibleTypesOrder[_.Key])
                .Each(_ => _.Value.Command.Execute(null));
            return new CommandsGroupsViewModel(cmds);
        }
        private static CommandsGroupsViewModel CreateRepositoryExtraCommandsGroups(
            Func<IEnumerable<IDataCommandHandler>> dataCommands)
        {
            var cmds = Enumerable.Range(1, 1)
                .Select(_ => new CommandsGroupInfo
                {
                    Key = new RibbonCmdGrpKey() { Value = RibbonCmdGrpType.Data },
                    Commands = dataCommands()
                        .Select(cmd => CreateCommand(cmd.Key, cmd))
                        .ToArray()
                });

            return new CommandsGroupsViewModel(cmds);
        }
    }
}
