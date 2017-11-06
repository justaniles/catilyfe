﻿namespace CatiLyfe.Backend.Web.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CatiLyfe.Backend.Web.Models;
    using CatiLyfe.DataLayer;

    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The post controller.
    /// </summary>
    [Route("/[controller]")]
    public class PostController : Controller
    {
        /// <summary>
        /// The data layer implementation.
        /// </summary>
        private readonly ICatiDataLayer datalayer;

        /// <summary>
        /// Initializes the post controller.
        /// </summary>
        /// <param name="datalayer">The data layer.</param>
        public PostController(ICatiDataLayer datalayer)
        {
            this.datalayer = datalayer;
        }

        /// <summary>
        /// Get many posts.
        /// </summary>
        /// <param name="top">The maximum number of posts to return.</param>
        /// <param name="skip">Skip for paging.</param>
        /// <param name="startDate">The startdate we are searching for.</param>
        /// <param name="endDate">The endate we are searching for.</param>
        /// <param name="tags">Tags to search by.</param>
        /// <returns>The posts if any.</returns>
        [HttpGet]
        public async Task<IEnumerable<PostModel>> GetMany(int? top, int? skip, DateTime? startDate, DateTime? endDate, IEnumerable<string> tags)
        {
            var posts = await this.datalayer.GetPost(top, skip, startDate, endDate, tags ?? Enumerable.Empty<string>(), false);
            return posts.Select(p => new PostModel(p));
        }

        /// <summary>
        /// Get a single post by its slug.
        /// </summary>
        /// <param name="slug">The slug of the post.</param>
        /// <returns>The post if it exist.</returns>
        [HttpGet("{slug}")]
        public async Task<PostModel> GetSingle(string slug)
        {
            var post = await this.datalayer.GetPost(slug, false);
            return new PostModel(post);
        }
    }
}