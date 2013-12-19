﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dev2.Studio.Core.AppResources.Enums;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.ViewModels.Navigation;
using Dev2.Studio.TO;

namespace Dev2.Studio.Deploy
{
    public interface IDeployStatsCalculator
    {
        /// <summary>
        ///     Calculates the stastics from navigation item view models
        /// </summary>
        void CalculateStats(IEnumerable<ITreeNode> items,
                            Dictionary<string, Func<ITreeNode, bool>> predicates,
                            ObservableCollection<DeployStatsTO> stats, out int deployItemCount);

        /// <summary>
        ///     The predicate used to detemine if an item should be deployed
        /// </summary>
        bool SelectForDeployPredicateWithTypeAndCategories(ITreeNode node,
                                                           ResourceType type, List<string> inclusionCategories,
                                                           List<string> exclusionCategories);

        /// <summary>
        ///     The predicate used to detemine if an item should be deployed
        /// </summary>
        bool SelectForDeployPredicate(ITreeNode node);

        /// <summary>
        ///     The predicate used to detemine which resources are going to be overridden
        /// </summary>
        bool DeploySummaryPredicateExisting(ITreeNode node,
                                            IEnvironmentModel targetEnvironment);

        /// <summary>
        ///     The predicate used to detemine which resources are going to be overridden
        /// </summary>
        bool DeploySummaryPredicateNew(ITreeNode node, IEnvironmentModel targetEnvironment);
    }
}