﻿using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Senparc.ProjectFileManager.Models
{
    /// <summary>
    /// .csproj file items in the first &lt;PropertyGroup&gt; tag
    /// </summary>
    public class PropertyGroup : INotifyPropertyChanged
    {
        public string TargetFramework { get; set; }
        public string TargetFrameworks { get; set; }
        public string Version { get; set; }
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public string PackageTags { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
        public string PackageLicenseUrl { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ProjectUrl { get; set; }
        public string PackageProjectUrl { get; set; }
        public string PackageIconUrl { get; set; }
        public string PackageReleaseNotes { get; set; }
        public string RepositoryUrl { get; set; }

        #region Additional Information
        public XElement OriginalElement { get; set; }

        public string FileName { get; set; }
        public string FullFilePath { get; set; }

        #endregion

        #region INotifyPropertyChanged functions

        private void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;

            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        /// <summary>
        /// Create a PropertyGroup entity from XML.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public static PropertyGroup GetObjet(XElement element, string fullFilePath)
        {
            var projectFile = Senparc.CO2NET.Utilities.XmlUtility.Deserialize<PropertyGroup>(element.ToString()) as PropertyGroup;
            if (projectFile == null)
            {
                throw new Exception("invalid xml file for new .csproj file format!");
            }
            projectFile.OriginalElement = element;
            projectFile.FullFilePath = fullFilePath;
            projectFile.FileName = Path.GetFileName(fullFilePath);
            return projectFile;
        }

        public void Save()
        {
            var doc = XDocument.Load(FullFilePath);
            var propertyGroup = doc.Root.Elements("PropertyGroup").FirstOrDefault();
            if (propertyGroup == null)
            {
                return;
            }

            FillXml(propertyGroup, "TargetFramework", TargetFramework);
            FillXml(propertyGroup, "TargetFrameworks", TargetFrameworks);
            FillXml(propertyGroup, "Version", Version);
            FillXml(propertyGroup, "AssemblyName", AssemblyName);
            FillXml(propertyGroup, "RootNamespace", RootNamespace);
            FillXml(propertyGroup, "Description", Description);
            FillXml(propertyGroup, "Copyright", Copyright);
            FillXml(propertyGroup, "PackageTags", PackageTags);
            FillXml(propertyGroup, "Authors", Authors);
            FillXml(propertyGroup, "Owners", Owners);
            FillXml(propertyGroup, "PackageLicenseUrl", PackageLicenseUrl);
            FillXml(propertyGroup, "Title", Title);
            FillXml(propertyGroup, "Summary", Summary);
            FillXml(propertyGroup, "ProjectUrl", ProjectUrl);
            FillXml(propertyGroup, "PackageProjectUrl", PackageProjectUrl);
            FillXml(propertyGroup, "PackageIconUrl", PackageIconUrl);

            PackageReleaseNotes = PackageReleaseNotes.TrimEnd() + Environment.NewLine;
            FillXml(propertyGroup, "PackageReleaseNotes", PackageReleaseNotes);
            FillXml(propertyGroup, "RepositoryUrl", RepositoryUrl);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
            var encoding = new UTF8Encoding(false);
            xws.Encoding = encoding;
            using (XmlWriter xw = XmlWriter.Create(FullFilePath, xws))
            {
                doc.Save(xw);
            }

        }

        private void FillXml(XElement propertyGroupElement, string elementName, string value)
        {
            if (value.IsNullOrEmpty())
            {
                return;
            }

            var element = propertyGroupElement.Element(elementName);
            if (element == null)
            {
                element = new XElement(elementName, value);
                propertyGroupElement.Add(element);
            }
            else
            {
                element.SetValue(value);
            }
        }
    }
}
