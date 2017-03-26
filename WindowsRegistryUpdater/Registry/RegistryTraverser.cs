using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsRegistry
{
    public class RegistryTraverser
    {
        public Func<RegistryInfo, TraversedType, bool> Filter { get; set; } = (registryInfo, traversedType) => false;
        public Action<RegistryPath, Exception> ErrorHandler { get; set; } = (registryPath, exception) => { };
        public List<RegistryPath> Roots { get; set; } = new List<RegistryPath>();
        public CancellationToken Token { get; set; }

        private IEnumerable<RegistryInfo> FilterNode(RegistryPath root)
        {
            if (Token != null && Token.IsCancellationRequested)
                yield break;
            var subKeys = new List<string>();
            var subValues = new List<string>();
            RegistryKey rootKey = null;
            try
            {
                rootKey = root.OpenKey();
            }
            catch (Exception ex)
            {
                ErrorHandler(root, ex);
            }
            if (rootKey == null)
                yield break;
            try
            {
                subKeys = rootKey.GetSubKeyNames().ToList();
                subValues = rootKey.GetValueNames().ToList();

                foreach (var valueName in subValues)
                {
                    var registryInfo = new RegistryInfo(root, valueName);
                    if (Filter(registryInfo, TraversedType.InKeyName))
                        yield return registryInfo;
                    var kind = rootKey.GetValueKind(valueName);
                    if (kind == RegistryValueKind.Binary || kind == RegistryValueKind.None || kind == RegistryValueKind.Unknown || kind == RegistryValueKind.MultiString)
                        continue;//todo: GetValue(...).ToString failed. Fix it
                    var value = rootKey.GetValue(valueName).ToString();
                    registryInfo.Value.Assign(value);
                    if (Filter(registryInfo, TraversedType.InValue))
                        yield return registryInfo;
                }

                foreach (var keyName in subKeys)
                {
                    var keyPath = RegistryPath.Combine(root.Path, keyName);
                    var registryPath = new RegistryPath(root.Root, keyPath);
                    var registryInfo = new RegistryInfo(registryPath);
                    if (Filter(registryInfo, TraversedType.InPath))
                        yield return registryInfo;
                    foreach (var node in FilterNode(registryPath))
                        yield return node;
                }
            }
            finally
            {
                rootKey.Dispose();
            }
            //SecurityException
        }

        public IEnumerable<RegistryInfo> FilterAll
        {
            get
            {
                foreach (var root in Roots)
                    foreach (var node in FilterNode(root))
                        yield return node;
            }
        }

        public IEnumerable<KeyValuePair<TraversedType, RegistryInfo>> FilterAllClassified
        {
            get
            {
                return FilterAll.Select(_ => new KeyValuePair<TraversedType, RegistryInfo>(Classifiy(_),_));
            }
        }

        private TraversedType Classifiy(RegistryInfo registryInfo)
        {
            var result = (TraversedType)(registryInfo.ValueName/*.Value*/ != null ? 1 : 0) + (registryInfo.Value/*.Value*/ != null ? 2 : 0);
            return result;
        }
    }
}
