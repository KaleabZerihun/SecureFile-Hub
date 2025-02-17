using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileProcessor.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? File { get; set; } 
        public string? ContentType { get; set; } 
        public byte[]? FileData { get; set; }
        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key - Link to IdentityUser
        [Required]
        public string UserId { get; set; } // Foreign key

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // Navigation Property
    }
}
