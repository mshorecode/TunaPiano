using System.ComponentModel.DataAnnotations;
namespace TunaPiano.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
        [Required]
        public string Bio {  get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}
