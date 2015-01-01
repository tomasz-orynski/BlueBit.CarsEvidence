using BlueBit.CarsEvidence.GUI.Desktop.Model.Objects;
using BlueBit.CarsEvidence.GUI.Desktop.Resources;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands
{
    public enum CmdKey
    {
        Add,
        Edit,
        Delete,
        Apply,
        Cancel,
        Generate,
        ShowList,

        Import,
        Export,

        Exit,
        Settings,
    }

    public sealed class CommandInfo :
        IInfo<CmdKey>
    {
        public CmdKey Key { get; set; }
        public ICommand Command { get; set; }
        public ICommandHandler CommandHandler { get; set; }


        private T GetResource<T>(string frmt)
        {
            return (T)App.Current.FindResource(string.Format(frmt, Key));
        }

        public string Name { get { return GetResource<string>(App.ResourceDictionary.StrCmd); } }
        public ImageSource ImageSource_S { get { return GetResource<BitmapImage>(App.ResourceDictionary.ImgCmd_S); } }
        public ImageSource ImageSource_N { get { return GetResource<BitmapImage>(App.ResourceDictionary.ImgCmd_N); } }
        public ImageSource ImageSource_L { get { return GetResource<BitmapImage>(App.ResourceDictionary.ImgCmd_L); } }
        public ImageSource ImageSource_16x16 { get { return GetResource<BitmapImage>(App.ResourceDictionary.ImgCmd_S).AsResized_16x16(); } }
        public Image Image_16x16 { get { return ImageSource_S.AsImage_16x16(); } }
    }

    public interface ICmdGrpKey
    {
        string NameKey { get; }
        string ImageSourceKey { get; }
    }

    public sealed class CommandsGroupInfo :
        IInfo<ICmdGrpKey>
    {
        public ICmdGrpKey Key { get; set; }
        public IEnumerable<CommandInfo> Commands { get; set; }

        private T GetResource<T>(string resKey) { return (T)App.Current.FindResource(resKey); }

        public string Name { get { return GetResource<string>(Key.NameKey); } }
        public ImageSource ImageSource { get { return GetResource<BitmapImage>(Key.ImageSourceKey); } }
    }
}
