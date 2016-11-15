using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SRogue.Core.Modules
{
    public class EntityLoader
    {
        protected Dictionary<Type, XmlDocument> Cache = new Dictionary<Type, XmlDocument>();

        protected const bool CreateNewInstanceIfLoadingFailed = true;
        protected const bool CreateNewFileIfLoadingFailed = true;

        public TType Load<TType>()
            where TType : class, IEntity
        {
            var xml = string.Empty;

            if (!Cache.ContainsKey(typeof(TType)))
            {
                var path = "res/{0}.xml".FormatWith(typeof(TType).Name);
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        xml = reader.ReadToEnd();
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);

                        Cache.Add(typeof(TType), doc);
                    }
                }
                catch (Exception ex)
                {
                    using (var writer = new StreamWriter("log.txt", true))
                    {
                        writer.WriteLine("Exception while reading entity data:");
                        writer.WriteLine("File: {0}".FormatWith(path));
                        writer.WriteLine("Exception: {0}".FormatWith(ex.Message));
                        writer.WriteLine("Stack Trace: {0}".FormatWith(ex.StackTrace));
                    }
                    var instance = Activator.CreateInstance<TType>();
                    if (CreateNewFileIfLoadingFailed)
                    {
                        using (var writer = new StreamWriter(path))
                        {
                            writer.Write(instance.Serialize());
                        }
                    }
                    if (CreateNewInstanceIfLoadingFailed)
                    {
                        return instance; 
                    }
                    throw ex;
                }
                
            }

            return Cache[typeof(TType)].DeserializeAs<TType>();
        }
    }
}
