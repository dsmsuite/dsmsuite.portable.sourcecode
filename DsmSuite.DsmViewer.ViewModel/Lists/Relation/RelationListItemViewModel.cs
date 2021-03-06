using System;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.ViewModel.Lists.Relation
{
    public class RelationListItemViewModel : ReactiveViewModelBase, IComparable
    {
        public RelationListItemViewModel(IDsmApplication application, IDsmRelation relation)
        {
            Relation = relation;

            ConsumerName = relation.Consumer.Fullname;
            ConsumerType = relation.Consumer.Type;
            ProviderName = relation.Provider.Fullname;
            ProviderType = relation.Provider.Type;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;

            CycleType cycleType = application.IsCyclicDependency(relation.Consumer, relation.Provider);
            switch (cycleType)
            {
                case CycleType.Hierarchical:
                    Cyclic = "Hierarchical";
                    break;
                case CycleType.System:
                    Cyclic = "System";
                    break;
                case CycleType.None:
                default:
                    Cyclic = "";
                    break;
            }

            if (relation.Properties != null)
            {
                foreach (KeyValuePair<string, string> elementProperty in relation.Properties)
                {
                    Properties += string.Format("{0}={1} ", elementProperty.Key, elementProperty.Value);
                }
            }
        }

        public IDsmRelation Relation { get; set; }

        public int Index { get; set; }

        public string ConsumerName { get; }
        public string ConsumerType { get; }
        public string ProviderName { get; }
        public string ProviderType { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }
        public string Cyclic { get; }
        public string Properties { get; }

        public int CompareTo(object obj)
        {
            RelationListItemViewModel other = obj as RelationListItemViewModel;

            int compareConsumer = string.Compare(ConsumerName, other?.ConsumerName, StringComparison.Ordinal);
            int compareProvider = string.Compare(ProviderName, other?.ProviderName, StringComparison.Ordinal);

            if (compareConsumer != 0)
            {
                return compareConsumer;
            }
            else
            {
                return compareProvider;
            }
        }
    }
}
