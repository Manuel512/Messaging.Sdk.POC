using System;

namespace Messaging.Contracts
{
    public class UserEvent
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public string FullName => $"{FirstName} {LastName}";

        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
