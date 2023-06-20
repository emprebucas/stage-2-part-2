namespace EcommerceApp.DTOs
{
    /// <summary>
    /// UserDto represents a Data Transfer Object (DTO) for an user. 
    /// It contains properties that are used for transferring user data between different layers or components of the application.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// UserId represents the unique identifier for the user.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Name represents the name of the user.
        /// </summary>
        public string? Name { get; set; }
    }
}
