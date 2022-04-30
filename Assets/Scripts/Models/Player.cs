using System.ComponentModel.DataAnnotations;

namespace PKU.DataModels
{
    public class PlayerData
    {
        public string NickName { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }
        [Key]
        public int Id { get; set; }

    }
}

