using System;
using System.Collections.Generic;

namespace ints;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}
