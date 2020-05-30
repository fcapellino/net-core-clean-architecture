namespace MainProject.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Common;
    using Newtonsoft.Json;

    [Table("Events")]
    public class Event : IEntity
    {
        public Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow.ToLocalTime();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; }

        [Required, MinLength(64), MaxLength(64)]
        public string SmartContractEventId { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string InternalData { get; set; }

        [NotMapped]
        public object Data
        {
            get => InternalData == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(InternalData);
            set => InternalData = JsonConvert.SerializeObject(value);
        }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
