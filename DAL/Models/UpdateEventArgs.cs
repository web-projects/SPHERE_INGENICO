using System;

namespace IPA.DAL.RBADAL.Models 
{
    public class UpdateEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
