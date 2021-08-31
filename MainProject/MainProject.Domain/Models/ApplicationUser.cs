namespace MainProject.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Common;
    using Microsoft.AspNetCore.Identity;

    [Table("AspNetUsers")]
    public sealed class ApplicationUser : IdentityUser<Guid>, IEntity
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
            UserName = Id.ToString();
        }

        [Required, MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public bool IsDisabled { get; set; }

        [InverseProperty("User")]
        public IList<ApplicationUserRole> UserRoles { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
