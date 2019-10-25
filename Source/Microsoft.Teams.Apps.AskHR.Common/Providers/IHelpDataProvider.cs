// <copyright file="IHelpDataProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Common.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Teams.Apps.AskHR.Common.Models;

    /// <summary>
    /// IHelpDataProvider interface to provide methods to manage Help tile information from storages
    /// </summary>
    public interface IHelpDataProvider
    {
        /// <summary>
        /// Get already saved entity detail from storage table for Tags
        /// </summary>
        /// <returns><see cref="Task"/> Already saved entity detail</returns>
        Task<List<HelpInfoEntity>> GetHelpTilesAsync();

        /// <summary>
        /// Upload or update the files to azure blob storage from provided file path
        /// </summary>
        /// <param name="imageDirectoryPath">folder path</param>
        /// <returns>success/failure boolean</returns>
        Task<bool> StoreOrUpdateFileAsync(string imageDirectoryPath);

        /// <summary>
        /// Get image files from azure blob storage
        /// </summary>
        /// <returns>list of image name and blob path</returns>
        Task<List<ImageFileUrl>> GetImageFilesAsync();

        /// <summary>
        /// Insert or update helpInfo table entity
        /// </summary>
        /// <param name="helpInfoEntity">HelpInfo</param>
        /// <returns>true if entity saved successfully</returns>
        Task<bool> SaveOrUpdateEntityAsync(HelpInfoEntity helpInfoEntity);

        /// <summary>
        /// Delete help info entity
        /// </summary>
        /// <param name="rowKey">row key of storage</param>
        /// <returns>true if row deleted successfully</returns>
        Task<bool> DeleteEntityAsync(string rowKey);
    }
}
