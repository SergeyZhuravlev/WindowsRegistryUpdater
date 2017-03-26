using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistry
{
    public class RegistryPath: ICloneable
    {
        public RegistryPath(RegistryKey root, string path)
        {
            Root = root;
            Path = path;
        }

        public RegistryPath(RegistryPath root, string subKey):
            this(root.Root, RegistryPath.Combine(root.Path, subKey))
        {}

        public RegistryPath(string fullPath, string subKey):
            this(RegistryPath.Combine(fullPath, subKey))
        { }

        public RegistryPath(string fullPath)
        {
            var pathSplited = fullPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!pathSplited.Any())
                throw new ArgumentException($"Empty fullPath '{fullPath}'");
            var root = RootFactory(pathSplited[0]);
            var path = string.Join("\\", pathSplited.Skip(1));
            Root = root;
            Path = path;
        }

        private RegistryKey RootFactory(string root)
        {
            switch (root.ToUpper())
            {
                case "HU":
                case "HKEY_USERS":
                    return Registry.Users;
                case "HCU":
                case "HKEY_CURRENT_USER":
                    return Registry.CurrentUser;
                case "HLM":
                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;
                case "HCR":
                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;
                case "HCC":
                case "HKEY_CURRENT_CONFIG":
                    return Registry.CurrentConfig;
                default:
                    throw new ArgumentException($"Unsupported root '{root}'");
            }
        }

        public static string RootName(RegistryKey root)
        {
            if (root == Registry.Users)
                return "HU";
            else if (root == Registry.CurrentUser)
                return "HCU";
            else if (root == Registry.LocalMachine)
                return "HLM";
            else if (root == Registry.ClassesRoot)
                return "HCR";
            else if (root == Registry.CurrentConfig)
                return "HCC";
            else
                return root.Name;//throw new ArgumentException($"Unknown root '{root}'");
        }

        public RegistryKey Root { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return RegistryPath.Combine(RootName(Root), Path);
        }

        public RegistryKey OpenKey(bool writable = false)
        {
            return
                string.IsNullOrWhiteSpace(Path)
                ? RegistryKey.FromHandle(Root.Handle)
                : Root.OpenSubKey(Path, writable);
        }

        public static string Combine(string path1, string path2)
        {
            var path2Fixed = path2.TrimStart(new[] { '\\' });
            if (path1.Length == 0)
                return path2Fixed;
            if (!path1.EndsWith("\\"))
                return path1 + "\\" + path2Fixed;
            else
                return path1 + path2Fixed;
        }

        public object Clone()
        {
            return new RegistryPath(Root, string.Copy(Path));
        }
    }
}
