// <copyright file="HelpDataProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Common.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Microsoft.Teams.Apps.AskHR.Common.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// HelpDataProvider.
    /// </summary>
    public class HelpDataProvider : IHelpDataProvider
    {
        private const string PartitionKey = "HelpInfo";

        private readonly Lazy<Task> initializeTableTask;
        private readonly Lazy<Task> initializeBlobTask;

        private CloudTable helpInfoCloudTable;
        private CloudBlobContainer helpInfoImageCloudBlobContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpDataProvider"/> class.
        /// HelpDataProvider constructor.
        /// </summary>
        /// <param name="connectionString">azure storage connection string.</param>
        public HelpDataProvider(string connectionString)
        {
            this.initializeTableTask = new Lazy<Task>(() => this.InitializeTableAsync(connectionString));
            this.initializeBlobTask = new Lazy<Task>(() => this.InitializeBlobAsync(connectionString));
        }

        /// <summary>
        /// Get help tile entities
        /// </summary>
        /// <returns>list of help tile</returns>
        public async Task<List<HelpInfoEntity>> GetHelpTilesAsync()
        {
            try
            {
                await this.EnsureInitializedAsync();

                var result = new List<HelpInfoEntity>();
                var query = new TableQuery<HelpInfoEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey));

                TableContinuationToken tableContinuationToken = null;

                do
                {
                    var queryResponse = await this.helpInfoCloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                    tableContinuationToken = queryResponse.ContinuationToken;
                    result.AddRange(queryResponse.Results);
                }
                while (tableContinuationToken != null);

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveOrUpdateEntityAsync(HelpInfoEntity helpInfoEntity)
        {
            try
            {
                helpInfoEntity.PartitionKey = PartitionKey;
                var result = await this.StoreOrUpdateEntityAsync(helpInfoEntity);
                return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete help info entity based on row key passed
        /// </summary>
        /// <param name="rowKey">row key of help tile</param>
        /// <returns>true if delete is successful</returns>
        public async Task<bool> DeleteEntityAsync(string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<HelpInfoEntity>(PartitionKey, rowKey);
                TableResult result = await this.helpInfoCloudTable.ExecuteAsync(retrieveOperation);
                HelpInfoEntity helpInfoEntity = (HelpInfoEntity)result?.Result;

                TableOperation deleteOperation = TableOperation.Delete(helpInfoEntity);
                var deleteResponse = await this.helpInfoCloudTable.ExecuteAsync(deleteOperation);

                return deleteResponse.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
             catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Method for uploading file to Azure Blob Storage.
        /// </summary>
        /// <param name="imageDirectoryPath">Local file path</param>
        /// <returns>URL of uploaded file.</returns>
        public async Task<bool> StoreOrUpdateFileAsync(string imageDirectoryPath)
        {
            try
            {
                // This code can be used for multiple file upload inside a folder
                DirectoryInfo directoryInfo = new DirectoryInfo(imageDirectoryPath);
                List<FileInfo> supportedImageFiles = new List<FileInfo>();
                var files = directoryInfo.EnumerateFiles();

                foreach (var file in files)
                {
                    if (Regex.IsMatch(file.Name, @".jpg|.jpeg|.png$"))
                    {
                        supportedImageFiles.Add(file);
                    }
                }

                await this.EnsureInitializedAsync();

                // foreach when running for multiple files
                foreach (FileInfo inputFile in supportedImageFiles)
                {
                    var blob = this.helpInfoImageCloudBlobContainer.GetBlockBlobReference(inputFile.Name);
                    await blob.UploadFromFileAsync(inputFile.FullName);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// Get the list of images from azure blob
        /// </summary>
        /// <returns>list of image file URL model</returns>
        public async Task<List<ImageFileUrl>> GetImageFilesAsync()
        {
            var imageDetails = new List<ImageFileUrl>();

            try
            {
                await this.EnsureInitializedAsync();

                var blobContainer = await this.helpInfoImageCloudBlobContainer.ListBlobsSegmentedAsync(null);
                foreach (var item in blobContainer.Results)
                {
                    if (Regex.IsMatch(item.Uri.Segments[2], @".jpg|.jpeg|.png$"))
                    {
                        ImageFileUrl fileUrl = new ImageFileUrl();
                        fileUrl.ImageName = item.Uri.Segments[2].Split('.')[0];
                        fileUrl.ImageBlobUrl = item.Uri.AbsoluteUri;
                        imageDetails.Add(fileUrl);
                    }
                }

                return imageDetails;
            }
            catch (Exception)
            {
                return imageDetails; // logging the error at Index.
            }
        }

        /// <summary>
        /// Store or update the entity
        /// </summary>
        /// <param name="entity"> entity </param>
        /// <returns> return entity result </returns>
        private async Task<TableResult> StoreOrUpdateEntityAsync(HelpInfoEntity entity)
        {
            await this.EnsureInitializedAsync();

            TableOperation addOrUpdateOperation = TableOperation.InsertOrReplace(entity);

            return await this.helpInfoCloudTable.ExecuteAsync(addOrUpdateOperation);
        }

        /// <summary>
        /// Create teams table if it doesnt exists
        /// </summary>
        /// <param name="connectionString">storage account connection string</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation task which represents table is created if its not existing.</returns>
        private async Task InitializeTableAsync(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            this.helpInfoCloudTable = cloudTableClient.GetTableReference(StorageInfo.HelpInfoTableName);
            await this.helpInfoCloudTable.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Create teams table if it doesnt exists
        /// </summary>
        /// <param name="connectionString">storage account connection string</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation task which represents table is created if its not existing.</returns>
        private async Task InitializeBlobAsync(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            this.helpInfoImageCloudBlobContainer = blobClient.GetContainerReference(StorageInfo.HelpImageBlobContainer);

            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Container,
            };

            await this.helpInfoImageCloudBlobContainer.CreateIfNotExistsAsync();
            await this.helpInfoImageCloudBlobContainer.SetPermissionsAsync(permissions);
        }

        /// <summary>
        /// Initialization of InitializeAsync method which will help in creating table and blob
        /// </summary>
        /// <returns>Task</returns>
        private async Task EnsureInitializedAsync()
        {
            await this.initializeTableTask.Value;
            await this.initializeBlobTask.Value;
        }
    }
}
