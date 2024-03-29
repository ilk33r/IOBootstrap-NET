﻿using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common;

public class IOGenerateBOPageFilesRequestModel : IORequestModel
{
    [Required]
    public string EntityName { get; set; }

    [Required]
    public string EntityDisplayName { get; set; }

    [Required]
    public string EntityItemName { get; set; }

    [Required]
    public IList<IOBOPageEntityModel> Properties { get; set; }

    [Required]
    public string ListEntityName { get; set; }

    [Required]
    public string ListEntityAPIPath { get; set; }

    [Required]
    public string ListEntityDisplayName { get; set; }

    [Required]
    public string UpdateEntityName { get; set; }

    [Required]
    public string UpdateEntityAPIPath { get; set; }

    [Required]
    public string UpdateEntityDisplayName { get; set; }

    [Required]
    public string DeleteEntityName { get; set; }

    [Required]
    public string DeleteEntityAPIPath { get; set; }

    [Required]
    public string DeleteEntityDisplayName { get; set; }

    [Required]
    public string CreateEntityName { get; set; }

    [Required]
    public string CreateEntityAPIPath { get; set; }

    [Required]
    public string CreateEntityDisplayName { get; set; }
}
