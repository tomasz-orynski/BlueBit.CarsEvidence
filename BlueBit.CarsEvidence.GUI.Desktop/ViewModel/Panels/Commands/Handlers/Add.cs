﻿using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands.Handlers
{
    public interface IAddCommandHandler :
        ICommandHandler,
        IObjectForEntityType
    {
    }

    public interface IAddCommandHandler<T> :
        IAddCommandHandler
        where T : EditDocumentObjectBase
    {
    }

    public class AddCommandHandler<T> :
        IAddCommandHandler<T>
        where T : EditDocumentObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<AddCommandHandler<T>>();
#endif
        private readonly IOpenEditDocumentCommandHelper<T> _helper;

        public EntityType ForType { get { return EntityTypeDict.GetValueForObjectType<T>(); } }

        public AddCommandHandler(
            IOpenEditDocumentCommandHelper<T> helper
            )
        {
            _helper = helper;
        }

        public void Execute()
        {
            _helper.NewEditDocument();
        }

        public bool CanExecute()
        {
            return true;
        }
    }
}
