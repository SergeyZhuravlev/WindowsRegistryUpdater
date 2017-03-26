using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistry;
using System.Globalization;
using Common;
using WindowsRegistryUpdater.Info;
using Cached;
using CaseSensitiveTraitor = System.Func<string, string>;
using PatternOperationInfoTraited = WindowsRegistryUpdater.Info.PatternOperationInfoGeneric<System.Func<string, string>>;
using UIHelpers;
using System.Threading;
using System.Diagnostics;

namespace WindowsRegistryUpdater.Operations
{
    public class DoTraverseRegistry
    {
        private HashSet<TraversedType> TraverseTypesSet { get; set; }
        private CancellationToken Token { get; set; }
        private PatternOperationInfoTraited Pattern { get; set; }

        public DoTraverseRegistry(PatternOperationInfo pattern, HashSet<TraversedType> traverseTypesSet, CancellationToken token)
        {
            var caseSensitiveTraiter = pattern.CaseSensitive ? new CaseSensitiveTraitor(_ => _) : new CaseSensitiveTraitor(_ => _.ToLower(CultureInfo.CurrentCulture));
            Pattern = new PatternOperationInfoTraited(
                caseSensitiveTraiter,
                caseSensitiveTraiter(pattern.Pattern));
            Token = token;
            TraverseTypesSet = traverseTypesSet;
        }

        public void Execute(OperationalInfo info, Collection<string> traverseErrors)
        {
            try
            {
                var registryTraverser = new RegistryTraverser();
                registryTraverser.ErrorHandler = (registryNode, error) => UI.Invoke(() =>
                    traverseErrors.Add(error.Message.Trim(new[] { '.' }) + " in " + registryNode));
                registryTraverser.Roots = new List<RegistryPath> { new RegistryPath("HLM"), new RegistryPath("HU") };
                registryTraverser.Filter = FilterForTraverse;
                registryTraverser.Token = Token;
                /*var traversedRegistry = CachedEnumerable.Make(registryTraverser.FilterAllClassified);
                foreach (var value in traversedRegistry.Enumerate.Where(_ => _.Key == TraversedType.InValue))
                    UI.Invoke(()=>info.ValuesToChangings.Add(new OperationalElement(value.Key, value.Value)));
                foreach (var value in traversedRegistry.Enumerate.Where(_ => _.Key == TraversedType.InKeyName))
                    UI.Invoke(()=>info.NamesToChangings.Add(new OperationalElement(value.Key, value.Value)));
                foreach (var value in traversedRegistry.Enumerate.Where(_ => _.Key == TraversedType.InPath))
                    UI.Invoke(()=>info.PathesToChangings.Add(new OperationalElement(value.Key, value.Value)));*/
                foreach (var node in registryTraverser.FilterAllClassified.Where(_ => TraverseTypesSet.Contains(_.Key)))
                {
                    if (Token.IsCancellationRequested)
                        return;
                    switch (node.Key)
                    {
                        case TraversedType.InValue:
                            UI.Invoke(() => info.ValuesToChangings.Add(new OperationalElement(node.Key, node.Value)));
                            break;
                        case TraversedType.InKeyName:
                            UI.Invoke(() => info.NamesToChangings.Add(new OperationalElement(node.Key, node.Value)));
                            break;
                        case TraversedType.InPath:
                            UI.Invoke(() => info.PathesToChangings.Add(new OperationalElement(node.Key, node.Value)));
                            break;
                    }
                    if (Token.IsCancellationRequested)
                        return;
                }
            }
            catch (Exception ex)
            {
                UI.Invoke(()=> traverseErrors.Add("Fatal error interruption " + ex));
            }
        }

        private bool FilterForTraverse(RegistryInfo node, TraversedType type)
        {
            switch (type)
            {
                case TraversedType.InPath:
                    return Pattern.CaseSensitive(node.Path.ToString()).Contains(Pattern.Pattern);
                case TraversedType.InKeyName:
                    return Pattern.CaseSensitive(node.ValueName.Value).Contains(Pattern.Pattern);
                case TraversedType.InValue:
                    return Pattern.CaseSensitive(node.Value.Value).Contains(Pattern.Pattern);
                default:
                    throw new Exception("Unknown TraversedType");
            }
        }
    }
}
