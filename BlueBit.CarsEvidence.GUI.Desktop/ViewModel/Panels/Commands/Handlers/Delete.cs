﻿using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using BlueBit.CarsEvidence.GUI.Desktop.Model;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using dotNetExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels.Commands
{
    public interface IDeleteCommandHandler<T> :
        ICommandHandlerForSelected<ObjectBase>
        where T : EditObjectBase
    {
    }

    public class DeleteCommandHandler<T> :
        CommandHandlerForSelectedBase<ObjectBase>,
        IDeleteCommandHandler<T>
        where T : EditObjectBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<DeleteCommandHandler<T>>();
#endif
        private readonly IEditObjects<T> _editObjects;
        private readonly ISingletonCreatorForItem<EditDocumentViewModelBase<T>, T> _editDocumentViewModelCreator;

        public DeleteCommandHandler(
            IEditObjects<T> editObjects,
            ISingletonCreatorForItem<EditDocumentViewModelBase<T>, T> editDocumentViewModelCreator)
        {
            _editObjects = editObjects;
            _editDocumentViewModelCreator = editDocumentViewModelCreator;
        }

        protected override void OnExecute(IEnumerable<ObjectBase> objects)
        {
            if (MessageBox.Show("Czy chcesz usunąć?", "TODO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                objects
                    .Select(i => i.ID)
                    .ToArray()
                    .Each(_editObjects.Delete);
            }
        }

        protected override bool OnCanExecute(IEnumerable<ObjectBase> objects)
        {
            return objects.All(item =>
                 {
                    if (_editDocumentViewModelCreator.GetInstance(_editObjects.Get(item.ID)) != null) return false;
                    if (!_editObjects.CanDelete(item)) return false;
                    return true;
                });
        }
    }
}