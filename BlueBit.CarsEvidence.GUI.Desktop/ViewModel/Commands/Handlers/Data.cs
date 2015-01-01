using BlueBit.CarsEvidence.BL;
using BlueBit.CarsEvidence.BL.Entities;
using BlueBit.CarsEvidence.GUI.Desktop.Configuration.Attributes;
using System;
using System.Runtime.Serialization;
using System.Xml;

namespace BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Commands.Handlers
{
    [RegisterAll(true)]
    public interface IDataCommandHandler :
        ICommandHandler
    {
        CmdKey Key { get; }
    }

    public abstract class DataCommandHandlerBase :
        IDataCommandHandler
    {
        [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
        public class DataHeader
        {
            [DataMember(Order=0)]
            public string AppName { get; set; }
            [DataMember(Order = 1)]
            public string AppVersion { get; set; }
            [DataMember(Order = 2, Name="CreateTime")]
            public DateTime DateTime { get; set; }
        }

        [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
        public abstract class DataBase
        {
        }
        
        [DataContract(Namespace = Consts.NamespaceEntities, IsReference = false, Name = "Root")]
        public class DataRoot<TData>
            where TData : DataBase
        {
            [DataMember(Order=0, Name="Header")]
            public DataHeader Header { get; set; }

            [DataMember(Order=1, Name="Data")]
            public TData Data { get; set; }
        }


        public abstract CmdKey Key { get; }

        public bool CanExecute()
        {
            return true;
        }

        public abstract void Execute();

        private static DataHeader CreateHeader()
        {
            var header = new DataHeader();
            var appNameVer = header.GetAppNameVersion();
            header.AppName = appNameVer.Item1;
            header.AppVersion = appNameVer.Item2.ToString();
            header.DateTime = DateTime.Now;
            return header;
        }

        protected static void Serialize<T>(T data, string path)
            where T : DataBase
        {
            var root = new DataRoot<T>()
            {
                Header = CreateHeader(),
                Data = data,
            };

            using (var writer = XmlWriter.Create(
                path,
                new XmlWriterSettings() { 
                    Indent = true,
                }))
            {
                var serializer = new DataContractSerializer(
                    typeof(DataRoot<T>), 
                    null, Int32.MaxValue, false, false, null);
                serializer.WriteObject(writer, root);
            }
        }

        protected static T DeSerialize<T>(string path)
            where T : DataBase
        {
            using (var reader = XmlReader.Create(
                path,
                new XmlReaderSettings()
                {
                }))
            {
                var knownTypes = new Type[] { 
                    typeof(Company)
                };

                var serializer = new DataContractSerializer(typeof(DataRoot<T>), knownTypes);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    { 
                        case XmlNodeType.Element:
                            if (serializer.IsStartObject(reader))
                            {
                                var root = (DataRoot<T>)serializer.ReadObject(reader);
                                return root.Data;
                            }
                            break;
                    }
                }
                return null;
            }
        }
    }
}
