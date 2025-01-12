// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore
{
    /// <summary>
    /// Localization provider builder interface to capture service collection and configuration context.
    /// </summary>
    public interface IDbLocalizationProviderBuilder
    {
        /// <summary>
        /// Service collection.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Configuration context.
        /// </summary>
        public ConfigurationContext Context { get; }
    }
}
