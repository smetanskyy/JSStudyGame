using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSStudyGame.ViewModels
{
    class PlayerAdditionalInfoVM
    {
        public int IdPlayerAdditionalInfo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Photo { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
    }
}
