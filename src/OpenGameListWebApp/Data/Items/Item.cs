﻿using OpenGameListWebApp.Data.Comments;
using OpenGameListWebApp.Data.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenGameListWebApp.Data.Items
{
    public class Item
    {
        #region Constructor
        public Item()
        {

        }
        #endregion Constructor

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Flags { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ViewCount { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        #endregion Properties

        #region Related Properties
        /// <summary>
        /// Current Item's author: this property will be loaded on first use using EF's Lazy-Loading feature
        /// </summary>
        [ForeignKey ("UserId")]
        public virtual ApplicationUser Author { get; set; }

        public virtual List<Comment> Comments { get; set; }
        #endregion Related Properties
    }
}
